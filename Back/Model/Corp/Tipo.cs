using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cmdb.Model.Corp;

[Table("tipo", Schema = "corp")]
public record Tipo
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("grupo")]
    public string Grupo { get; set; } = string.Empty;

    [Column("ativo")]
    public bool Ativo { get; set; }

}
