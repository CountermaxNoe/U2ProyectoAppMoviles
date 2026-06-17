using AlertaCiudadanaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AlertaCiudadanaAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Incidente> Incidentes { get; set; }
    public DbSet<FotoIncidente> FotosIncidente { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Correo).IsUnique();
            entity.Property(u => u.Correo).IsRequired().HasMaxLength(150);
            entity.Property(u => u.NombreReal).IsRequired().HasMaxLength(100);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.Rol).IsRequired().HasMaxLength(20).HasDefaultValue("usuario");
        });

        modelBuilder.Entity<Incidente>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Titulo).IsRequired().HasMaxLength(80);
            entity.Property(i => i.Categoria).IsRequired().HasMaxLength(50);
            entity.Property(i => i.Estado).IsRequired().HasMaxLength(20).HasDefaultValue("Pendiente");
            entity.HasOne(i => i.Usuario)
                  .WithMany(u => u.Incidentes)
                  .HasForeignKey(i => i.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FotoIncidente>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.Property(f => f.NombreArchivo).IsRequired().HasMaxLength(100);
            entity.HasOne(f => f.Incidente)
                  .WithMany(i => i.Fotos)
                  .HasForeignKey(f => f.IncidenteId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
