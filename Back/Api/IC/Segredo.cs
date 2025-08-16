using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cmdb.Api.IC;

[Route("api/IC/[controller]")]
[ApiController]
public class Segredo : ControllerBase
{
    private readonly Model.Db _db;
    public Segredo(Model.Db db)
    {
        _db = db;
    }
}
