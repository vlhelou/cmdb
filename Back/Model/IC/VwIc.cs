using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cmdb.Model.IC;

[Table("vw_ic", Schema = "ic")]
public record VwIc
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

    [Column("propriedades", TypeName = "jsonb")]
    public IList<ICPropriedade>? Propriedades { get; set; }

    [Column("nomecompleto")]
    public string NomeCompleto { get; set; } = string.Empty;

    [Column("listaancestrais")]
    public string ListaAncestrais { get; set; } = string.Empty;

    [Column("nivel")]
    public int Nivel { get; set; }

    public ICollection<int> LstAncestrais
    {
        get
        {
            List<int> retorno = new List<int>();
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
            return retorno;
        }
    }

    [Column("idtipo")]
    public int IdTipo { get; set; }

    [Column("idorganograma")]
    public int IdOrganograma { get; set; }

    [ForeignKey("IdPai")]
    public VwIc? Pai { get; set; }

    [ForeignKey("IdTipo")]
    public Corp.Tipo? Tipo { get; set; }

    [NotMapped]
    public IList<VwIc>? Ancestrais { get; set; }

    [NotMapped]
    public IList<VwIc>? Filhos { get; set; }

    //[InverseProperty("IC")]
    //public virtual ICollection<Conhecimento> Conhecimentos { get; set; }


    //[InverseProperty("IC")]
    //public virtual ICollection<Segredo> Segredos { get; set; }


    //[InverseProperty("ICDependente")]
    //public virtual ICollection<Dependencia> Dependencias { get; set; }

    //[InverseProperty("ICPrincipal")]
    //public virtual ICollection<Dependencia> Dependentes { get; set; }

    //[InverseProperty("IC")]
    //public virtual ICollection<IncidenteIC> Incidentes { get; set; }

    [ForeignKey("IdOrganograma")]
    public Seg.Organograma? Responsavel { get; set; }


}
