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
    public IActionResult Grava()
    {
        return Ok();
    }


    [HttpGet("[action]/{id}")]
    public IActionResult Exclui(int id)
    {
        return Ok();
    }




}
