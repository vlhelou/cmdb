using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.IdentityModel.Tokens;
using Novell.Directory.Ldap;

using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.Json;


namespace Cmdb.Api.Seg;

[Route("api/seg/[controller]")]
public class Usuario : Controller
{
    private readonly Model.Db _db;
    private readonly IConfiguration _configuration;
    private readonly string[] PropsUser = { "mail", "displayName", "UserPrincipalName", "Description", "SamAccountName", "Name", "dn" };

    public Usuario(Model.Db db, IConfiguration configuration)
    {
        this._db = db;
        this._configuration = configuration;
    }

    [HttpPost("[action]")]
    public IActionResult Novo([FromBody] UsrForm item)
    {
        if (item.Id == 0)
        {
            var novo = item.ToUsuario();
            _db.Add(novo);
            _db.SaveChanges();
            novo.AjustaSenha(item.Senha);
            _db.SaveChanges();

        }
        return Ok();
    }


    [AllowAnonymous]
    [HttpPost("[action]")]
    public IActionResult Login([FromBody] UsrLogin item)
    {
        if (item is null)
            return BadRequest(new MensagemErro("Parâmetro não informado"));


        //var x = _db.SegUsuario.ToList();
        var localizado = _db.SegUsuario.AsNoTracking().FirstOrDefault(x => x.Identificacao.ToLower() == item.identificacao.ToLower());
        if (localizado == null)
            return BadRequest(new MensagemErro("usuario não localizado"));
        if (!localizado.Ativo)
            return BadRequest(new MensagemErro("usuário inativo"));

        if (item.local)
        {
            if (localizado.Senha != (localizado.Id.ToString() + item.senha).ToSha512())
                return BadRequest(new MensagemErro("usuário ou senha incorretos"));
        }
        else
        {
            var cnLdap = DadosConexaoLdap();
            if (!ValidaLoginLdap(item.identificacao, item.senha, cnLdap))
                return BadRequest(new MensagemErro("usuário ou senha incorretos"));
        }

        string token = GeraToken(localizado, 24);
        return Ok(new
        {
            token,
            localizado.Identificacao,
            localizado.Email,
            localizado.Administrador
        });
    }


    [HttpGet("[action]")]
    [Authorize(Roles = "admin")]
    public IActionResult Lista()
    {
        return Ok(_db.SegUsuario.AsNoTracking().OrderBy(p => p.Identificacao).ToList());
    }

    [HttpGet("[action]")]

    public IActionResult MeusOrganogramas()
    {
        int idLogado = Util.Claim2Usuario(HttpContext.User.Claims).Id;
        var localizado = _db.SegUsuario.AsNoTracking().FirstOrDefault(x => x.Id == idLogado);
        if (localizado == null)
            return BadRequest(new MensagemErro("Usuário não localizado"));
        var orgs = _db.SegVwOrganograma
            .Where(p => p.Equipe!.Any(q => q.IdUsuario == idLogado))
            .AsNoTracking()
            .ToList();
        return Ok(orgs);
    }


