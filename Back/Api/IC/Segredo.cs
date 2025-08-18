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

    [HttpGet("[action]/{id}")]
    public IActionResult MeusSegredosPorIc(int id)
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
                 (p.IdUsuarioDono.HasValue && p.IdUsuarioDono == idLogado && p.IdIC==id)
                 || (p.IdOrganogramaDono.HasValue && organograma.Contains(p.IdOrganogramaDono.Value))
                 )
             .AsNoTracking()
             .Include(p=>p.UsuarioDono)
             .Include(p=>p.OrganogramaDono)
             .Include(p=>p.IC)
             .ToList();

        return Ok(segredos);
    }

    [HttpPost("[action]")]
    public IActionResult Novo([FromBody] SegredoItem item)
    {
        if (item == null)
            return BadRequest(new MensagemErro("Dados inválidos"));
        if (string.IsNullOrEmpty(item.conteudo))
            return BadRequest(new MensagemErro("Conteúdo não pode ser vazio"));
        if (item.idUsuarioDno == null && item.IdOrganogramaDono == null)
            return BadRequest(new MensagemErro("Deve informar dono do segredo"));

        int idLogado = Util.Claim2Usuario(HttpContext.User.Claims).Id;
        var localizado = _db.SegUsuario
            .AsNoTracking()
            .Include(p=>p.Locacoes)
            .FirstOrDefault(x => x.Id == idLogado);
        return Ok();
    }

    [HttpGet("[action]/{id}")]
    public IActionResult Exclui(int id)
    {
        return Ok();
    }

    [HttpPost("[action]")]
    public IActionResult Altera()
    {
        return Ok();
    }

    public record SegredoItem(int? idUsuarioDno, int? IdOrganogramaDono, string conteudo);

}
