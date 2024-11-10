using Cmdb.Model.Seg;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations.Schema;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Serialization;

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
        var localizado = _db.SegUsuario.FirstOrDefault(x => x.Email.ToLower() == item.Email.ToLower());
        if (localizado == null)
            return Unauthorized(new MensagemErro("usuario não localizado"));
        if (localizado.Senha != (localizado.Id.ToString() + item.Senha).ToSha512())
            return Unauthorized(new MensagemErro("usuário ou senha incorretos"));

        string token = GeraToken(localizado, 24);
        return Ok(new {
            token,
            localizado.Nome,
            localizado.Email
        });
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
        public string Email { get; set; } = string.Empty;

        public string Senha { get; set; } = string.Empty;
    }

    private string GeraToken(Model.Seg.Usuario usuario, int validadeHoras)
    {
        var jwtKey = Convert.FromBase64String(_configuration.GetValue<string>("jwt") ?? string.Empty);
        var tokenHandler = new JwtSecurityTokenHandler();
        var role = usuario.Administrador ? "admin" : "user";
        ClaimsIdentity claim = new ClaimsIdentity(new Claim[]
        {
            new Claim("id",usuario.Id.ToString()),
            new Claim("nome",usuario.Nome),
            new Claim("email",usuario.Email),
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