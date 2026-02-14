using Cmdb.Model.IC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel.Embeddings;
using OllamaSharp;
using Pgvector.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Cmdb.Api.IC;

[Route("api/IC/[controller]")]
[ApiController]
public class IC : ControllerBase
{
    private readonly Model.Db _db;
    private readonly OllamaApiClient _service;
    private readonly bool embeddingHabilitado;
    public IC(Model.Db db, OllamaApiClient service)
    {
        _db = db;
        _service = service;
        embeddingHabilitado = db.CorpConfiguracao.AsNoTracking().FirstOrDefault(p => p.Id == 28)?.ValorBoleano ?? false;
    }


    [HttpGet("[action]/{id}")]
    public IActionResult Busca(int id)
    {
        var retorno = _db.IcVwIc.Where(p => p.Id == id)
            .AsNoTracking()
            .FirstOrDefault();
        return Ok(retorno);
    }


    [HttpGet("[action]/{id}")]
    public IActionResult BuscaComFamilia(int id)
    {
        var Localizado = Services.IcService.LocalizaComFamilia(_db, id);
        return Ok(Localizado);

    }


    [HttpPost("[action]")]
    public IActionResult Pesquisa([FromBody] PesquisaIC prm)
    {
        var nomes = string.Join(" & ", prm.Chave.Split(' ').Where(q => q.Trim().Length > 0).Select(p => p.Trim() + ":*"));



        var consulta = _db.IcVwIc.FromSql($"""
            select 
            	id
            	, idpai
            	, nome
            	, ativo
            	, ativofinal
            	, propriedades
            	, idtipo
            	, idorganograma
            	, nomecompleto
            	, listaancestrais
            	, pesquisats
            	, nivel
            	, ts_rank(pesquisats , to_tsquery('portuguese',{nomes})) rank
                , observacao
            from ic.vw_ic
            where pesquisats @@ to_tsquery('portuguese',{nomes})
            order by rank desc
                                    
            """)
            .AsNoTracking()
            .AsQueryable();


        if (prm.Ativo != null)
            consulta = consulta.Where(p => p.AtivoFinal == prm.Ativo);

        if (prm.FilhoDe != null)
            consulta = consulta.Where(p => p.NomeCompleto.ToLower().StartsWith(prm.FilhoDe.ToLower() + "|"));

        if (prm.Tipo != null)
            consulta = consulta.Where(p => p.IdTipo == prm.Tipo);

        var retorno = consulta.Include(p => p.Tipo).ToList();
        return Ok(retorno);

    }

    [HttpPost("[action]")]
    public async Task<IActionResult> PesquisaEmbedding([FromBody] JsonElement prm)
    {
        if (!embeddingHabilitado)
            return BadRequest(new MensagemErro("Pesquisa por embedding não está habilitada"));

        string prompt;
        if (prm.TryGetProperty("prompt", out JsonElement jprompt))
        {
            prompt = jprompt.GetString() ?? string.Empty;
        }
        else
        {
            return BadRequest(new MensagemErro("Parâmetro 'prompt' não informado"));
        }


        var embedding = _service.AsTextEmbeddingGenerationService();
        var temp = await embedding.GenerateEmbeddingAsync(JsonSerializer.Serialize(prompt));


        var primeiros = _db.IcIc.AsNoTracking()
            .Where(p => p.Embedding != null)
            .OrderBy(p => p.Embedding!.CosineDistance(new Pgvector.Vector(temp.ToArray())))
            .Take(5)
            .ToList();

        var ids = primeiros.Select(primeiros => primeiros.Id).ToList();

        var completo = _db.IcVwIc
            .Where(p => ids.Contains(p.Id))
            .Include(p => p.Tipo)
            .AsNoTracking().ToList();

        return Ok(completo);
    }


    [HttpGet("[action]")]
    public IActionResult ListaCompleta()
    {
        List<Model.IC.VwIc> final = new();
        var lista = _db.IcVwIc.AsNoTracking().OrderBy(p => p.Nivel).ThenBy(p => p.Nome).ToList();
        var retorno = lista.Where(p => p.IdPai == null).FirstOrDefault();
        if (retorno == null)
        {
            return BadRequest(new MensagemErro("Não há elemento root"));
        }
        PopulaFilhosMenu(ref retorno, ref lista);
        return Ok(retorno);
    }

    [HttpPost("[action]")]
    [Authorize(Roles = "admin")]
    public IActionResult Grava([FromBody] Model.IC.IC item)
    {
        if (item == null)
            return BadRequest(new MensagemErro("Parâmetro não informado"));

        int IdLogado = Util.Claim2Usuario(HttpContext.User.Claims).Id;


        if (item.IdPai is null)
            return BadRequest(new MensagemErro("O Item Raiz não pode ser alterado"));

        var Logado = _db.SegUsuario.Where(p => p.Id == IdLogado).AsNoTracking().FirstOrDefault();

        if (Logado == null)
            return BadRequest(new MensagemErro("Usuário inexistente"));

        if (!Logado.Ativo)
            return BadRequest(new MensagemErro("Usuário inativo"));

        if (!Logado.Administrador)
            return BadRequest(new MensagemErro("Usuário não tem permissão para alterar ICs"));



        item.Nome = item.Nome.Trim();

        if (item.Id == 0)
        {
            if (item.IdPai == null)
                return BadRequest(new MensagemErro("Sem referencia de Pai"));

            _db.Entry(item).State = EntityState.Added;
            _db.SaveChanges();
            if (embeddingHabilitado)
            {
                Services.IcService.AtualizaEmbedding(_db, _service, item.Id);
            }
            return Ok(item);
        }
        else
        {
            var localizado = _db.IcIc.Where(p => p.Id == item.Id).FirstOrDefault();
            if (localizado == null)
                return BadRequest(new MensagemErro("IC não localizado"));
            localizado.Altera(item);
            _db.SaveChanges();
            if (embeddingHabilitado)
            {
                var origem = Services.IcService.LocalizaComFamilia(_db, localizado.Id);
                var embedding = _service.AsTextEmbeddingGenerationService();
                var temp = embedding.GenerateEmbeddingAsync(JsonSerializer.Serialize(origem)).Result;
                localizado.Embedding = new Pgvector.Vector(temp);
                _db.SaveChanges();
            }

            return Ok(localizado);
        }
    }