    [HttpPost("[action]")]
    [Authorize(Roles = "admin")]
    public Model.Seg.Usuario Grava([FromBody] UsuarioCadastro prm)
    {
        if (prm == null)
            throw new Exception("Parametro não informado");


        Model.Seg.Usuario usuario = new()
        {
            Id = prm.id,
            Identificacao = prm.identificacao,
            Email = prm.email,
            Administrador = prm.administrador,
            Ativo = prm.ativo,
            Local = prm.local
        };


        Model.Seg.Usuario retorno;
        Model.Seg.Usuario Logado = Util.Claim2Usuario(HttpContext.User.Claims);
        var Autor = _db.SegUsuario.AsNoTracking().FirstOrDefault(p => p.Id == Logado.Id);

        if (Autor == null)
            throw new Exception("Autor não localizado");

        if (!Autor.Administrador || !Autor.Ativo)
            throw new Exception("Autor inativo ou não é administrador");

        if (usuario.Id == 0)
        {
            Model.Seg.Usuario novo = new Model.Seg.Usuario();

            novo.Gd = Guid.NewGuid();
            novo.Identificacao = usuario.Identificacao;
            novo.Email = usuario.Email;
            novo.Ativo = usuario.Ativo;
            novo.Administrador = usuario.Administrador;
            novo.Local = usuario.Local;
            _db.Add(novo);

            _db.SaveChanges();
            novo.Senha = (novo.Id.ToString() + novo.Senha).ToSha512();
            _db.SaveChanges();
            retorno = novo;
        }
        else
        {

            Model.Seg.Usuario? alterado = _db.SegUsuario.Find(usuario.Id);
            if (alterado == null)
                throw new Exception("usuário a ser alterado não existe");
            alterado.Identificacao = usuario.Identificacao;
            alterado.Email = usuario.Email;
            alterado.Ativo = usuario.Ativo;
            alterado.Administrador = usuario.Administrador;
            _db.Update(alterado);
            _db.SaveChanges();
            retorno = alterado;
        }
        return retorno;

    }

    [HttpGet("[action]/{id:int}")]
    [Authorize(Roles = "admin")]
    public IActionResult Exclui(int id)
    {

        if (id == 0)
            throw new Exception("Id não informado");

        var usuario = _db.SegUsuario
            .Include(p => p.Locacoes)
            .Include(p => p.Segredos)
            .FirstOrDefault(p => p.Id == id);

        if (usuario is null)
            return BadRequest(new MensagemErro("Usuário não localizado"));


        Model.Seg.Usuario Logado = Util.Claim2Usuario(HttpContext.User.Claims);
        var Autor = _db.SegUsuario.AsNoTracking().FirstOrDefault(p => p.Id == Logado.Id);

        if (Autor == null)
            throw new Exception("Autor não localizado");

        if (!Autor.Administrador || !Autor.Ativo)
            throw new Exception("Autor inativo ou não é administrador");


        _db.Remove(usuario);
        _db.SaveChanges();

        return Ok();
    }


    [HttpGet("[action]/{identificacao}")]
    [AllowAnonymous]
    public IActionResult EsqueciSenha(string identificacao)
    {
        var localizado = _db.SegUsuario.FirstOrDefault(p => p.Identificacao.ToLower() == identificacao.ToLower());
        if (localizado is null)
            return BadRequest(new MensagemErro("Usuário não localizado"));

        if (localizado.Local == false)
            return BadRequest(new MensagemErro("Usuário não é local"));


        List<long> configuracoes = new() { 19, 20, 21, 22, 23, 24, 14 };
        var config = _db.CorpConfiguracao.AsNoTracking().Where(p => configuracoes.Contains(p.Id));
        if (config.Count() != configuracoes.Count())
            return BadRequest(new MensagemErro("configurações não informadas"));

        var porta = config.FirstOrDefault(p => p.Id == 20)?.ValorNumerico ?? 0;
        SmtpClient smtpClient = new SmtpClient(config.FirstOrDefault(p => p.Id == 19)?.ValorTexto ?? string.Empty, (int)porta);
        if (config.FirstOrDefault(p => p.Id == 21)?.ValorBoleano ?? false)
        {
            string usuario = config.FirstOrDefault(p => p.Id == 22)?.ValorTexto ?? string.Empty;
            string senha = config.FirstOrDefault(p => p.Id == 24)?.ValorTexto ?? string.Empty;
            string srtChave = config.FirstOrDefault(p => p.Id == 14)?.ValorTexto ?? string.Empty;
            if (string.IsNullOrEmpty(srtChave))
                return BadRequest(new MensagemErro("Chave de criptografia não encontrada"));
            Guid chave;
            if (!Guid.TryParse(srtChave, out chave))
                return BadRequest(new MensagemErro("Chave de criptografia inválida"));
            smtpClient.Credentials = new System.Net.NetworkCredential(usuario, Util.Descriptografa(senha, chave, "AES"));
        }
        smtpClient.EnableSsl = config.FirstOrDefault(p => p.Id == 23)?.ValorBoleano ?? false;


        localizado.ChaveTrocaSenha = Guid.NewGuid();
        localizado.ChaveValidade = DateTimeOffset.Now.AddHours(2).ToUniversalTime();
        _db.SaveChanges();

        MailMessage mail = new MailMessage();
        mail.From = new MailAddress("naoresponder@cmdb.com.br");
        mail.To.Add(localizado.Email);
        mail.Subject = "Recuperação de email";
        mail.Body = $"a chave para troca de senha é: {localizado.ChaveTrocaSenha}";

        try
        {
            smtpClient.Send(mail);
        }
        catch (Exception ex)
        {
            return BadRequest(new MensagemErro($"Falha ao enviar email: {ex.Message}"));
        }
        return Ok();
    }


