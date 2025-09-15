using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
//using Novell.Directory.Ldap;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;


namespace Cmdb.Api.Seg;

[Route("api/seg/[controller]")]
public class Usuario : Controller
{
    private readonly Model.Db _db;
    private readonly IConfiguration _configuration;

    public Usuario(Model.Db db, IConfiguration configuration)
    {
        this._db = db;
        this._configuration = configuration;
    }

    [AllowAnonymous]
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


        var x = _db.SegUsuario.ToList();
        var localizado = _db.SegUsuario.FirstOrDefault(x => x.Identificacao.ToLower() == item.identificacao.ToLower());
        if (localizado == null)
            return BadRequest(new MensagemErro("usuario não localizado"));
        if (!localizado.Ativo)
            return BadRequest(new MensagemErro("usuário inativo"));

        if (item.local)
        {
            if (localizado.Senha != (localizado.Id.ToString() + item.senha).ToSha512())
                return BadRequest(new MensagemErro("usuário ou senha incorretos"));
        } else
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
        //return Ok();
    }


    [HttpGet("[action]")]
    [Authorize(Roles = "admin")]
    public IActionResult Lista()
    {
        return Ok(_db.SegUsuario.ToList());
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
    public Model.Seg.Usuario Grava([FromBody] Model.Seg.Usuario usuario)
    {
        if (usuario == null)
            throw new Exception("Parametro não informado");

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



    [HttpGet("[action]")]
    [AllowAnonymous]
    public IActionResult Teste()
    {
        using Novell.Directory.Ldap.LdapConnection cn = new();
        //string SearchBase = "dc=example,dc=com";
        string SearchBase = "dc=cmdb,dc=com";
        string[] PropsUser = { "sn", "cn", "uid", "telephoneNumber", "mail" };
        //cn.ConnectAsync("ldap.forumsys.com", 389).Wait();
        cn.ConnectAsync("192.168.0.100", 389).Wait();
        //cn.BindAsync(null, null).Wait();
        cn.BindAsync("uid=john,ou=People,dc=cmdb,dc=com", "oculos").Wait();
        //string searchFilter = $"(&(objectClass=person)(cn=Isaac Newton))";
        string searchFilter = $"(&(objectClass=person)(uid=john))";
        var Pesquisa = cn.SearchAsync(SearchBase, Novell.Directory.Ldap.LdapConnection.ScopeSub, searchFilter, PropsUser, false).Result;
        UsuarioReply? retorno = null;
        if (Pesquisa.HasMoreAsync().Result)
        {
            try
            {
                var item = Pesquisa.NextAsync().Result;
                retorno = new();
                string completo = item.ToString().ToLower();
                retorno.Dn = item.Dn;
                if (completo.IndexOf("mail") >= 0)
                    retorno.Email = item.Get("mail")?.StringValue;
                if (completo.IndexOf("description") >= 0)
                    retorno.Descricao = item.Get("Description").StringValue;
                if (completo.IndexOf("name") >= 0)
                    retorno.Nome = item.Get("Name").StringValue;
                if (completo.IndexOf("displayname") >= 0)
                    retorno.NomeExibicao = item.Get("displayName").StringValue;
                if (completo.IndexOf("samaccountname") >= 0)
                    retorno.SammAccount = item.Get("SamAccountName").StringValue;
                return Ok(retorno);

            }
            catch (Exception ex)
            {
                return Ok(new { erro = ex.Message });
            }
        }

        return Ok(new { msg = "não acho nada" });
    }


    private ConexaoLdap DadosConexaoLdap()
    {
        List<long> orgs = new() { 12, 7, 10, 9, 8, 6, 5, 2 ,11};
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
            DN= valores.FirstOrDefault(p => p.Id == 11)?.ValorTexto ?? string.Empty,
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
        cn.ConnectAsync(cnLdap.Servidor, cnLdap.Porta).Wait();
        try
        {
            string strBind = string.Format(cnLdap.DN, usuario) ;
            cn.BindAsync(strBind, senha).Wait();
        } catch (Exception)
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


    //[HttpGet("[action]")]
    //[AllowAnonymous]
    //public IActionResult teste2()
    //{
    //    string ldapServer = "192.168.0.100"; // e.g., "ldap.example.com"
    //    int ldapPort = 389; // or 636 for LDAPS
    //    string bindDn = "uid=john,ou=People,dc=cmdb,dc=com"; // Your bind DN
    //    string bindPassword = "oculos"; // Your bind password
    //    string searchBase = "dc=example,dc=com"; // Base DN for your search
    //    string searchFilter = "(cn=*)"; // Example filter to find all objects with a common name

    //    using System.DirectoryServices.Protocols.LdapConnection cn = new(ldapServer);
    //    cn.Credential = new System.Net.NetworkCredential(bindDn, bindPassword);
    //    cn.Bind();
    //    return Ok();
    //}


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
        string? token = valores.FirstOrDefault(p => p.Id == 16)?.ValorTexto;

        if (string.IsNullOrEmpty(strChave) || duracao is null || string.IsNullOrEmpty(token))
            throw new Exception("Token, chave ou duração inválidos");

        Guid chave;
        if (!Guid.TryParse(strChave, out chave))
            throw new Exception("Chave de criptografia não configurada");

        var jwtLimpo = Util.Descriptografa(token, chave, "AES");


        var jwtKey = Encoding.UTF8.GetBytes(jwtLimpo);
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