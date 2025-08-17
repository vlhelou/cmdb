using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    [HttpGet("[action]")]
    public IActionResult MeusSegredos()
    {
        int idLogado = Util.Claim2Usuario(HttpContext.User.Claims).Id;
        var localizado = _db.SegUsuario.AsNoTracking().FirstOrDefault(x => x.Id == idLogado);
        if (localizado == null)
            return BadRequest(new MensagemErro("Usuário não localizado"));

        var organograma = _db.SegVwOrganograma
            .Where(p => p.Equipe!.Any(q => q.IdUsuario == idLogado))
            .AsNoTracking()
            .Select(p => p.Id)
            .ToList<int>(); ;

        var segredos = _db.IcSegredo
             .Where(p =>
                 (p.IdUsuarioDono.HasValue && p.IdUsuarioDono == idLogado)
                 || (p.IdOrganogramaDono.HasValue && organograma.Contains(p.IdOrganogramaDono.Value))
                 )
             .AsNoTracking()
             .ToList();

        return Ok();
    }
}
