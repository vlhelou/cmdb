using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace McpServer.DTOs.IC;

public record ICPropriedade
{
    public string Nome { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
}
