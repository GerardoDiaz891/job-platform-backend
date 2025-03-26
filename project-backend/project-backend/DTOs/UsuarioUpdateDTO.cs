namespace project_backend.DTOs
{
    public class UsuarioUpdateDTO
    {
        public string Nombre { get; set; }
        public string Contraseña { get; set; }
        public string NombreEmpresa { get; set; }
        public string TipoEmpresa { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string SitioWeb { get; set; }
        public string DescripcionEmpresa { get; set; }
    }
}
