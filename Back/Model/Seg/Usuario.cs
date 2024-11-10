using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cmdb.Model.Seg;

[Table("usuario", Schema = "seg")]
public record Usuario
{

    [Key]
    [Column("id")]
    public int Id { get; set; }
        
    [Column("nome")]
    public string Nome { get; set; }=string.Empty;
        
    [Column("email")]
    public string Email { get; set; }=string.Empty;
        
    [Column("administrador")]
    public bool Administrador { get; set; }

    [Column("ativo")]
    public bool Ativo { get; set; }

    [Column("gd")]
    [JsonIgnore]
    public Guid? Gd { get; set; }
    
    [Column("senha")]
    [JsonIgnore]
    public string Senha { get; set; }=string.Empty;

    public void NovoGD()
    {
        Gd = Guid.NewGuid();
    }

    public void AjustaSenha(string senha)
    {
        Senha = (this.Id.ToString()+senha).ToSha512();
    }
    

    // [JsonIgnore]
    // [InverseProperty("Usuario")]
    // public List<Equipe> Locacoes { get; set; }
    //
    // [JsonIgnore]
    // [InverseProperty("UsuarioDono")]
    // public List<IC.Segredo> Segredos { get; set; }

}