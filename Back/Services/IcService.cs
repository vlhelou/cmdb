

using Cmdb.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel.Embeddings;
using OllamaSharp;
using System.Text;
using System.Text.Json;

namespace Cmdb.Services;

public static class IcService
{
    public static Model.IC.VwIc LocalizaComFamilia(Model.Db db, int id)
    {

        var localizado = db.IcVwIc
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Include(p => p.Tipo)
            .Include(p => p.Responsavel)
            .Include(p => p.Conhecimentos)
            .FirstOrDefault();
        if (localizado == null)
            throw new Exception("IC não localizado");
        localizado.Ancestrais = db.IcVwIc.Where(p => localizado.LstAncestrais.AsEnumerable().Contains(p.Id))
            .AsNoTracking()
            .Include(p => p.Tipo)
            .OrderBy(p => p.Nivel)
            .ToList<Model.IC.VwIc>();
        localizado.Filhos = db.IcVwIc
            .AsNoTracking()
            .Where(p => p.IdPai == localizado.Id)
            .Include(p => p.Tipo)
            .OrderBy(p => p.Tipo!.Nome)
            .ThenBy(p => p.Nome).ToList();
        if (localizado.Ancestrais.Count > 0)
        {
            localizado.Pai = localizado.Ancestrais.Last();
        }
        return localizado;

    }

    public static string GeraTextoEmbedding(Model.Db db, int id)
    {
        var ic = Services.IcService.LocalizaComFamilia(db, id);
        StringBuilder saida = new();
        if (ic.Ancestrais is not null && ic.Ancestrais.Count() > 0)
            saida.AppendLine($"Ancestrais: ({string.Join(", ", ic.Ancestrais.Select(a => $"nome: {a.Nome} tipo:{a.Tipo!.Nome}"))})");

        if (ic.Filhos is not null && ic.Filhos.Count() > 0)
            saida.AppendLine($"Filhos: ({string.Join(", ", ic.Filhos.Select(a => $"nome: {a.Nome} tipo:{a.Tipo!.Nome}"))})");

        if (ic.Conhecimentos is not null && ic.Conhecimentos.Count() > 0)
        {

            saida.AppendLine($"Conhecimentos: ");
            foreach (var conhecimento in ic.Conhecimentos)
            {
                saida.AppendLine($"- problema: {conhecimento.Problema} solução: {conhecimento.Solucao}");
            }
        }

        saida.AppendLine($"nome: {ic.Nome} tipo:{ic.Tipo!.Nome}");
        saida.AppendLine($"nome completo: {ic.NomeCompleto} ");
        if (!string.IsNullOrEmpty(ic.Observacao))
            saida.AppendLine($"observacao: {ic.Observacao}");
        return saida.ToString();

    }

    public static void AtualizaEmbedding(Model.Db db, OllamaApiClient service, int id)
    {
        var item = db.IcIc.FirstOrDefault(p => p.Id == id);
        if (item == null)
            throw new Exception("IC não localizado");

        string origem = GeraTextoEmbedding(db, id);
        var embedding = service.AsTextEmbeddingGenerationService();
        var temp = embedding.GenerateEmbeddingAsync(JsonSerializer.Serialize(origem)).Result;
        item.Embedding = new Pgvector.Vector(temp);
        db.SaveChanges();


    }


}
