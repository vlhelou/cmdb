using Microsoft.AspNetCore.Authorization;
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
        var nomes = string.Join(" & ", prm.Chave.Split(' ').Where(p=> p.Trim().Length>0));
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


    [HttpPost("[action]")]
    [Authorize(Roles = "admin")]
    public IActionResult Grava([FromBody] Model.Seg.Organograma item)
    {
        if (item == null)
            return BadRequest(new MensagemErro("Parâmetro não informado"));

        int IdLogado = Util.Claim2Usuario(HttpContext.User.Claims).Id;


        if (item.IdPai is null)
            return BadRequest(new MensagemErro("O Item Raiz não pode ser alterado"));

        var Logado = _db.SegUsuario.Where(p => p.Id == IdLogado).AsNoTracking().FirstOrDefault();

        if (Logado is null)
            return BadRequest(new MensagemErro("Usuário não localizado"));

        if (!Logado.Ativo)
            return BadRequest(new MensagemErro("Usuário inativo"));



        //List<int> MinhasLocacoes = Logado.Locacoes.Select(p => p.IdOrganograma).ToList();
        //caso seja novo

        if (item.Id == 0)
        {
            if (item.IdPai == null)
                return BadRequest(new MensagemErro("Sem referencia de Pai"));

            _db.Entry(item).State = EntityState.Added;
            _db.SaveChanges();
            return Ok(item);
        }
        else
        {
            var localizado = _db.SegOrganograma.Where(p => p.Id == item.Id).FirstOrDefault();
            if (localizado == null)
                return BadRequest(new MensagemErro("IC não localizado"));
            localizado.Altera(item);
            _db.SaveChanges();
            return Ok(localizado);
        }
    }


    [HttpGet("[action]/{idIc}/{idNovoPai}")]
    [Authorize(Roles = "admin")]
    public IActionResult MudaPaternidade(int idIc, int idNovoPai)
    {
        var org = _db.SegOrganograma.Where(p => p.Id == idIc).FirstOrDefault();
        var novoPai = _db.SegOrganograma.Where(p => p.Id == idNovoPai).AsNoTracking().FirstOrDefault();
        if (org is null || novoPai is null)
            return BadRequest(new MensagemErro("IC ou Novo pai não localizado"));
        org.IdPai = idNovoPai;
        _db.SaveChanges();
        return Ok();
    }


    public record PesquisaOrg(string Chave, bool? Ativo);

}
