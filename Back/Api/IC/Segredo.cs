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
                p.IdIC == id
                && (p.IdUsuarioDono.HasValue && p.IdUsuarioDono == idLogado 
                    || (p.IdOrganogramaDono.HasValue && organograma.Contains(p.IdOrganogramaDono.Value))
                    )
              )
             .AsNoTracking()
             .Include(p => p.UsuarioDono)
             .Include(p => p.OrganogramaDono)
             .Include(p => p.IC)
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
        
        int idLogado = Util.Claim2Usuario(HttpContext.User.Claims).Id;

        var localizado = _db.SegUsuario
            .AsNoTracking()
            .Include(p => p.Locacoes)
            .FirstOrDefault(x => x.Id == idLogado);

        if (localizado == null)
            return BadRequest(new MensagemErro("Usuário não localizado"));

        if (item.IdOrganogramaDono==0)
        {
            //segredo de usuário
            var segredo = new Model.IC.Segredo
            {
                IdIC = item.idIc,
                IdUsuarioDono = idLogado,
                Conteudo = Util.Criptograva(item.conteudo, localizado.Gd),
                Algoritmo = "AES"
            };
            _db.IcSegredo.Add(segredo);
            _db.SaveChanges();
        }
        else
        {
            //segredo de organograma
            if (localizado.Locacoes == null || !localizado.Locacoes.Any(p => p.IdOrganograma == item.IdOrganogramaDono))
                return BadRequest(new MensagemErro("Usuário não pertence ao organograma selecionado"));
            var organograma = _db.SegOrganograma.AsNoTracking().FirstOrDefault(p => p.Id == item.IdOrganogramaDono);
            if (organograma == null)
                return BadRequest(new MensagemErro("Organograma não localizado"));
            var segredo = new Model.IC.Segredo
            {
                IdIC = item.idIc,
                IdOrganogramaDono = item.IdOrganogramaDono,
                Conteudo = Util.Criptograva(item.conteudo, organograma.Gd),
                Algoritmo = "AES"
            };
            _db.IcSegredo.Add(segredo);
            _db.SaveChanges();
        }

        return Ok();
    }

    [HttpGet("[action]/{id}")]
    public IActionResult Exclui(int id)
    {
        int idLogado = Util.Claim2Usuario(HttpContext.User.Claims).Id;
        if (!AcessoPermitido(id, idLogado))
            return BadRequest(new MensagemErro("Acesso negado"));
        var segredo = _db.IcSegredo.FirstOrDefault(p => p.Id == id);
        if (segredo == null)
            return BadRequest(new MensagemErro("Segredo não localizado"));
        _db.IcSegredo.Remove(segredo);
        _db.SaveChanges();
        return Ok();
    }

    [HttpGet("[action]/{id}")]
    public IActionResult Visualiza(int id)
    {
        int idLogado = Util.Claim2Usuario(HttpContext.User.Claims).Id;
        var segredo = _db.IcSegredo.AsNoTracking().FirstOrDefault(p => p.Id == id);
        string conteudo;
        if (segredo == null)
            return BadRequest(new MensagemErro("Segredo não localizado"));

        if (segredo.IdUsuarioDono.HasValue)
        {
            if (segredo.IdUsuarioDono != idLogado)
            {
                return BadRequest(new MensagemErro("usuário não é proprietário do segredo"));
            }
            else
            {
                var dono = _db.SegUsuario.AsNoTracking().FirstOrDefault(p => p.Id == segredo.IdUsuarioDono);
                if (dono == null)
                    return BadRequest(new MensagemErro("Proprietário do segredo não localizado"));
                conteudo = Util.Descriptograva(segredo.Conteudo, dono.Gd, segredo.Algoritmo);
                return Ok(new { conteudo});
            }
        }
        else
        {
            var organograma = _db.SegOrganograma
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == segredo.IdOrganogramaDono && p.Equipe!.Any(q=>q.IdUsuario==idLogado));
            if (organograma == null)
                return BadRequest(new MensagemErro("usuário não pertence ao organograma proprietário do segredo"));
            conteudo = Util.Descriptograva(segredo.Conteudo, organograma.Gd, segredo.Algoritmo);
            return Ok(new { conteudo });
        }


    }

    private bool AcessoPermitido(int id, int idUsuario)
    {
        var logado = _db.SegUsuario
            .AsNoTracking()
            .Include(p => p.Locacoes)
            .FirstOrDefault(x => x.Id == idUsuario);
        var segredo = _db.IcSegredo
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);
        if (logado is null || segredo is null)
            return false;
        if(segredo.IdUsuarioDono.HasValue && segredo.IdUsuarioDono == idUsuario)    
            return true;
        if(segredo.IdOrganogramaDono.HasValue && logado.Locacoes != null && logado.Locacoes.Any(p=>p.IdOrganograma == segredo.IdOrganogramaDono))   
            return true;


        return false;
    }

    public record SegredoItem(int idIc, int IdOrganogramaDono, string conteudo);

}
