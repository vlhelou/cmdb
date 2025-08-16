using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cmdb.Model.Seg;

[Table("organograma", Schema = "seg")]
public record Organograma
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("idpai")]
    public int? IdPai { get; set; }
    
    [Column("nome")]
    public string Nome { get; set; }=string.Empty;

    [Column("ativo")]
    public bool Ativo { get; set; }

    [Column("gd")]
    [JsonIgnore]
    public Guid GD { get; set; }

    [ForeignKey("IdPai")]
    public Organograma? Pai { get; set; }

    [InverseProperty("Organograma")]
    public ICollection<Equipe>? Equipe { get; set; }


}
