using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_backend.Models;

public class VacanteUsuario
{
    [Key]
    public int Id { get; set; }
   
    [ForeignKey(nameof(Usuario))]
    public int IdUsuario { get; set; }
    public virtual Usuario Usuario { get; set; }
    
    [ForeignKey(nameof(Vacante))]
    public int IdVacante { get; set; }
    public virtual Vacante Vacante { get; set; }
    
}