using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_backend.Data;
using project_backend.Models;
using project_backend.Services;
using System.Threading.Tasks;

namespace project_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;

        public AuthController(ApplicationDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Buscar el usuario por correo
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == request.Correo);

            // Verificar si el usuario existe y si la contraseña es correcta
            if (usuario == null || !usuario.VerifyPassword(request.Contraseña))
            {
                return Unauthorized("Correo o contraseña incorrectos.");
            }

            // Generar el token JWT
            var token = _tokenService.GenerateToken(usuario);

            return Ok(new { Token = token });
        }
    }

    public class LoginRequest
    {
        public string Correo { get; set; }
        public string Contraseña { get; set; }
    }
}