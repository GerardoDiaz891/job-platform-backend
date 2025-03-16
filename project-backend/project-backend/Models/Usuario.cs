namespace project_backend.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public int RolId { get; set; } // FK al Rol
        public Rol? Rol { get; set; } // Relación muchos a uno con Rol

        // Campos específicos para usuarios empresariales
        public string? NombreEmpresa { get; set; }
        public string? TipoEmpresa { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? SitioWeb { get; set; }
        public string? DescripcionEmpresa { get; set; }
        public ICollection<Vacante> Vacantes { get; set; } = new List<Vacante>();


        // Relación uno a uno con CV (solo para usuarios postulantes)
        public CV? CV { get; set; }
    }
}