    [HttpGet("[action]")]
    public IActionResult UsaEmbedding()
    {
        return Ok(embeddingHabilitado);
    }

    [HttpGet("[action]/{id}")]
    public IActionResult IcEditavel(int id)
    {
        bool retorno = false;
        try
        {
            retorno = this.DetectaIcEditavel(id, this.User);
        }
        catch (Exception ex)
        {
            return BadRequest(new MensagemErro(ex.Message));
        }

        return Ok(retorno);
    }

    [HttpGet("[action]/{idIc}/{idNovoPai}")]
    [Authorize(Roles = "admin")]
    public IActionResult MudaPaternidade(int idIc, int idNovoPai)
    {
        var ic = _db.IcIc.Where(p => p.Id == idIc).FirstOrDefault();
        var novoPai = _db.IcIc.Where(p => p.Id == idNovoPai).AsNoTracking().FirstOrDefault();
        if (ic is null || novoPai is null)
            return BadRequest(new MensagemErro("IC ou Novo pai não localizado"));
        ic.IdPai = idNovoPai;
        _db.SaveChanges();
        return Ok();
    }

    private VwIc LocalizaComFamilia(int id)
    {
        var localizado = _db.IcVwIc
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Include(p => p.Tipo)
            .Include(p => p.Responsavel)
            .Include(p => p.Conhecimentos)
            .FirstOrDefault();
        if (localizado == null)
            throw new Exception("IC não localizado");
        localizado.Ancestrais = _db.IcVwIc.Where(p => localizado.LstAncestrais.AsEnumerable().Contains(p.Id))
            .AsNoTracking()
            .Include(p => p.Tipo)
            .OrderBy(p => p.Nivel)
            .ToList<Model.IC.VwIc>();
        localizado.Filhos = _db.IcVwIc
            .AsNoTracking()
            .Where(p => p.IdPai == localizado.Id)
            .Include(p => p.Tipo)
            .OrderBy(p => p.Tipo!.Nome)
            .ThenBy(p => p.Nome).ToList();
        if (localizado.Ancestrais.Count > 0)
        {
            localizado.Pai = localizado.Ancestrais.Last();
        }

        return localizado;

    }

    [HttpGet("[action]")]
    [Authorize(Roles = "admin")]
    public IActionResult EmbeddingRestante()
    {
        if (!embeddingHabilitado)
            return BadRequest(new MensagemErro("Embedding não esta habilitado"));

        var lista = _db.IcIc.Where(p => p.Embedding == null).ToList();
        foreach (var item in lista)
        {
            //var localizado = this.LocalizaComFamilia(item.Id);
            //if (localizado == null)
            //    continue;
            var embedding = _service.AsTextEmbeddingGenerationService();
            var temp = embedding.GenerateEmbeddingAsync(Services.IcService.GeraTextoEmbedding(_db, item.Id)).Result;
            item.Embedding = new Pgvector.Vector(temp);
            _db.SaveChanges();
        }
        return Ok(lista.Count);
    }


    [HttpGet("[action]")]
    [Authorize(Roles = "admin")]
    public IActionResult EmbeddingZera()
    {
        if (!embeddingHabilitado)
            return BadRequest(new MensagemErro("Embedding não esta habilitado"));

        var lista = _db.IcIc;
        foreach (var item in lista)
        {
            item.Embedding = null;
        }
        _db.SaveChanges();

        return Ok();
    }

    private void PopulaFilhosMenu(ref Model.IC.VwIc item, ref List<Model.IC.VwIc> lista)
    {
        int idPai = item.Id;
        var filho = lista
            .Where(p => p.IdPai == idPai)
            .OrderBy(p => p.Nivel).ThenBy(p => p.Nome)
            .FirstOrDefault();
        if (filho != null)
        {
            item.Filhos = new List<Model.IC.VwIc>();
            while (filho != null)
            {
                item.Filhos.Add(filho);
                lista.Remove(filho);
                PopulaFilhosMenu(ref filho, ref lista);
                filho = lista
                    .Where(p => p.IdPai == idPai)
                    .OrderBy(p => p.Nivel).ThenBy(p => p.Nome)
                    .FirstOrDefault();
            }
        }
    }

    private bool DetectaIcEditavel(int idIc, ClaimsPrincipal usuario)
    {
        if (usuario.IsInRole("admin"))
            return true;

        int IdLogado = Util.Claim2Usuario(usuario.Claims).Id;

        var localizado = _db.IcVwIc.AsNoTracking().Include(p => p.Responsavel).ThenInclude(p => p!.Equipe).FirstOrDefault(p => p.Id == idIc);
        if (localizado is null)
            throw new Exception("Ic não localizado");

        if (localizado.Responsavel is not null && localizado.Responsavel.Equipe is not null)
        {
            if (localizado.Responsavel.Equipe.Any(p => p.IdUsuario == IdLogado))
                return true;
        }


        return false;
    }

    public record PesquisaIC(string Chave, bool? Ativo, string? FilhoDe, int? Tipo);

}
