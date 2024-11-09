using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cmdb.Model.Seg;

[Table("usuario", Schema = "seg")]
public record Usuario
{

    [Key]
    [Column("id")]
    public int Id { get; private set; }
        
    [Column("nome")]
    public string Nome { get; private set; }=string.Empty;
        
    [Column("email")]
    public string Email { get; private set; }=string.Empty;
        
    [Column("administrador")]
    public bool Administrador { get; private set; }

    [Column("ativo")]
    public bool Ativo { get; private set; }

    [Column("gd")]
    [JsonIgnore]
    public Guid? Gd { get; private set; }
    
    [Column("senha")]
    [JsonIgnore]
    public string Senha { get; private set; }=string.Empty;

    

    // [JsonIgnore]
    // [InverseProperty("Usuario")]
    // public List<Equipe> Locacoes { get; set; }
    //
    // [JsonIgnore]
    // [InverseProperty("UsuarioDono")]
    // public List<IC.Segredo> Segredos { get; set; }

}