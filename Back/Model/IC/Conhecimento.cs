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
    public int IdIC { get; set; }

    [Column("problema")]
    public string Problema { get; set; }=string.Empty;

    [Column("solucao")]
    public string Solucao { get; set; }=string.Empty;

    [Column("idusuario")]
    public int IdUsuario { get; set; }

    [Column("dataalteracao")]
    public DateTimeOffset DataAlteracao { get; set; }

    [ForeignKey("IdIC")]
    public Model.IC.VwIc? IC { get; set; }

    [ForeignKey("IdUsuario")]
    public Seg.Usuario? UsuarioDono { get; set; }


}
