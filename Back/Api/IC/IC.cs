using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        var Localizado = _db.IcVwIc
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Include(p => p.Tipo)
            .Include(p => p.Responsavel)
            .FirstOrDefault();
        if (Localizado == null)
            throw new Exception("IC não localizado");
        Localizado.Ancestrais = _db.IcVwIc.Where(p => Localizado.LstAncestrais.AsEnumerable().Contains(p.Id))
            .AsNoTracking()
            .Include(p => p.Tipo)
            .OrderBy(p => p.Nivel)
            .ToList<Model.IC.VwIc>();
        Localizado.Filhos = _db.IcVwIc.AsNoTracking()
            .Where(p => p.IdPai == Localizado.Id).Include(p => p.Tipo).ToList();
        if (Localizado.Ancestrais.Count > 0)
        {
            Localizado.Pai = Localizado.Ancestrais.Last();
        }

        foreach (var filho in Localizado.Filhos)
            filho.Pai = null;

        foreach (var pai in Localizado.Ancestrais)
            pai.Filhos = null;
        Localizado.Filhos = Localizado.Filhos.OrderBy(p => p.Tipo!.Nome).ThenBy(p => p.Nome).ToList();
        return Ok(Localizado);

    }


    [HttpPost("[action]")]
    public IActionResult Pesquisa([FromBody] PesquisaIC prm)
    {
        var nomes = string.Join(" or ", prm.Chave.Split(' '));

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
            	, ts_rank(pesquisats , websearch_to_tsquery({nomes})) rank
            from ic.vw_ic
            where pesquisats @@ websearch_to_tsquery({nomes})
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

        //int ct = 0;
        //System.Text.StringBuilder where = new System.Text.StringBuilder();
        //List<object> valores = new List<object>();
        //if (prm.TryGetProperty("Chave", out JsonElement chave) && chave.ValueKind == JsonValueKind.String)
        //{
        //    where.AppendFormat(" and Nome.ToLower().Contains(@{0}) ", ct);
        //    valores.Add(chave.GetString()!.ToLower());
        //    ct++;
        //}
        //if (prm.TryGetProperty("Ativo", out JsonElement ativo) && ativo.ValueKind != JsonValueKind.Null)
        //{
        //    where.AppendFormat(" and AtivoFinal==@{0}", ct);
        //    valores.Add(prm.GetBoolean());
        //    ct++;
        //}
        //if (prm.TryGetProperty("FilhoDe", out JsonElement FilhoDe) && FilhoDe.ValueKind == JsonValueKind.String)
        //{
        //    where.AppendFormat(" and NomeCompleto.ToLower().StartsWith(@{0})", ct);
        //    valores.Add(FilhoDe.GetString()!.ToLower() + "|");
        //    ct++;
        //}

        //if (prm.TryGetProperty("Tipo", out JsonElement tipo) && tipo.ValueKind == JsonValueKind.Number)
        //{
        //    where.AppendFormat(" and IdTipo=@{0}", ct);
        //    valores.Add(tipo.GetInt32());
        //    ct++;
        //}


        //if (where.ToString().StartsWith(" and"))
        //{
        //    where.Remove(0, 4);
        //}
        //var q = _db.IcVwIc
        //    .Where(where.ToString(), valores.ToArray()).Include(p => p.Tipo)
        //    .ToList();
    }

    public record PesquisaIC(string Chave, bool? Ativo, string? FilhoDe, int? Tipo);

}
