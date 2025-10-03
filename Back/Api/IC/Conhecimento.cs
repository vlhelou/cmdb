using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cmdb.Api.IC;

[Route("api/ic/[controller]")]
[ApiController]
public class Conhecimento : ControllerBase
{
    private readonly Model.Db _db;
    public Conhecimento(Model.Db db)
    {
        _db = db;
    }


    [HttpGet("[action]/{id}")]
    public IActionResult ConhecimentosPorIC(int id)
    {
        var retorno = _db.IcConhecimento.AsNoTracking().Where(p => p.IdIC == id).ToList();
        return Ok(retorno);
    }


    [HttpPost("[action]")]
    public IActionResult Grava([FromBody] Model.IC.Conhecimento item)
    {
        if (item.Id == 0)
        {
            _db.IcConhecimento.Add(item);
            _db.SaveChanges();
            return Ok(item);
        }
        else
        {
            int idLogado = Util.Claim2Usuario(HttpContext.User.Claims).Id;

            var localizado = _db.IcConhecimento.FirstOrDefault(p => p.Id == item.Id);
            if (localizado == null)
            {
                return BadRequest(new MensagemErro("Conhecimento não localizado"));
            }
            localizado.DataAlteracao = DateTimeOffset.Now;
            localizado.IdUsuario = idLogado;
            localizado.Problema = item.Problema;
            localizado.Solucao = item.Solucao;
            _db.IcConhecimento.Update(localizado);
            _db.SaveChanges();
            return Ok(localizado);

        }

    }


    [HttpGet("[action]/{id}")]
    public IActionResult Exclui(int id)
    {
        var localizado = _db.IcConhecimento.FirstOrDefault(p => p.Id == id);
        if (localizado == null)
        {
            return BadRequest(new MensagemErro("Conhecimento não localizado"));
        }
        _db.IcConhecimento.Remove(localizado);
        _db.SaveChanges();
        return Ok();
    }




}
