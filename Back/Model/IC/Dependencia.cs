using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cmdb.Model.IC;

[Table("dependencia", Schema = "ic")]
public record Dependencia
{

    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("idicprincipal")]
    public int IdIcPrincipal { get; set; }

    [Column("idicdependente")]
    public int IdIcDependente { get; set; }

    [Column("observacao")]
    public string? Observacao { get; set; }

    [Column("idautor")]
    public int IdAutor { get; set; }

    [Column("dataalteracao")]
    public DateTimeOffset DataAlteracao { get; set; } = DateTimeOffset.Now.ToUniversalTime();

    [ForeignKey("IdIcPrincipal")]
    public virtual VwIc? IcPrincipal { get; set; }


    [ForeignKey("IdIcDependente")]
    public virtual VwIc? IcDependente { get; set; }
}
