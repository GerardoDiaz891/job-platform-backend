using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_backend.Data;
using project_backend.DTOs;
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

        // Endpoint para iniciar sesión
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

        // Endpoint para registro de usuarios
        [HttpPost("register")]
        public async Task<ActionResult<UsuarioDTO>> Register([FromBody] UsuarioDTO usuarioDTO)
        {
            // Verificar si el correo ya está registrado
            var usuarioExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == usuarioDTO.Correo);

            if (usuarioExistente != null)
            {
                return BadRequest("El correo ya está registrado.");
            }

            // Verificar si las contraseñas coinciden (asumiendo que se envían confirmadas desde el frontend)
            if (string.IsNullOrEmpty(usuarioDTO.Contraseña))
            {
                return BadRequest("La contraseña no puede estar vacía.");
            }

            // Asignar rol si no se especifica
            if (usuarioDTO.IdRol == 0)
            {
                var rolPredeterminado = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Nombre == "Postulante");

                if (rolPredeterminado == null)
                {
                    return BadRequest("No se encontró el rol predeterminado.");
                }

                usuarioDTO.IdRol = rolPredeterminado.Id;
            }

            // Crear el usuario
            var usuario = new Usuario
            {
                Nombre = usuarioDTO.Nombre,
                Correo = usuarioDTO.Correo,
                Contraseña = usuarioDTO.Contraseña,
                IdRol = usuarioDTO.IdRol,
                NombreEmpresa = usuarioDTO.NombreEmpresa,
                TipoEmpresa = usuarioDTO.TipoEmpresa,
                Direccion = usuarioDTO.Direccion,
                Telefono = usuarioDTO.Telefono,
                SitioWeb = usuarioDTO.SitioWeb,
                DescripcionEmpresa = usuarioDTO.DescripcionEmpresa
            };

            // Hashear la contraseña antes de guardar
            usuario.SetPassword(usuario.Contraseña);

            // Guardar en la base de datos
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Obtener el nombre del rol
            var rol = await _context.Roles.FindAsync(usuario.IdRol);

            // Crear respuesta con los datos completos
            var usuarioResponse = new UsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                IdRol = usuario.IdRol,
                NombreRol = rol?.Nombre,
                NombreEmpresa = usuario.NombreEmpresa,
                TipoEmpresa = usuario.TipoEmpresa,
                Direccion = usuario.Direccion,
                Telefono = usuario.Telefono,
                SitioWeb = usuario.SitioWeb,
                DescripcionEmpresa = usuario.DescripcionEmpresa
            };

            return CreatedAtAction(nameof(Login), usuarioResponse);
        }

    }

    // Clase para manejar la solicitud de inicio de sesión
    public class LoginRequest
    {
        public string Correo { get; set; }
        public string Contraseña { get; set; }
    }
}