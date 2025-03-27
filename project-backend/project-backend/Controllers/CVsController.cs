using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_backend.Data;
using project_backend.Models;
using project_backend.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace project_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CVsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CVsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Obtebner ID de user autenticado
        private int GetAuthenticatedUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }
            return userId;
        }

        // GET: api/CVs
        [Authorize(Roles = "Empresarial, Postulante")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CVDTO>>> GetCVs()
        {
            var userId = GetAuthenticatedUserId();
            var cvs = await _context.CVs
                .Where(cv => cv.IdUsuario == userId)
                .ToListAsync();

            var cvsDTO = cvs.Select(cv => new CVDTO
            {
                Id = cv.Id,
                RutaArchivo = cv.RutaArchivo,
                FechaSubida = cv.FechaSubida,
                IdUsuario = cv.IdUsuario,
                IdVacante = cv.IdVacante
            }).ToList();

            return Ok(cvsDTO);
        }

        // GET: api/CVs/5
        [Authorize(Roles = "Empresarial, Postulante")]
        [HttpGet("{id}")]
        public async Task<ActionResult<CVDTO>> GetCV(int id)
        {
            var userId = GetAuthenticatedUserId();
            var cv = await _context.CVs
                .FirstOrDefaultAsync(c => c.Id == id && c.IdUsuario == userId);

            if (cv == null)
            {
                return NotFound();
            }

            var cvDTO = new CVDTO
            {
                Id = cv.Id,
                RutaArchivo = cv.RutaArchivo,
                FechaSubida = cv.FechaSubida,
                IdUsuario = cv.IdUsuario,
                IdVacante = cv.IdVacante
            };

            return Ok(cvDTO);
        }

        // PUT: api/CVs/5
        [Authorize(Roles = "Postulante")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCV(int id, CVDTO cvDTO)
        {
            if (id != cvDTO.Id)
            {
                return BadRequest();
            }

            var userId = GetAuthenticatedUserId();
            var existingCV = await _context.CVs.FindAsync(id);

            if (existingCV == null || existingCV.IdUsuario != userId)
            {
                return NotFound();
            }

            existingCV.RutaArchivo = cvDTO.RutaArchivo;
            existingCV.FechaSubida = cvDTO.FechaSubida;
            existingCV.IdVacante = cvDTO.IdVacante;

            _context.Entry(existingCV).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CVExists(id))
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

        // POST: api/CVs/upload/{idVacante}
        [Authorize(Roles = "Postulante")]
        [HttpPost("upload/{idVacante}")]
        public async Task<ActionResult<CVDTO>> UploadCV(IFormFile file, int idVacante)
        {
            try
            {
                var userId = GetAuthenticatedUserId();

                if (file == null || file.Length == 0)
                {
                    return BadRequest("No se ha proporcionado un archivo válido.");
                }

                // Debe ser PDF
                if (file.ContentType != "application/pdf")
                {
                    return BadRequest("Solo se permiten archivos PDF.");
                }

                // La vacante debe existir!!!!!!!!!!!!
                var vacante = await _context.Vacantes.FindAsync(idVacante);
                if (vacante == null)
                {
                    return BadRequest("La vacante especificada no existe.");
                }

                // No debe existir un CV para esta vacante!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                var existingCV = await _context.CVs
                    .FirstOrDefaultAsync(c => c.IdVacante == idVacante && c.IdUsuario == userId);

                if (existingCV != null)
                {
                    return BadRequest("Ya has subido un CV para esta vacante.");
                }

                // _env.WebRootPath no sea null
                if (string.IsNullOrEmpty(_env.WebRootPath))
                {
                    return StatusCode(500, "La ruta del servidor no está configurada correctamente.");
                }

                // Crear carpeta para CVs si no existe
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "cvs");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generar nombre único para el archivo
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Guardar
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Crear registro
                var cv = new CV
                {
                    RutaArchivo = fileName,
                    FechaSubida = DateTime.UtcNow,
                    IdUsuario = userId,
                    IdVacante = idVacante
                };

                _context.CVs.Add(cv);
                await _context.SaveChangesAsync();

                // Crear DTO
                var cvDTO = new CVDTO
                {
                    Id = cv.Id,
                    RutaArchivo = cv.RutaArchivo,
                    FechaSubida = cv.FechaSubida,
                    IdUsuario = cv.IdUsuario,
                    IdVacante = cv.IdVacante
                };

                return Ok(cvDTO);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/CVs/download/{idVacante}
        [HttpGet("download/{idVacante}")]
        public async Task<IActionResult> DownloadCV(int idVacante)
        {
            try
            {
                var userId = GetAuthenticatedUserId();

                // Buscar el CV del usuario para la vacante especificada
                var cv = await _context.CVs
                    .FirstOrDefaultAsync(c => c.IdVacante == idVacante && c.IdUsuario == userId);

                if (cv == null)
                {
                    return NotFound("CV no encontrado para esta vacante.");
                }

                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "cvs");
                var filePath = Path.Combine(uploadsFolder, cv.RutaArchivo);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Archivo no encontrado.");
                }

                var fileStream = System.IO.File.OpenRead(filePath);
                return File(fileStream, "application/pdf", cv.RutaArchivo);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // DELETE: api/CVs/5
        [Authorize(Roles = "Postulante")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCV(int id)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                var cv = await _context.CVs
                    .FirstOrDefaultAsync(c => c.Id == id && c.IdUsuario == userId);

                if (cv == null)
                {
                    return NotFound("CV no encontrado.");
                }

                // Eliminar el archivo del servidor
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "cvs");
                var filePath = Path.Combine(uploadsFolder, cv.RutaArchivo);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Eliminar el registro de la BD
                _context.CVs.Remove(cv);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        private bool CVExists(int id)
        {
            return _context.CVs.Any(e => e.Id == id);
        }
    }
}