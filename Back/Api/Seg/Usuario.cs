using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cmdb.Api.Seg;

[Route("api/seg/[controller]")]
public class Usuario(Model.Db db) : Controller
{
    private readonly Model.Db _db = db;

    [AllowAnonymous]
    [HttpPost("[action]")]
    public IActionResult Novo(Model.Seg.Usuario item)
    {
        
        return Ok();
    }
}