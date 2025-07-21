using Microsoft.AspNetCore.Http;
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
}
