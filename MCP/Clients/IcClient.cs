using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace McpServer.Clients;

internal class IcClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    public IcClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
    }

    public async Task<List<DTOs.IC.IC>> PesquisarIC(PesquisaIC pesquisa)
    {
        var jsonContent = JsonSerializer.Serialize(pesquisa, _jsonOptions);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("Api/IC/IC/Pesquisa", content);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var ics = JsonSerializer.Deserialize<List<DTOs.IC.IC>>(responseContent, _jsonOptions);
        return ics ?? new List<DTOs.IC.IC>();
    }

    
}

public record PesquisaIC(string Chave, bool? Ativo, string? FilhoDe, int? Tipo);