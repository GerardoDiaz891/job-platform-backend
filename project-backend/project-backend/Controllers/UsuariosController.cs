using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_backend.Data;
using project_backend.Models;
using project_backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace project_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Rol) // Incluir Rol para obtener el nombre
                .ToListAsync();

            var usuariosDTO = usuarios.Select(u => new UsuarioDTO
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Correo = u.Correo,
                IdRol = u.IdRol,
                NombreRol = u.Rol?.Nombre, // Nombre del Rol
                NombreEmpresa = u.NombreEmpresa,
                TipoEmpresa = u.TipoEmpresa,
                Direccion = u.Direccion,
                Telefono = u.Telefono,
                SitioWeb = u.SitioWeb,
                DescripcionEmpresa = u.DescripcionEmpresa,
                IdCV = u.IdCV
            }).ToList();

            return Ok(usuariosDTO);
        }

        // GET: api/Usuarios/5
        [Authorize(Roles = "Administrador")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDTO>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol) // Incluir el Rol para obtener el nombre
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                return NotFound();
            }

            var usuarioDTO = new UsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                IdRol = usuario.IdRol,
                NombreRol = usuario.Rol?.Nombre,
                NombreEmpresa = usuario.NombreEmpresa,
                TipoEmpresa = usuario.TipoEmpresa,
                Direccion = usuario.Direccion,
                Telefono = usuario.Telefono,
                SitioWeb = usuario.SitioWeb,
                DescripcionEmpresa = usuario.DescripcionEmpresa,
                IdCV = usuario.IdCV
            };

            return Ok(usuarioDTO);
        }

        // PUT: api/Usuarios/5
        [Authorize(Roles = "Administrador")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, UsuarioDTO usuarioDTO)
        {
            if (id != usuarioDTO.Id)
            {
                return BadRequest();
            }

            // Obtener el usuario existente
            var usuarioExistente = await _context.Usuarios.FindAsync(id);
            if (usuarioExistente == null)
            {
                return NotFound();
            }

            // Verificar si el rol existe
            var rolExistente = await _context.Roles.FindAsync(usuarioDTO.IdRol);
            if (rolExistente == null)
            {
                return BadRequest("El rol especificado no existe.");
            }

            // Actualizar solo los campos permitidos
            usuarioExistente.Nombre = usuarioDTO.Nombre;
            usuarioExistente.Correo = usuarioDTO.Correo;
            usuarioExistente.IdRol = usuarioDTO.IdRol;
            usuarioExistente.NombreEmpresa = usuarioDTO.NombreEmpresa;
            usuarioExistente.TipoEmpresa = usuarioDTO.TipoEmpresa;
            usuarioExistente.Direccion = usuarioDTO.Direccion;
            usuarioExistente.Telefono = usuarioDTO.Telefono;
            usuarioExistente.SitioWeb = usuarioDTO.SitioWeb;
            usuarioExistente.DescripcionEmpresa = usuarioDTO.DescripcionEmpresa;
            usuarioExistente.IdCV = usuarioDTO.IdCV;

            // Actualizar contraseña solo si se proporcionó una nueva
            if (!string.IsNullOrEmpty(usuarioDTO.Contraseña))
            {
                usuarioExistente.SetPassword(usuarioDTO.Contraseña);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Usuarios
        //[Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<ActionResult<UsuarioDTO>> PostUsuario(UsuarioDTO usuarioDTO)
        {
            // Verificar si el rol existe
            var rolExistente = await _context.Roles.FindAsync(usuarioDTO.IdRol);
            if (rolExistente == null)
            {
                return BadRequest("El rol especificado no existe.");
            }

            // Crear el usuario a partir del DTO
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
                DescripcionEmpresa = usuarioDTO.DescripcionEmpresa,
                IdCV = usuarioDTO.IdCV
            };

            // Hashear la contraseña antes de guardar el usuario
            usuario.SetPassword(usuario.Contraseña);

            // Asignar el Rol existente al usuario
            usuario.Rol = rolExistente;

            // Agregar el usuario a la base de datos
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Crear el DTO de respuesta
            var nuevoUsuarioDTO = new UsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                IdRol = usuario.IdRol,
                NombreRol = usuario.Rol?.Nombre,
                NombreEmpresa = usuario.NombreEmpresa,
                TipoEmpresa = usuario.TipoEmpresa,
                Direccion = usuario.Direccion,
                Telefono = usuario.Telefono,
                SitioWeb = usuario.SitioWeb,
                DescripcionEmpresa = usuario.DescripcionEmpresa,
                IdCV = usuario.IdCV
            };

            return CreatedAtAction("GetUsuario", new { id = nuevoUsuarioDTO.Id }, nuevoUsuarioDTO);
        }

        // DELETE: api/Usuarios/5
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }


        // GET: api/Usuarios/mi-informacion
        [Authorize]
        [HttpGet("mi-informacion")]
        public async Task<ActionResult<UsuarioDTO>> GetMiInformacion()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (usuario == null)
            {
                return NotFound();
            }

            var usuarioDTO = new UsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                IdRol = usuario.IdRol,
                NombreRol = usuario.Rol?.Nombre,
                NombreEmpresa = usuario.NombreEmpresa,
                TipoEmpresa = usuario.TipoEmpresa,
                Direccion = usuario.Direccion,
                Telefono = usuario.Telefono,
                SitioWeb = usuario.SitioWeb,
                DescripcionEmpresa = usuario.DescripcionEmpresa,
                IdCV = usuario.IdCV
            };

            return Ok(usuarioDTO);
        }


        // PUT: api/Usuarios/mi-informacion
        [Authorize]
        [HttpPut("mi-informacion")]
        public async Task<IActionResult> PutMiInformacion(UsuarioUpdateDTO usuarioUpdateDTO)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            var usuario = await _context.Usuarios.FindAsync(userId);
            if (usuario == null)
            {
                return NotFound();
            }

            usuario.Nombre = usuarioUpdateDTO.Nombre;
            usuario.NombreEmpresa = usuarioUpdateDTO.NombreEmpresa;
            usuario.TipoEmpresa = usuarioUpdateDTO.TipoEmpresa;
            usuario.Direccion = usuarioUpdateDTO.Direccion;
            usuario.Telefono = usuarioUpdateDTO.Telefono;
            usuario.SitioWeb = usuarioUpdateDTO.SitioWeb;
            usuario.DescripcionEmpresa = usuarioUpdateDTO.DescripcionEmpresa;

            if (!string.IsNullOrEmpty(usuarioUpdateDTO.Contraseña))
            {
                usuario.SetPassword(usuarioUpdateDTO.Contraseña);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(userId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
    }
}