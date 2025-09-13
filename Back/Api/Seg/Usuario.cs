using Cmdb.Model.Seg;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Novell.Directory.Ldap;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;


namespace Cmdb.Api.Seg;

[Route("api/seg/[controller]")]
public class Usuario(Model.Db db, IConfiguration configuration) : Controller
{
    private readonly Model.Db _db = db;
    private readonly IConfiguration _configuration = configuration;

    [AllowAnonymous]
    [HttpPost("[action]")]
    public IActionResult Novo([FromBody] UsrForm item)
    {
        if (item.Id == 0)
        {
            var novo = item.ToUsuario();
            novo.NovoGD();
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
        var localizado = _db.SegUsuario.FirstOrDefault(x => x.Email.ToLower() == item.Identificacao.ToLower());
        if (localizado == null)
            return BadRequest(new MensagemErro("usuario não localizado"));
        if (localizado.Senha != (localizado.Id.ToString() + item.Senha).ToSha512())
            return BadRequest(new MensagemErro("usuário ou senha incorretos"));

        string token = GeraToken(localizado, 24);
        return Ok(new
        {
            token,
            localizado.Nome,
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
            .Where(p=>p.Equipe!.Any(q=>q.IdUsuario==idLogado))
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
        var Autor = db.SegUsuario.AsNoTracking().FirstOrDefault(p=>p.Id==Logado.Id);

        if (Autor == null)
            throw new Exception("Autor não localizado");

        if (!Autor.Administrador || !Autor.Ativo)
            throw new Exception("Autor inativo ou não é administrador");

        if (usuario.Id == 0)
        {
            Model.Seg.Usuario novo = new Model.Seg.Usuario();

            novo.Gd = Guid.NewGuid();
            novo.Nome = usuario.Nome;
            novo.Email = usuario.Email;
            novo.Ativo = usuario.Ativo;
            novo.Administrador = usuario.Administrador;
            novo.Local = usuario.Local;
            novo.Login = usuario.Login;
            db.Add(novo);

            db.SaveChanges();
            novo.Senha = (novo.Id.ToString() + novo.Senha).ToSha512();
            db.SaveChanges();
            retorno = novo;
        }
        else
        {

            Model.Seg.Usuario alterado = db.SegUsuario.Find(usuario.Id);
            if (alterado == null)
                throw new Exception("usuário a ser alterado não existe");
            alterado.Nome = usuario.Nome;
            alterado.Email = usuario.Email;
            alterado.Ativo = usuario.Ativo;
            alterado.Administrador = usuario.Administrador;
            db.Update(alterado);
            db.SaveChanges();
            retorno = alterado;
        }
        return retorno;

    }



    [HttpGet("[action]")]
    [AllowAnonymous]
    public IActionResult Teste()
    {
        using LdapConnection cn = new();
        //string SearchBase = "dc=example,dc=com";
        string SearchBase = "dc=cmdb,dc=com";
        string[] PropsUser = { "sn", "cn", "uid", "telephoneNumber", "mail" };
        //cn.ConnectAsync("ldap.forumsys.com", 389).Wait();
        cn.ConnectAsync("192.168.0.100", 389).Wait();
        //cn.BindAsync(null, null).Wait();
        cn.BindAsync("uid=john,ou=People,dc=cmdb,dc=com", "oculos").Wait();
        //string searchFilter = $"(&(objectClass=person)(cn=Isaac Newton))";
        string searchFilter = $"(&(objectClass=person)(uid=john))";
        var Pesquisa = cn.SearchAsync(SearchBase, LdapConnection.ScopeSub, searchFilter, PropsUser, false).Result;
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
                return Ok( retorno);

            }
            catch (Exception ex)
            {
                return Ok(new { erro = ex.Message });
            }
        }

        return Ok(new { msg = "não acho nada" });
    }


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
                Nome = this.Nome,
                Email = this.Email,
                Administrador = this.Administrador,
                Ativo = true
            };
        }
    }


    public record UsrLogin
    {
        public string Identificacao{ get; set; } = string.Empty;

        public string Senha { get; set; } = string.Empty;
        public bool Local { get; set; } 
    }

    private record UsuarioReply
    {
        public string? Dn { get; set; }
        public string? Email { get; set; }
        public string? Descricao { get; set; }
        public string? Nome { get; set; }
        public string?  NomeExibicao { get; set; }
        public string? SammAccount { get; set; }

    }
    private string GeraToken(Model.Seg.Usuario usuario, int validadeHoras)
    {
        var jwtKey = Convert.FromBase64String(_configuration.GetValue<string>("jwt") ?? string.Empty);
        var tokenHandler = new JwtSecurityTokenHandler();
        var role = usuario.Administrador ? "admin" : "user";
        ClaimsIdentity claim = new ClaimsIdentity(new Claim[]
        {
            new Claim("id",usuario.Id.ToString()),
            new Claim(ClaimTypes.Name,usuario.Nome),
            new Claim(ClaimTypes.Email,usuario.Email),
            new Claim(ClaimTypes.Role,role)
        });
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claim,
            Expires = DateTime.UtcNow.AddHours(validadeHoras),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(jwtKey), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}