namespace project_backend.DTOs
{
    public class CVDTO
    {
        public int Id { get; set; }
        public string RutaArchivo { get; set; }
        public DateTime FechaSubida { get; set; }
        public int IdUsuario { get; set; }
    }
}