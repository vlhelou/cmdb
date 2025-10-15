using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cmdb.Api.IC;

[Route("api/ic/[controller]")]
[ApiController]
public class Dependencia : ControllerBase
{
    private readonly Model.Db _db;
    public Dependencia(Model.Db db)
    {
        _db = db;
    }

    [HttpGet("[action]/{id}/{dependente}")]
    public IActionResult DependenciasPorIC(int id, bool dependente)
    {
        if (dependente)
            return Ok(_db.IcDependencia
                .AsNoTracking()
                .Include(p => p.IcPrincipal)
                .Where(p => p.IdIcDependente == id)
                .OrderBy(p => p.IcPrincipal!.Nome)
                .ToList());
        else
            return Ok(_db.IcDependencia
                .AsNoTracking()
                .Include(p => p.IcDependente)
                .Where(p => p.IdPrincipal == id)
                .OrderBy(p => p.IcDependente!.Nome)
                .ToList());
    }

    [HttpPost("[action]")]
    [Authorize(Roles = "admin")]
    public IActionResult Grava([FromBody] Model.IC.Dependencia item)
    {
        int idLogado = Util.Claim2Usuario(HttpContext.User.Claims).Id;
        if (item.Id == 0)
        {
            item.DataAlteracao = DateTimeOffset.Now.ToUniversalTime();
            item.IdAutor = idLogado;
            _db.IcDependencia.Add(item);
            _db.SaveChanges();
            return Ok(item);
        }
        else
        {
            var localizado = _db.IcDependencia.FirstOrDefault(p => p.Id == item.Id);
            if (localizado == null)
            {
                return BadRequest(new MensagemErro("Dependência não localizada"));
            }
            localizado.DataAlteracao = DateTimeOffset.Now.ToUniversalTime();
            localizado.IdAutor = idLogado;
            localizado.Observacao = item.Observacao;
            localizado.IdIcDependente = item.IdIcDependente;
            localizado.IdPrincipal = item.IdPrincipal;
            _db.IcDependencia.Update(localizado);
            _db.SaveChanges();
            return Ok(localizado);
        }
    }

    [HttpDelete("[action]/{id}")]
    [Authorize(Roles = "admin")]
    public IActionResult Exclui(int id)
    {
        int idLogado = Util.Claim2Usuario(HttpContext.User.Claims).Id;
        var localizado = _db.IcDependencia.FirstOrDefault(p => p.Id == id);
        if (localizado == null)
        {
            return BadRequest(new MensagemErro("Dependência não localizada"));
        }
        _db.IcDependencia.Remove(localizado);
        _db.SaveChanges();
        return Ok();
    }
}
