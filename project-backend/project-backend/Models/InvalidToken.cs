// Models/InvalidToken.cs
namespace project_backend.Models
{
    public class InvalidToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime InvalidatedAt { get; set; }
        public DateTime ExpiresAt { get; set; } // Para limpiar tokens expirados automáticamente
    }
}