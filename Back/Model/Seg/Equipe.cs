using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cmdb.Model.Seg;

[Table("equipe", Schema = "seg")]
public class Equipe
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("idusuario")]
    public int IdUsuario { get; set; }

    [Column("idorganograma")]
    public int IdOrganograma { get; set; }

    [Column("idautor")]
    public int IdAutor { get; set; }

    [Column("data")]
    public DateTimeOffset Data { get; set; }

    [ForeignKey("IdUsuario")]
    public Usuario? Usuario { get; set; }

    [ForeignKey("IdOrganograma")]
    public Organograma? Organograma { get; set; }

    [ForeignKey("IdOrganograma")]
    public VwOrganograma? VwOrganograma { get; set; }

    [ForeignKey("IdAutor")]
    public Usuario? Autor { get; set; }
}
