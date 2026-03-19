using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cmdb.Api.Corp;

[Route("api/corp/[controller]")]
[ApiController]
public class Configuracao : ControllerBase
{
    private readonly Model.Db _db;
    private readonly Guid _chave;
    private readonly ILogger<Configuracao> _logger;
    public Configuracao(Model.Db db, ILogger<Configuracao> logger)
    {
        _db = db;
        var strChave = _db.CorpVwConfiguracao.AsNoTracking().FirstOrDefault(p => p.Id == 2)?.ValorTexto;
        _logger = logger;
        if (string.IsNullOrWhiteSpace(strChave) || !Guid.TryParse(strChave.Trim(), out _chave))
        {
            _logger.LogError("Chave de criptografia não configurada ou inválida.");
            throw new Exception("Chave de criptografia não configurada");
        }

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


        string valorNovo = string.Empty;
        switch (localizado.TipoValor)
        {
            case "numerico":
                localizado.ValorNumerico = item.ValorNumerico;
                valorNovo = item.ValorNumerico?.ToString() ?? "null";
                break;
            case "texto":
                if (localizado.ValorSensivel)
                {
                    localizado.ValorTexto = Util.Criptografa(item.ValorTexto ?? string.Empty, _chave);
                    valorNovo = "*****";
                }
                else
                {
                    localizado.ValorTexto = item.ValorTexto;
                    valorNovo = item.ValorTexto ?? "null";
                }
                break;
            case "data":
                localizado.ValorData = item.ValorData;
                valorNovo = item.ValorData?.ToString("o") ?? "null";
                break;
            case "complexo":
                localizado.ValorComplexo = item.ValorComplexo;
                valorNovo = item.ValorComplexo ?? "null";
                break;

            case "boleano":
                localizado.ValorBoleano = item.ValorBoleano;
                valorNovo = item.ValorBoleano?.ToString() ?? "null";
                break;
            default:
                _logger.LogWarning($"Tipo de valor não suportado para configuração: {localizado.Id} - {localizado.Nome}, tipo informado: {localizado.TipoValor}");
                return BadRequest(new { Mensagem = "Tipo de valor não suportado" });
        }

        _db.SaveChanges();
        if (localizado.ValorSensivel)
        {
            _db.Entry(localizado).State = EntityState.Detached;
            localizado.ValorTexto = "*****";
        }
        _logger.LogInformation($"Valor da configuração ajustado: {localizado.Id} - {localizado.Nome}, com o valor de : {valorNovo}");

        return Ok(localizado);
    }


}
