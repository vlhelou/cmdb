using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cmdb.Model.Seg;

[Table("vw_organograma", Schema = "seg")]
public class VwOrganograma
{

    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("idpai")]
    public int? IdPai { get; set; }

    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("ativo")]
    public bool Ativo { get; set; }

    [Column("ativofinal")]
    public bool AtivoFinal { get; set; }

    [Column("nomecompleto")]
    public string NomeCompleto { get; set; } = string.Empty;

    [Column("listaancestrais")]
    public string? ListaAncestrais { get; set; }

    [Column("nivel")]
    public int Nivel { get; set; }

    [InverseProperty("VwOrganograma")]
    public ICollection<Equipe>? Equipe { get; set; }

    public ICollection<int> LstAncestrais
    {
        get
        {
            List<int> retorno = new List<int>();
            if (ListaAncestrais != null)
            {
                if (ListaAncestrais.StartsWith(","))
                {
                    string strLista = ListaAncestrais.Remove(0, 1);
                    foreach (string numero in strLista.Split(','))
                    {
                        if (int.TryParse(numero, out int nr))
                        {
                            retorno.Add(nr);
                        }
                    }

                }
            }
            return retorno;
        }
    }

    [JsonIgnore]
    [NotMapped]
    [ForeignKey("IdPai")]
    public VwOrganograma? Pai { get; set; }

    //[InverseProperty("Organograma")]
    //public ICollection<Equipe>? Equipe { get; set; }

    [InverseProperty("OrganogramaDono")]
    public ICollection<IC.Segredo>? Segredos { get; set; }


    [NotMapped]
    public IList<VwOrganograma>? Ancestrais { get; set; }


    [NotMapped]
    public IList<VwOrganograma>? Filhos { get; set; }
}
