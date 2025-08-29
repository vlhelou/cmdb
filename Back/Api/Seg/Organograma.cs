using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Cmdb.Api.IC.IC;

namespace Cmdb.Api.Seg;

[Route("api/seg/[controller]")]
[ApiController]
public class Organograma : ControllerBase
{
    private readonly Model.Db _db;
    public Organograma(Model.Db db)
    {
        _db = db;
    }

    [HttpPost("[action]")]
    public IActionResult Pesquisa([FromBody] PesquisaOrg prm)
    {
        var nomes = string.Join(" & ", prm.Chave.Split(' '));



        var consulta = _db.SegVwOrganograma.FromSql($"""
            select 
            	id
            	, idpai
            	, nome
            	, ativo
            	, ativofinal
            	, nomecompleto
            	, listaancestrais
            	, nivel
                , ts_rank(pesquisats , to_tsquery('portuguese',{nomes})) rank
            from seg.vw_organograma 
            where pesquisats @@ to_tsquery('portuguese',{nomes})
            order by rank desc
            """)
            .AsNoTracking()
            .AsQueryable();


        if (prm.Ativo != null)
            consulta = consulta.Where(p => p.AtivoFinal == prm.Ativo);

        var retorno = consulta.ToList();
        return Ok(retorno);

    }

    public record PesquisaOrg(string Chave, bool? Ativo);

}
