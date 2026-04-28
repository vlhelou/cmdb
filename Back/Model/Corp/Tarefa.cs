using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cmdb.Model.Corp;

[Table("tarefa", Schema = "corp")]
public record Tarefa
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("idautor")]
    public int IdAutor { get; set; }

    [Column("idexecutor")]
    public int IdExecutor { get; set; }

    [Column("tarefatitulo")]
    public string TarefaTitulo { get; set; }= string.Empty;

    [Column("tarefadescricao")]
    public string? TarefaDescricao { get; set; } 


    [Column("datacriacao")]
    public DateTimeOffset DataCriacao { get; set; }

    [Column("dataconclusao")]
    public DateTimeOffset DataConclusao { get; set; }

    [ForeignKey("IdAutor")]
    public Model.Seg.Usuario Autor { get; set; } = null!;

    [ForeignKey("IdExecutor")]
    public Model.Seg.Usuario Executor { get; set; } = null!;

    [InverseProperty("Tarefa")]
    public ICollection<TarefaComplemento>? Complementos { get; set; } 

}
