using Microsoft.EntityFrameworkCore;
using project_backend.Models;

namespace project_backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<CV> CVs { get; set; }
        public DbSet<Vacante> Vacantes { get; set; }
        public DbSet<InvalidToken> InvalidTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relación Usuario -> Rol
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol) // Un Usuario tiene un Rol
                .WithMany() // Un Rol puede tener muchos Usuarios
                .HasForeignKey(u => u.IdRol) // Foreignkey en Usuario
                .OnDelete(DeleteBehavior.Restrict); // Evita eliminación en cascada

            // Usuario -> CV (uno a uno)
            modelBuilder.Entity<CV>()
                .HasOne(c => c.Usuario) // Un CV tiene un Usuario
                .WithOne(u => u.CV) // Un Usuario tiene un CV
                .HasForeignKey<CV>(c => c.IdUsuario) // Foreignkey en CV
                .OnDelete(DeleteBehavior.Cascade); // Elimina el CV si se elimina el usuario

            // CV -> Vacante (muchos a uno)
            modelBuilder.Entity<CV>()
                .HasOne(c => c.Vacante) // Un CV una Vacante
                .WithMany(v => v.CVs) // Una Vacante muchos CVs
                .HasForeignKey(c => c.IdVacante) // Foreignkey en CV
                .OnDelete(DeleteBehavior.Cascade); // Elimina el CV si se elimina la vacante

            // Usuario -> VacanteUsuario (muchos a muchos)
            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.VacanteUsuarios)
                .WithOne(vu => vu.Usuario)
                .HasForeignKey(vu => vu.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            // Nombre del rol es único
            modelBuilder.Entity<Rol>()
                .HasIndex(r => r.Nombre)
                .IsUnique();

            // Correo del usuario como único
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Correo)
                .IsUnique();
        }
    }
}