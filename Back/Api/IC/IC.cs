using Cmdb.Model;
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


        //List<int> MinhasLocacoes = Logado.Locacoes.Select(p => p.IdOrganograma).ToList();
        //caso seja novo

        if (item.Id == 0)
        {
            if (item.IdPai == null)
                return BadRequest(new MensagemErro("Sem referencia de Pai"));

            _db.Entry(item).State = EntityState.Added;
            _db.SaveChanges();
            return Ok(item);
        } else
        {
            var localizado = _db.IcIc.Where(p => p.Id == item.Id).FirstOrDefault();
            if (localizado == null)
                return BadRequest(new MensagemErro("IC não localizado"));
            localizado.Altera(item);
            _db.SaveChanges();
            return Ok(localizado);
        }
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

    public record PesquisaIC(string Chave, bool? Ativo, string? FilhoDe, int? Tipo);

}