    [HttpPost("[action]")]
    [AllowAnonymous]
    public IActionResult RecuperaSenhaChave([FromBody] RecuperacaoChaveTipo prm)
    {
        var localizado = _db.SegUsuario.FirstOrDefault(p => p.ChaveTrocaSenha == prm.Chave);
        if (localizado is null)
            return BadRequest(new MensagemErro("chave não localizada"));

        if (localizado.ChaveValidade < DateTimeOffset.Now)
            return BadRequest(new MensagemErro("chave já vencida"));

        try
        {
            var chave = Util.ChaveCriptografia(_db);
        }
        catch (Exception ex)
        {
            return BadRequest(new MensagemErro($"falha ao obter chave de criptografia: {ex.Message}"));
        }

        localizado.Senha = (localizado.Id.ToString() + prm.Senha).ToSha512();
        localizado.ChaveTrocaSenha = null;
        localizado.ChaveValidade = null;
        _db.SaveChanges();

        return Ok();
    }

    [HttpPost("[action]")]
    public IActionResult Pesquisa([FromBody] JsonElement prm)
    {
        string chave;
        if (prm.TryGetProperty("chave", out JsonElement jchave))
        {
            chave = jchave.GetString() ?? string.Empty;
            if (string.IsNullOrEmpty(chave))
            {
                return BadRequest(new MensagemErro("parametro vazio"));
            }
        }
        else
        {
            return BadRequest(new MensagemErro("parametro não reconhecido"));
        }
        chave = chave.Trim().ToLower();
        var lista = _db.SegUsuario.Where(p => p.Identificacao.ToLower().Contains(chave)).AsNoTracking().ToList();
        return Ok(lista);
    }


    [HttpPost("[action]")]
    public IActionResult TrocaSenha([FromBody] TrocadeSenha prm)
    {
        string senhaNova = prm.novaSenha.Trim();

        if (string.IsNullOrEmpty(senhaNova))
            return BadRequest(new MensagemErro("senha nova não informada"));
        if (senhaNova.Length < 6)
            return BadRequest(new MensagemErro("senha pequena"));

        int idLogado = Util.Claim2Usuario(HttpContext.User.Claims).Id;
        var localizado = _db.SegUsuario.FirstOrDefault(x => x.Id == idLogado);
        if (localizado == null)
            return BadRequest(new MensagemErro("Usuário não localizado"));

        if ((localizado.Id.ToString() + prm.senhaAtual).ToSha512() != localizado.Senha)
            return BadRequest(new MensagemErro("senha atual não confere"));

        localizado.Senha = (localizado.Id.ToString() + prm.novaSenha).ToSha512();
        _db.SaveChanges();

        return Ok();
    }


    [HttpGet("[action]")]
    public IActionResult Eu()
    {
        int idLogado = Util.Claim2Usuario(HttpContext.User.Claims).Id;
        var localizado = _db.SegUsuario
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == idLogado);

