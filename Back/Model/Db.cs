using Microsoft.EntityFrameworkCore;

namespace Cmdb.Model;

public class Db(DbContextOptions<Db> options) : DbContext(options)
{
 
    public static readonly ILoggerFactory MyLoggerFactory
        = LoggerFactory.Create(builder => { 
            builder.AddConsole(); 
        });
    
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
    }
    //seg
    public DbSet<Model.Seg.Usuario> SegUsuario { get; set; }
    public DbSet<Model.Seg.Organograma> SegOrganograma { get; set; }
    public DbSet<Model.Seg.VwOrganograma> SegVwOrganograma { get; set; }


    //corp
    public DbSet<Model.Corp.Tipo> CorpTipo { get; set; }

    //ic
    public DbSet<Model.IC.IC> IcIc { get; set; }
    public DbSet<Model.IC.ICPropriedade> IcPropriedade { get; set; }
    public DbSet<Model.IC.VwIc> IcVwIc { get; set; }


}