using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cmdb.Model.IC;
[Table("segredo", Schema = "ic")]
public record Segredo
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("idic")]
    public int IdIC { get; set; }

    [Column("conteudo")]
    public string Conteudo { get; set; }=string.Empty;

    [Column("idusuariodono")]
    public int? IdUsuarioDono { get; set; }

    [Column("idorganogramadono")]
    public int? IdOrganogramaDono { get; set; }

    [Column("algoritmo")]
    public string Algoritmo { get; set; } = "Rijndael";

    [ForeignKey("IdIC")]
    public Model.IC.VwIc? IC { get; set; }


    [ForeignKey("IdUsuarioDono")]
    public Seg.Usuario? UsuarioDono { get; set; }

    [ForeignKey("IdOrganogramaDono")]
    public Seg.VwOrganograma? OrganogramaDono { get; set; }

}
