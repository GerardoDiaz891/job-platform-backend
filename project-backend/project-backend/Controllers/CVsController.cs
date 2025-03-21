using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public class CVsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CVsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/CVs
        [Authorize(Roles = "Empresarial, Postulante")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CVDTO>>> GetCVs()
        {
            var cvs = await _context.CVs.ToListAsync();
            var cvsDTO = cvs.Select(cv => new CVDTO
            {
                Id = cv.Id,
                RutaArchivo = cv.RutaArchivo,
                FechaSubida = cv.FechaSubida,
                IdUsuario = cv.IdUsuario
            }).ToList();

            return Ok(cvsDTO);
        }

        // GET: api/CVs/5
        [Authorize(Roles = "Empresarial, Postulante")]
        [HttpGet("{id}")]
        public async Task<ActionResult<CVDTO>> GetCV(int id)
        {
            var cv = await _context.CVs.FindAsync(id);

            if (cv == null)
            {
                return NotFound();
            }

            var cvDTO = new CVDTO
            {
                Id = cv.Id,
                RutaArchivo = cv.RutaArchivo,
                FechaSubida = cv.FechaSubida,
                IdUsuario = cv.IdUsuario
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

            var cv = new CV
            {
                Id = cvDTO.Id,
                RutaArchivo = cvDTO.RutaArchivo,
                FechaSubida = cvDTO.FechaSubida,
                IdUsuario = cvDTO.IdUsuario
            };

            _context.Entry(cv).State = EntityState.Modified;

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

        // Método para subir un archivo PDF
        [Authorize(Roles = "Postulante")]
        [HttpPost("upload")]
        public async Task<ActionResult<CVDTO>> UploadCV(IFormFile file, int usuarioId, int idVacante)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No se ha proporcionado un archivo válido.");
            }

            // Verificar que el archivo sea un PDF
            if (file.ContentType != "application/pdf")
            {
                return BadRequest("Solo se permiten archivos PDF.");
            }

            // Verificar que el usuario exista
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return BadRequest("El usuario especificado no existe.");
            }

            // Verificar que la vacante exista
            var vacante = await _context.Vacantes.FindAsync(idVacante);
            if (vacante == null)
            {
                return BadRequest("La vacante especificada no existe.");
            }

            // Verificar que _env.WebRootPath no sea null
            if (string.IsNullOrEmpty(_env.WebRootPath))
            {
                return StatusCode(500, "La ruta del servidor no está configurada correctamente.");
            }

            // Crear una carpeta para almacenar los CVs si no existe
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "cvs");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generar un nombre único para el archivo
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Guardar el archivo en el servidor
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Crear un nuevo registro de CV en la base de datos
            var cv = new CV
            {
                RutaArchivo = fileName, // Guardar solo el nombre del archivo
                FechaSubida = DateTime.UtcNow,
                IdUsuario = usuarioId,
                IdVacante = idVacante // Asociar el CV a la vacante
            };

            _context.CVs.Add(cv);
            await _context.SaveChangesAsync();

            // Crear el DTO de respuesta
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

        // Método para descargar un archivo PDF
        [Authorize]
        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadCV(int id)
        {
            var cv = await _context.CVs.FindAsync(id);
            if (cv == null)
            {
                return NotFound("CV no encontrado.");
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

        // DELETE: api/CVs/5
        [Authorize(Roles = "Postulante")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCV(int id)
        {
            var cv = await _context.CVs.FindAsync(id);
            if (cv == null)
            {
                return NotFound("CV no encontrado.");
            }

            // Eliminar el archivo físico del servidor
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "cvs");
            var filePath = Path.Combine(uploadsFolder, cv.RutaArchivo);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Eliminar el registro de la base de datos
            _context.CVs.Remove(cv);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CVExists(int id)
        {
            return _context.CVs.Any(e => e.Id == id);
        }
    }
}