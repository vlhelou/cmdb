using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text;

namespace McpServer.DTOs.IC;

public record IC
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

    [Column("propriedades", TypeName = "jsonb")]
    public List<ICPropriedade>? Propriedades { get; set; }

    [Column("idtipo")]
    public int IdTipo { get; set; }

    [Column("idorganograma")]
    public int? IdOrganograma { get; set; }



    [ForeignKey("IdPai")]
    public IC? Pai { get; set; }

    [ForeignKey("IdTipo")]
    public DTOs.Corp.Tipo? Tipo { get; set; }

    [ForeignKey("IdOrganograma")]
    public Seg.Organograma? Responsavel { get; set; }


    public void Altera(IC item)
    {
        this.Nome = item.Nome;
        this.Ativo = item.Ativo;
        this.IdTipo = item.IdTipo;
        this.Propriedades = item.Propriedades;

    }


}
