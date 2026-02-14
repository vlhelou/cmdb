using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cmdb.Model.IC;

[Table("conhecimento", Schema = "ic")]
public record Conhecimento
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("idic")]
    public int IdIc { get; set; }

    [Column("problema")]
    public string Problema { get; set; } = string.Empty;

    [Column("solucao")]
    public string Solucao { get; set; } = string.Empty;

    [Column("idautor")]
    public int IdAutor { get; set; }

    [Column("dataalteracao")]
    public DateTimeOffset DataAlteracao { get; set; }

    [ForeignKey("IdIc")]
    public Model.IC.VwIc? IC { get; set; }

    [ForeignKey("IdAutor")]
    public Seg.Usuario? Autor { get; set; }

}
