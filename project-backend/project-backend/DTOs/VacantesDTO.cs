namespace project_backend.DTOs;

public class VacantesDTO
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
    public int? UsuarioId { get; set; } // FK al Usuario (empresarial)
    public List<CVDTO> CVs { get; set; } = new List<CVDTO>();
}