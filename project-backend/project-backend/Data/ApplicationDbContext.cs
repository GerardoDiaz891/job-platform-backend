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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la relación Usuario -> Rol
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol) // Un Usuario tiene un Rol
                .WithMany() // Un Rol puede tener muchos Usuarios (pero no se expone en el modelo Rol)
                .HasForeignKey(u => u.RolId) // Clave foránea en Usuario
                .OnDelete(DeleteBehavior.Restrict); // Evita la eliminación en cascada

            // Configuración de la relación Usuario -> CV (uno a uno)
            modelBuilder.Entity<CV>()
                .HasOne(c => c.Usuario) // Un CV tiene un Usuario
                .WithOne(u => u.CV) // Un Usuario tiene un CV
                .HasForeignKey<CV>(c => c.UsuarioId) // Clave foránea en CV
                .OnDelete(DeleteBehavior.Cascade); // Elimina el CV si se elimina el usuario

            // Configuración de la relación Usuario -> Vacante (uno a muchos)
            modelBuilder.Entity<Vacante>()
                .HasOne(v => v.Usuario) // Una Vacante tiene un Usuario
                .WithMany(u => u.Vacantes) // Un Usuario puede tener muchas Vacantes
                .HasForeignKey(v => v.UsuarioId) // Clave foránea en Vacante
                .OnDelete(DeleteBehavior.Cascade); // Elimina las vacantes si se elimina el usuario

            // Configurar el nombre del rol como único
            modelBuilder.Entity<Rol>()
                .HasIndex(r => r.Nombre)
                .IsUnique();

            // Configurar el correo del usuario como único
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Correo)
                .IsUnique();
        }
    }
}