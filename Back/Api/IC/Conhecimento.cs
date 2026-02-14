using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OllamaSharp;

namespace Cmdb.Api.IC;

[Route("api/ic/[controller]")]
[ApiController]
public class Conhecimento : ControllerBase
{
    private readonly Model.Db _db;
    private readonly bool embeddingHabilitado;
    private readonly OllamaApiClient _service;


    public Conhecimento(Model.Db db, OllamaApiClient service)
    {
        _db = db;
        embeddingHabilitado = db.CorpConfiguracao.AsNoTracking().FirstOrDefault(p => p.Id == 28)?.ValorBoleano ?? false;
        _service = service;
    }


    [HttpGet("[action]/{id}")]
    public IActionResult ConhecimentosPorIC(int id)
    {
        var retorno = _db.IcConhecimento.AsNoTracking().Where(p => p.IdIc == id).ToList();
        return Ok(retorno);
    }


    [HttpPost("[action]")]
    public IActionResult Grava([FromBody] Model.IC.Conhecimento item)
    {
        if (item.Id == 0)
        {
            _db.IcConhecimento.Add(item);
            _db.SaveChanges();
            if (embeddingHabilitado)
                Services.IcService.AtualizaEmbedding(_db, _service, item.Id);
            return Ok(item);
        }
        else
        {
            int idLogado = Util.Claim2Usuario(HttpContext.User.Claims).Id;

            var localizado = _db.IcConhecimento.FirstOrDefault(p => p.Id == item.Id);
            if (localizado == null)
            {
                return BadRequest(new MensagemErro("Conhecimento não localizado"));
            }
            localizado.DataAlteracao = DateTimeOffset.Now.ToUniversalTime();
            localizado.IdAutor = idLogado;
            localizado.Problema = item.Problema;
            localizado.Solucao = item.Solucao;
            _db.IcConhecimento.Update(localizado);
            _db.SaveChanges();
            if (embeddingHabilitado)
                Services.IcService.AtualizaEmbedding(_db, _service, item.Id);

            return Ok(localizado);

        }

    }


    [HttpGet("[action]/{id}")]
    public IActionResult Exclui(int id)
    {
        var localizado = _db.IcConhecimento.FirstOrDefault(p => p.Id == id);
        if (localizado == null)
        {
            return BadRequest(new MensagemErro("Conhecimento não localizado"));
        }
        _db.IcConhecimento.Remove(localizado);
        _db.SaveChanges();
        return Ok();
    }




}
