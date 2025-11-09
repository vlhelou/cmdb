using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cmdb.Model.IC;

[Table("ic", Schema = "ic")]
public record IC
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("idpai")]
    public int? IdPai { get; set; }

    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("ativo")]
    public bool Ativo { get; set; }

    [Column("propriedades", TypeName = "jsonb")]
    public List<ICPropriedade>? Propriedades { get; set; }

    [Column("idtipo")]
    public int IdTipo { get; set; }

    [Column("idorganograma")]
    public int? IdOrganograma { get; set; }



    [ForeignKey("IdPai")]
    public IC? Pai { get; set; }

    [ForeignKey("IdTipo")]
    public Model.Corp.Tipo? Tipo { get; set; }

    [ForeignKey("IdOrganograma")]
    public Seg.Organograma? Responsavel { get; set; }


    public void Altera(IC item)
    {
        this.Nome = item.Nome;
        this.Ativo = item.Ativo;
        this.IdTipo = item.IdTipo;
        this.Propriedades = item.Propriedades;

    }

    //[NotMapped]
    //public IList<ICPropriedade>? ListaPropriedades
    //{
    //    get
    //    {
    //        if (string.IsNullOrEmpty(this.Propriedades))
    //            return null;
    //        IList<ICPropriedade>? retorno = null;
    //        try
    //        {
    //            retorno = JsonSerializer.Deserialize<IList<ICPropriedade>>(Propriedades);
    //        }
    //        catch (JsonException)
    //        {
    //            retorno = null;
    //        }
    //        return retorno;
    //    }
    //    set {
    //        if (value == null || value.Count == 0)
    //        {
    //            this.Propriedades = null;
    //        }
    //        else
    //        {
    //            this.Propriedades = JsonSerializer.Serialize(value);
    //        }
    //    }

    //}

}
