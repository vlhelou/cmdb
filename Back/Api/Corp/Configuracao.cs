using Cmdb.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cmdb.Api.Corp;

[Route("api/corp/[controller]")]
[ApiController]
public class Configuracao : ControllerBase
{
    private readonly Model.Db _db;
    public Configuracao(Model.Db db)
    {
        _db = db;
    }

}