        if (localizado is null)
            return BadRequest(new MensagemErro("Usuário não localizado"));

        var equipes = _db.SegEquipe
            .AsNoTracking()
            .Where(p => p.IdUsuario == idLogado)
            .Include(p => p.Organograma)
            .ToList();

        localizado.Locacoes = equipes;

        return Ok(localizado);
    }


    [HttpPost("[action]")]
    public IActionResult AlteraEmail([FromBody] JsonElement prm)
    {
        if (prm.TryGetProperty("email", out JsonElement jemail))
        {
            string email = jemail.GetString()?.Trim()?? string.Empty;
            int idLogado = Util.Claim2Usuario(HttpContext.User.Claims).Id;
            var localizado = _db.SegUsuario
                .FirstOrDefault(x => x.Id == idLogado);
            if (localizado is null)
                return BadRequest(new MensagemErro("Usuário não localizado"));

            localizado.Email = email;
            _db.SaveChanges();

            return Ok(localizado);
        }
        return BadRequest(new MensagemErro("falha ao obter parâmetro"));
    }


    private ConexaoLdap DadosConexaoLdap()
    {
        List<long> orgs = new() { 2, 6, 7, 8, 9, 10, 11, 12 };
        var valores = _db.CorpConfiguracao.AsNoTracking().Where(p => orgs.Contains(p.Id));
        if (valores.Count() != orgs.Count)
            throw new Exception("Configuração de ldap não encontrada");
        string srtChave = valores.FirstOrDefault(p => p.Id == 2)?.ValorTexto ?? string.Empty;
        if (string.IsNullOrEmpty(srtChave))
            throw new Exception("Chave de criptografia não encontrada");
        Guid chave;
        if (!Guid.TryParse(srtChave, out chave))
            throw new Exception("Chave de criptografia inválida");

        string propriedades = valores.FirstOrDefault(p => p.Id == 10)?.ValorComplexo ?? string.Empty;
        if (string.IsNullOrEmpty(propriedades))
            throw new Exception("Propriedade de usuário não informada");


        ConexaoLdap cnLadap = new()
        {
            PesquisaNomeusuario = valores.FirstOrDefault(p => p.Id == 12)?.ValorTexto ?? string.Empty,
            Porta = (int)(valores.FirstOrDefault(p => p.Id == 7)?.ValorNumerico ?? 0),
            SearchBase = valores.FirstOrDefault(p => p.Id == 9)?.ValorTexto ?? string.Empty,
            Senha = valores.FirstOrDefault(p => p.Id == 8)?.ValorTexto ?? string.Empty,
            Servidor = valores.FirstOrDefault(p => p.Id == 6)?.ValorTexto ?? string.Empty,
            Usuario = valores.FirstOrDefault(p => p.Id == 5)?.ValorTexto ?? string.Empty,
            DN = valores.FirstOrDefault(p => p.Id == 11)?.ValorTexto ?? string.Empty,
        };
        cnLadap.Senha = Util.Descriptografa(cnLadap.Senha, chave, "AES");
        try
        {
            var props = JsonSerializer.Deserialize<PropriedadeLdap>(propriedades);
            if (props == null)
                throw new Exception("Propriedade de usuário inválida");
            cnLadap.Propriedades = props;
        }
        catch (Exception)
        {
            throw new Exception("falha ao obter as propriedades");
        }

        return cnLadap;
    }

    private bool ValidaLoginLdap(string usuario, string senha, ConexaoLdap cnLdap)
    {
        using Novell.Directory.Ldap.LdapConnection cn = new();
        cn.ConnectAsync(cnLdap.Servidor, (int)cnLdap.Porta).Wait();

        cn.BindAsync(cnLdap.DN, cnLdap.Senha).Wait();
        string searchFilter = string.Format(cnLdap.PesquisaNomeusuario, usuario);
        var Pesquisa = cn.SearchAsync(cnLdap.SearchBase, LdapConnection.ScopeSub, searchFilter, PropsUser, false).Result;

        string dnUsuarioLogin = string.Empty;
        if (Pesquisa.HasMoreAsync().Result)
        {
            try
            {
                var item = Pesquisa.NextAsync().Result;
                string completo = item.ToString().ToLower();
                dnUsuarioLogin = item.Dn;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }




        try
        {

            cn.BindAsync(dnUsuarioLogin, senha).Wait();
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }


    private record PropriedadeLdap
    {
        public string? Email { get; set; }
        public string? Descricao { get; set; }
        public string? Nome { get; set; }
        public string? SammAccount { get; set; }
    }

    private record ConexaoLdap
    {
        public string PesquisaNomeusuario { get; set; } = string.Empty;
        public int Porta { get; set; }
        public string SearchBase { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Servidor { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string DN { get; set; } = string.Empty;
        public PropriedadeLdap Propriedades { get; set; } = new();

    }

    public record RecuperacaoChaveTipo
    {
        public Guid Chave { get; set; }
        public string Senha { get; set; } = string.Empty;
    }


    public record TrocadeSenha(string senhaAtual, string novaSenha);
    public record UsuarioCadastro(int id, string identificacao, string email, bool administrador, bool ativo, bool local);

    public record UsrForm
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool Administrador { get; set; }

        public string Senha { get; set; } = string.Empty;


        public Model.Seg.Usuario ToUsuario()
        {
            return new Model.Seg.Usuario
            {
                Id = this.Id,
                Identificacao = this.Nome,
                Email = this.Email,
                Administrador = this.Administrador,
                Ativo = true
            };
        }
    }

    public record UsrLogin
    {
        public string identificacao { get; set; } = string.Empty;

        public string senha { get; set; } = string.Empty;
        public bool local { get; set; }
    }

    private record UsuarioReply
    {
        public string? Dn { get; set; }
        public string? Email { get; set; }
        public string? Descricao { get; set; }
        public string? Nome { get; set; }
        public string? NomeExibicao { get; set; }
        public string? SammAccount { get; set; }

    }

    private string GeraToken(Model.Seg.Usuario usuario, int validadeHoras)
    {
        List<long> orgs = new() { 2, 16, 17 };
        var valores = _db.CorpConfiguracao.AsNoTracking().Where(p => orgs.Contains(p.Id));
        if (valores.Count() != orgs.Count)
            throw new Exception("Configuração de orgs não encontrada");
        string? strChave = valores.FirstOrDefault(p => p.Id == 2)?.ValorTexto;
        decimal? duracao = valores.FirstOrDefault(p => p.Id == 17)?.ValorNumerico;
        string? chaveJWT = valores.FirstOrDefault(p => p.Id == 16)?.ValorTexto;

        if (string.IsNullOrEmpty(strChave) || duracao is null || string.IsNullOrEmpty(chaveJWT))
            throw new Exception("Token, chave ou duração inválidos");

        Guid chave;
        if (!Guid.TryParse(strChave, out chave))
            throw new Exception("Chave de criptografia não configurada");




        var jwtKey = Encoding.UTF8.GetBytes(chaveJWT);
        var tokenHandler = new JwtSecurityTokenHandler();
        var role = usuario.Administrador ? "admin" : "user";
        ClaimsIdentity claim = new ClaimsIdentity(new Claim[]
        {
            new Claim("id",usuario.Id.ToString()),
            new Claim(ClaimTypes.Name,usuario.Identificacao),
            new Claim(ClaimTypes.Email,usuario.Email),
            new Claim(ClaimTypes.Role,role)
        });
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claim,
            Expires = DateTime.UtcNow.AddHours(validadeHoras),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(jwtKey), SecurityAlgorithms.HmacSha256Signature)
        };

        var retorno = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(retorno);
    }
}