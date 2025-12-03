using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace McpServer.DTOs.Corp;

public record Tipo
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("grupo")]
    public string Grupo { get; set; } = string.Empty;

    [Column("ativo")]
    public bool Ativo { get; set; }

}
