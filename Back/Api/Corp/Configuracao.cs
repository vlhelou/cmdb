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
    private readonly Guid _chave ;
    public Configuracao(Model.Db db)
    {
        _db = db;
        var strChave= _db.CorpVwConfiguracao.AsNoTracking().FirstOrDefault(p => p.Id == 2)?.ValorTexto;
        if (string.IsNullOrWhiteSpace(strChave) || !Guid.TryParse(strChave.Trim(), out _chave))
            throw new Exception("Chave de criptografia não configurada");

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

    [HttpPost("[action]")]
    public IActionResult GravaValor([FromBody] Model.Corp.VwConfiguracao item)
    {
        if (item is null)
            return BadRequest(new { Mensagem = "Item não informado" });



        var localizado = _db.CorpConfiguracao.FirstOrDefault(x => x.Id == item.Id);
        if (localizado is null)
            return BadRequest(new { Mensagem = "Item não localizado" });

        if (localizado.ValorSensivel && item.ValorTexto == "*****")
            return BadRequest(new { Mensagem = "valor sensível não alterado" });

        switch (localizado.TipoValor)
        {
            case "numerico":
                localizado.ValorNumerico = item.ValorNumerico;
                break;
            case "texto":
                if (localizado.ValorSensivel)
                    localizado.ValorTexto = Util.Criptografa(item.ValorTexto ?? string.Empty, _chave);
                else
                    localizado.ValorTexto = item.ValorTexto;
                break;
            case "data":
                localizado.ValorData = item.ValorData;
                break;
            case "complexo":
                localizado.ValorComplexo = item.ValorComplexo;
                break;

            case "boleano":
                localizado.ValorBoleano= item.ValorBoleano;
                break;
            default:
                return BadRequest(new { Mensagem = "Tipo de valor não suportado" });
        }

        _db.SaveChanges();
        if (localizado.ValorSensivel)
        {
            _db.Entry(localizado).State = EntityState.Detached;
            localizado.ValorTexto = "*****";
        }

        return Ok(localizado);
    }


}
