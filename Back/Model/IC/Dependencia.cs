using System.ComponentModel.DataAnnotations.Schema;

namespace Cmdb.Model.IC;

public record Dependencia
{
    public int Id { get; set; }
    public int IdPrincipal { get; set; }
    public int IdIcDependente { get; set; }

    public string? Observacao { get; set; } 
    public int IdAutor { get; set; }
    public DateTimeOffset DataAlteracao { get; set; } = DateTimeOffset.Now.ToUniversalTime();


    [ForeignKey("IdPrincipal")]
    public virtual IC? IcPrincipal { get; set; }


    [ForeignKey("IdIcDependente")]
    public virtual IC? IcDependente { get; set; }
}
