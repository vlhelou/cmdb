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


    [HttpGet("[action]")]
    public IActionResult ListaCompleta()
    {
        List<Model.IC.VwIc> final = new();
        var lista = _db.SegVwOrganograma.AsNoTracking().OrderBy(p => p.Nivel).ThenBy(p => p.Nome).ToList();
        var retorno = lista.Where(p => p.IdPai == null).FirstOrDefault();
        if (retorno == null)
        {
            return BadRequest(new MensagemErro("Não há elemento root"));
        }
        PopulaFilhosMenu(ref retorno, ref lista);
        return Ok(retorno);
    }

    private void PopulaFilhosMenu(ref Model.Seg.VwOrganograma item, ref List<Model.Seg.VwOrganograma> lista)
    {
        int idPai = item.Id;
        var filho = lista
            .Where(p => p.IdPai == idPai)
            .OrderBy(p => p.Nivel).ThenBy(p => p.Nome)
            .FirstOrDefault();
        if (filho != null)
        {
            item.Filhos = new List<Model.Seg.VwOrganograma>();
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

    public record PesquisaOrg(string Chave, bool? Ativo);

}
