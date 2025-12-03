using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace McpServer.Tools;


[McpServerToolType]
internal static class ICTool
{
    [McpServerTool, Description("busca por ic, podento filtrar por nome, paternidade, tipo e se esta ativo")]
    public static async Task<string> PesquisarIC(Clients.IcClient client, [Description("nome, tipo, ativo e paternidade")] Clients.PesquisaIC pesquisa)
    {
        try
        {
            var retorno = await client.PesquisarIC(pesquisa);
            return retorno.Count == 0
                ? "Nenhum livro encontrado"
                : JsonSerializer.Serialize(retorno);

        }
        catch (Exception ex)
        {
            //Log
            return $"Erro ao buscar ICS: {ex.Message}";
        }
    }
}
