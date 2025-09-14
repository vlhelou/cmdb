using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cmdb.Model.Seg;

[Table("usuario", Schema = "seg")]
public record Usuario
{

    [Key]
    [Column("id")]
    public Int32 Id { get; set; }

    [Column("identificacao")]
    public string Identificacao { get; set; } = string.Empty;

    [Column("gd")]
    [JsonIgnore]
    public Guid Gd { get; set; }

    [Column("senha")]
    [JsonIgnore]
    public string? Senha { get; set; }


    [Column("administrador")]
    public bool Administrador { get; set; }

    [Column("ativo")]
    public bool Ativo { get; set; }


    [Column("local")]
    public bool Local { get; set; }


    [Column("email")]
    public string Email { get; set; } = string.Empty;

    public void AjustaSenha(string senha)
    {
        Senha = (this.Id.ToString() + senha).ToSha512();
    }


    [InverseProperty("Usuario")]
    public List<Equipe>? Locacoes { get; set; }

    [InverseProperty("UsuarioDono")]
    public List<IC.Segredo>? Segredos { get; set; }

}