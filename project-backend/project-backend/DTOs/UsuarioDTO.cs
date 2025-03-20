namespace project_backend.DTOs
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; } // Solo para registro
        public int IdRol { get; set; }
        public string NombreRol { get; set; } // Nombre del Rol
        public string NombreEmpresa { get; set; }
        public string TipoEmpresa { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string SitioWeb { get; set; }
        public string DescripcionEmpresa { get; set; }
        public int? IdCV { get; set; }
    }
}