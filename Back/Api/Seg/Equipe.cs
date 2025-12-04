using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cmdb.Api.Seg;

[Route("api/seg/[controller]")]
[ApiController]
public class Equipe : ControllerBase
{
    private readonly Model.Db _db;
    public Equipe(Model.Db db)
    {
        _db = db;
    }

    [HttpGet("[action]/{idic}")]
    public IActionResult UsuarioPorOrganograma(int idic)
    {
        var retorno = _db.SegEquipe
            .AsNoTracking()
            .Where(p => p.IdOrganograma == idic)
            .Include(p=>p.Usuario)
            .ToList();
        return Ok(retorno);
    }


    [HttpGet("[action]/{idusuario}")]
    public IActionResult OrganogramasPorUsuario(int idusuario)
    {
        var retorno = _db.SegEquipe
            .AsNoTracking()
            .Where(p => p.IdUsuario == idusuario)
            .Include(p => p.Organograma)
            .ToList();

        return Ok(retorno);
    }

    [HttpGet("[action]/{idusuario}/{idOrg}")]
    [Authorize(Roles = "admin")]
    public IActionResult Adicionar(int idusuario, int idOrg)
    {

        int IdLogado = Util.Claim2Usuario(HttpContext.User.Claims).Id;
        var existente = _db.SegEquipe
            .AsNoTracking()
            .Where(p => p.IdUsuario == idusuario && p.IdOrganograma == idOrg)
            .FirstOrDefault();

        if (existente != null)
            return BadRequest(new MensagemErro("Registro já existente"));

        var usuario = _db.SegUsuario.Find(idusuario);
        var organograma = _db.SegOrganograma.Find(idOrg);

        if (usuario == null || organograma == null)
            return NotFound(new MensagemErro("Usuário ou Organograma não encontrado"));

        var novaEquipe = new Model.Seg.Equipe
        {
            IdUsuario = idusuario,
            IdOrganograma = idOrg,
            Data = DateTimeOffset.Now.ToUniversalTime(),
            IdAutor = IdLogado
        };

        _db.SegEquipe.Add(novaEquipe);
        _db.SaveChanges();

        return Ok(new { Mensagem = "Implementar" });
    }


    [HttpGet("[action]/{id}")]
    [Authorize(Roles = "admin")]
    public IActionResult Exclui(int id)
    {
        var localizado  = _db.SegEquipe.Find(id);
        if (localizado != null)
        {
            _db.SegEquipe.Remove(localizado);
            _db.SaveChanges();
            return Ok();
        }
        return NotFound(new MensagemErro("Registro não encontrado"));
    }

}
