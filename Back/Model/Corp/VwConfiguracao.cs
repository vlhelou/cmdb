using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cmdb.Model.Corp;


[Table("vw_configuracao", Schema = "corp")]
public record VwConfiguracao
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

    [Column("valorboleano")]
    public bool? ValorBoleano { get; set; }


    [Column("valorsensivel")]
    public bool ValorSensivel { get; set; }

    [Column("nomecompleto")]
    public string NomeCompleto { get; set; } = string.Empty;

    [Column("listaancestrais")]
    public string ListaAncestrais { get; set; } = string.Empty;

    [Column("nivel")]
    public int Nivel { get; set; }

    [NotMapped]
    public List<VwConfiguracao>? Filhos { get; set; }


}
