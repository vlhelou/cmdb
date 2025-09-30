using System.Drawing;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

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

        retorno.Identificacao= usrAtuntenciacao.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value ?? string.Empty;
        retorno.Email = usrAtuntenciacao.FirstOrDefault(p => p.Type == ClaimTypes.Email)?.Value ?? string.Empty;

        string role = usrAtuntenciacao.FirstOrDefault(p => p.Type == ClaimTypes.Role)?.Value ?? string.Empty;
        retorno.Administrador = role == "admin";

        return retorno;
    }


    public static string Criptografa(string origem, Guid chave)
    {

        byte[] encrypted;
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = chave.ToByteArray();
            aesAlg.IV = chave.ToByteArray().Reverse().ToArray();
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        swEncrypt.Write(origem);
                    }
                    encrypted = msEncrypt.ToArray();
                    return Convert.ToBase64String(encrypted);
                }
            }

        }


    }

    public static string Descriptografa(string hash, Guid chave, string algoritmo)
    {
        byte[] encrypted = Convert.FromBase64String(hash);
        switch (algoritmo)
        {
            case "AES":
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = chave.ToByteArray();
                    aesAlg.IV = chave.ToByteArray().Reverse().ToArray();
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using MemoryStream msDecrypt = new MemoryStream(encrypted);
                    using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                    using StreamReader srDecrypt = new StreamReader(csDecrypt);
                    return srDecrypt.ReadToEnd();
                }
            default:
                throw new Exception("Algoritmo desconhecido");
        }

    }

    public static string ToSHA512(this String origem)
    {
        using (SHA512 sha512Hash = SHA512.Create())
        {
            var bytes = sha512Hash.ComputeHash(Encoding.UTF8.GetBytes(origem));
            StringBuilder retorno = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                retorno.Append(bytes[i].ToString("x2"));
            }
            return retorno.ToString();
        }
    }


    public static Guid ChaveCriptografia(Model.Db db)
    {
        string? strChave = db.CorpConfiguracao.AsNoTracking().FirstOrDefault(p => p.Id == 2)?.ValorTexto;
        if (string.IsNullOrEmpty(strChave))
            throw new Exception("chave não localizada");

        if (Guid.TryParse(strChave, out Guid retorno))
            return retorno;

        throw new Exception("falha ao converter a chave");
    }
}


public record MensagemErro(string mensagem);

