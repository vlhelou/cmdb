using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cmdb.Model.Corp;

[Table("configuracao", Schema = "corp")]
public record Configuracao
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("idpai")]
    public int? IdPai { get; set; }

    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("tipovalor")]
    public string TipoValor { get; set; } = string.Empty;

    [Column("valornumerico")]
    public decimal? ValorNumerico { get; set; }

    [Column("valortexto")]
    public string? ValorTexto { get; set; }


    [Column("valordata")]
    public DateTimeOffset? ValorData { get; set; }

    [Column("valorcomplexo", TypeName = "json")]
    public string? ValorComplexo { get; set; }

    [Column("valorsensivel")]
    public bool ValorSensivel { get; set; }

    [Column("valorboleano")]
    public bool? ValorBoleano { get; set; }




}
