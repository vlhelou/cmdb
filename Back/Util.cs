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
}