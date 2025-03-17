using System.ComponentModel.DataAnnotations.Schema;

namespace project_backend.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        
        [ForeignKey(nameof(Rol))]
        public int IdRol { get; set; } // FK al Rol
        public virtual Rol? Rol { get; set; } // Relación muchos a uno con Rol

        // Campos específicos para usuarios empresariales
        public string? NombreEmpresa { get; set; }
        public string? TipoEmpresa { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? SitioWeb { get; set; }
        public string? DescripcionEmpresa { get; set; }
        

        // Relación uno a uno con CV (solo para usuarios postulantes)
        
        [ForeignKey(nameof(CV))]
        
        public int? IdCV { get; set; }
        public virtual CV? CV { get; set; }

        public virtual ICollection<VacanteUsuario> VacanteUsuarios { get; set; } = new List<VacanteUsuario>();
    }
}