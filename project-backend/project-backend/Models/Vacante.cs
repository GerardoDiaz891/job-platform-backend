using System.ComponentModel.DataAnnotations.Schema;

namespace project_backend.Models
{
    public class Vacante
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Salario { get; set; }
        public string Horario { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public DateTime FechaExpiracion { get; set; }
        public string HabilidadesRequeridas { get; set; }
        public string Ubicacion { get; set; }
        public string TipoTrabajo { get; set; } // "Remoto", "Presencial", "Híbrido"
        
        public virtual ICollection<VacanteUsuario> VacanteUsuarios { get; set; }
        
        
    }
}