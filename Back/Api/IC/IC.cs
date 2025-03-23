using Cmdb.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Cmdb.Api.IC;

[Route("api/IC/[controller]")]
[ApiController]
public class IC : ControllerBase
{
    private readonly Model.Db _db;
    public IC(Model.Db db)
    {
        _db = db;
    }


}
