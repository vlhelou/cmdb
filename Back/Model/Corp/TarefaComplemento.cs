using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cmdb.Model.Corp;


[Table("tarefa", Schema = "corp")]
public record TarefaComplemento
{
    [Key]
    [Column("id")]
    public int Id { get; set; }


    [Column("idtarefa")]
    public int IdTarefa { get; set; }


    [Column("idautor")]
    public int IdAutor { get; set; }


    [Column("datacriacao")]
    public DateTimeOffset DataCriacao{ get; set; }

    [Column("complemento")]
    public string Complemento { get; set; } = string.Empty;


    [ForeignKey("IdTarefa")]
    public Tarefa Tarefa { get; set; } = null!;

    [ForeignKey("IdAutor")]
    public Model.Seg.Usuario Autor { get; set; } = null!;

}
