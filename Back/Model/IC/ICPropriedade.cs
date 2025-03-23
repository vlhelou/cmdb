using System.ComponentModel.DataAnnotations;

namespace Cmdb.Model.IC;

public record ICPropriedade
{
    [Key]
    public string Nome { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
}
