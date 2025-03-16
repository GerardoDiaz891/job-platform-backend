namespace project_backend.Models
{
    public class CV
    {
        public int Id { get; set; }
        public string RutaArchivo { get; set; } // Ruta donde se almacena el archivo
        public DateTime FechaSubida { get; set; } = DateTime.UtcNow; // Inicializar con la fecha actual
        public int UsuarioId { get; set; } // FK al Usuario
        public Usuario Usuario { get; set; } // Relación uno a uno con Usuario
    }
}