using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
    public IList<ICPropriedade>? Propriedades { get; set; }

    [Column("idtipo")]
    public int IdTipo { get; set; }

    [Column("idorganograma")]
    public int? IdOrganograma { get; set; }

    [ForeignKey("IdPai")]
    public IC Pai { get; set; }

    [ForeignKey("IdTipo")]
    public Model.Corp.Tipo? Tipo { get; set; }

    [ForeignKey("IdOrganograma")]
    public Seg.Organograma? Responsavel { get; set; }

}
