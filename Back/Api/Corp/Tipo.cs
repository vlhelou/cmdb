using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cmdb.Api.Corp;

[Route("api/corp/[controller]")]
[ApiController]
public class Tipo : ControllerBase
{
    private readonly Model.Db _db;
    public Tipo(Model.Db db)
    {
        _db = db;
    }

    [HttpGet("[action]")]
    public IActionResult ListaAtivos([FromQuery] string grupo)
    {
        var retorno = _db.CorpTipo
            .AsNoTracking()
            .Where(p => p.Ativo && p.Grupo.ToLower().Equals(grupo.ToLower()))
            .OrderBy(p => p.Nome)
            .ToList();
        return Ok(retorno);
    }


    [HttpGet("[action]")]
    public IActionResult Lista()
    {
        var retorno = _db.CorpTipo
            .AsNoTracking()
            .OrderBy(p => p.Nome)
            .ToList();
        return Ok(retorno);
    }

    [HttpPost("[action]")]
    [Authorize(Roles = "admin")]
    public IActionResult Grava([FromBody] Model.Corp.Tipo item)
    {
        if (item.Id == 0)
        {
            _db.CorpTipo.Add(item);
            _db.SaveChanges();
        }
        else
        {
            var localizado = _db.CorpTipo.AsNoTracking().FirstOrDefault(p => p.Id == item.Id);
            if (localizado == null)
                return BadRequest(new MensagemErro("Tipo não localizado"));
            localizado.Nome = item.Nome;
            localizado.Grupo = item.Grupo;
            localizado.Ativo = item.Ativo;
            _db.CorpTipo.Update(localizado);
            _db.SaveChanges();
        }
        return Ok();
    }

    [HttpGet("[action]/{id}")]
    [Authorize(Roles = "admin")]
    public IActionResult Exclui(int id)
    {
        var localizado = _db.CorpTipo.AsNoTracking().FirstOrDefault(p => p.Id == id);
        if (localizado == null)
            return BadRequest(new MensagemErro("Tipo não localizado"));
        try
        {

            _db.CorpTipo.Remove(localizado);
            _db.SaveChanges();
        }
        catch (Exception ex)
        {
            return BadRequest(new MensagemErro("erro exclusão de tipo: " + ex.Message));
        }
        return Ok();
    }

}
