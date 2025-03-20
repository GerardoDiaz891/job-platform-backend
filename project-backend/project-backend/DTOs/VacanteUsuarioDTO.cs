namespace project_backend.DTOs
{
    public class VacanteUsuarioDTO
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; } // FK al Usuario
        public int IdVacante { get; set; } // FK a la Vacante
    }
}