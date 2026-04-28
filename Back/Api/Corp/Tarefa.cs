using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cmdb.Api.Corp;

[Route("api/corp/[controller]")]
[ApiController]
public class Tarefa : ControllerBase
{
    private readonly Model.Db _db;

    public Tarefa(Model.Db db)
    {
        _db = db;
    }


    [HttpPost("[action]")]
    public IActionResult Grava(Model.Corp.Tarefa item)
    {
        if (item.Id == 0)
        {
            _db.CorpTarefa.Add(item);
            _db.SaveChanges();
        }
        else
        {
            var localizado = _db.CorpTarefa.AsNoTracking().FirstOrDefault(p => p.Id == item.Id);
            if (localizado == null)
                return BadRequest(new MensagemErro("Tipo não localizado"));
            localizado.Nome = item.Nome;
            localizado.Grupo = item.Grupo;
            localizado.Ativo = item.Ativo;
            _db.CorpTarefa.Update(localizado);
            _db.SaveChanges();
        }
        return Ok();
    }


    [HttpGet("[action]/{id:int}")]
    public IActionResult Exclui(int id)
    {
        if (id == 0)
            throw new Exception("Id não informado");

        var localizado = _db.CorpTarefa.FirstOrDefault(p => p.Id == id);
        if (localizado == null)
            return BadRequest(new MensagemErro("Tarefa não localizada"));

        try
        {
            _db.Remove(localizado);
            _db.SaveChanges();
        }
        catch (Exception ex)
        {
            return BadRequest(new MensagemErro("erro exclusão de tarefa: " + ex.Message));
        }

        return Ok();
    }


    [HttpGet("[action]/{id:int}")]
    public IActionResult ComplementoExclui(int id)
    {
        if (id == 0)
            throw new Exception("Id não informado");

        var localizado = _db.CorpTarefaComplemento.FirstOrDefault(p => p.Id == id);
        if (localizado == null)
            return BadRequest(new MensagemErro("Tarefa não localizada"));

        try
        {
            _db.Remove(localizado);
            _db.SaveChanges();
        }
        catch (Exception ex)
        {
            return BadRequest(new MensagemErro("erro exclusão de tarefa: " + ex.Message));
        }

        return Ok();
    }

}
