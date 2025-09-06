using Cmdb.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    [HttpGet("[action]")]
    [AllowAnonymous]
    public IActionResult ArvoreCompleta()
    {
        var listaOriginal = _db.CorpVwConfiguracao
            .OrderBy(c => c.Nivel).ThenBy(c => c.Nome)
            .AsNoTracking()
            .ToList();
        listaOriginal.Where(p => p.ValorSensivel).ToList().ForEach(p => p.ValorTexto = "*****");

        List<Model.Corp.VwConfiguracao> retorno = new();
        foreach (var item in listaOriginal.Where(p => p.IdPai == null))
        {
            retorno.Add(item);
            AdicionarFilhosArvore(item, listaOriginal);
        }
        return Ok(retorno);
    }

    private void AdicionarFilhosArvore(Model.Corp.VwConfiguracao pai, List<Model.Corp.VwConfiguracao> listaOriginal)
    {
        pai.Filhos = listaOriginal.Where(p => p.IdPai == pai.Id).ToList();
        foreach (var item in pai.Filhos)
        {
            AdicionarFilhosArvore(item, listaOriginal);
        }
    }

}
