using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cmdb.Api.Seg;

[Route("api/[controller]")]
[ApiController]
public class Organograma : ControllerBase
{
    private readonly Model.Db _db;
    public Organograma(Model.Db db)
    {
        _db = db;
    }
}
