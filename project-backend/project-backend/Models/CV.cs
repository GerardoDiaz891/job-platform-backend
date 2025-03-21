using System.ComponentModel.DataAnnotations.Schema;

namespace project_backend.Models
{
    public class CV
    {
        public int Id { get; set; }
        public string RutaArchivo { get; set; } // Ruta donde se almacena el archivo
        public DateTime FechaSubida { get; set; } = DateTime.UtcNow; // Inicializar con la fecha actual

        [ForeignKey(nameof(Usuario))]
        public int IdUsuario { get; set; } // FK al Usuario
        public virtual Usuario Usuario { get; set; } // Relación uno a uno con Usuario

        [ForeignKey(nameof(Vacante))]
        public int IdVacante { get; set; } // FK a la Vacante
        public virtual Vacante Vacante { get; set; } // Relación muchos a uno con Vacante
    }
}