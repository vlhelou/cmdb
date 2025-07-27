using System.Drawing;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Cmdb;

public static class Util
{
    public static string ToSha512(this string origem)
    {
        using var sha512Hash = SHA512.Create();
        var bytes = SHA512.HashData(Encoding.UTF8.GetBytes(origem));
        // var bytes = sha512Hash.ComputeHash(Encoding.UTF8.GetBytes(origem));
        var retorno = new StringBuilder();

        foreach (var t in bytes)
            retorno.Append(t.ToString("x2"));

        return retorno.ToString();
    }

    public static Model.Seg.Usuario Claim2Usuario(IEnumerable<Claim> usrAtuntenciacao)
    {
        if (usrAtuntenciacao == null)
            throw new Exception("sem parâmetro");
        Model.Seg.Usuario retorno = new();

        if (int.TryParse(usrAtuntenciacao.FirstOrDefault(p => p.Type == "id")?.Value ?? string.Empty, out int _id))
            retorno.Id = _id;

        retorno.Nome = usrAtuntenciacao.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value ?? string.Empty;
        retorno.Email = usrAtuntenciacao.FirstOrDefault(p => p.Type == ClaimTypes.Email)?.Value ?? string.Empty;

        string role = usrAtuntenciacao.FirstOrDefault(p => p.Type == ClaimTypes.Role)?.Value ?? string.Empty;
        retorno.Administrador = role == "admin";

        return retorno;
    }

}


public record MensagemErro(string mensagem);

