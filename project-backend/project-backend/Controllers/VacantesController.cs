using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_backend.Data;
using project_backend.DTOs;
using project_backend.Models;

namespace project_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacantesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VacantesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Vacantes
        //[Authorize(Roles = "Empresarial")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VacantesDTO>>> GetVacantes()
        {
            var vacantes = await _context.Vacantes
                .Include(v => v.CVs) // Incluir los CVs relacionados
                .ToListAsync();

            var vacantesDTO = vacantes.Select(v => new VacantesDTO
            {
                Id = v.Id,
                Nombre = v.Nombre,
                Descripcion = v.Descripcion,
                Salario = v.Salario,
                Horario = v.Horario,
                FechaPublicacion = v.FechaPublicacion,
                FechaExpiracion = v.FechaExpiracion,
                HabilidadesRequeridas = v.HabilidadesRequeridas,
                Ubicacion = v.Ubicacion,
                TipoTrabajo = v.TipoTrabajo,
                CVs = v.CVs.Select(cv => new CVDTO
                {
                    Id = cv.Id,
                    RutaArchivo = cv.RutaArchivo,
                    FechaSubida = cv.FechaSubida,
                    IdUsuario = cv.IdUsuario,
                    IdVacante = cv.IdVacante
                }).ToList()
            }).ToList();

            return Ok(vacantesDTO);
        }

        // GET: api/Vacantes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VacantesDTO>> GetVacante(int id)
        {
            var vacante = await _context.Vacantes
                .Include(v => v.CVs)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vacante == null)
            {
                return NotFound();
            }

            var vacanteDTO = new VacantesDTO
            {
                Id = vacante.Id,
                Nombre = vacante.Nombre,
                Descripcion = vacante.Descripcion,
                Salario = vacante.Salario,
                Horario = vacante.Horario,
                FechaPublicacion = vacante.FechaPublicacion,
                FechaExpiracion = vacante.FechaExpiracion,
                HabilidadesRequeridas = vacante.HabilidadesRequeridas,
                Ubicacion = vacante.Ubicacion,
                TipoTrabajo = vacante.TipoTrabajo,
                CVs = vacante.CVs.Select(cv => new CVDTO
                {
                    Id = cv.Id,
                    RutaArchivo = cv.RutaArchivo,
                    FechaSubida = cv.FechaSubida,
                    IdUsuario = cv.IdUsuario,
                    IdVacante = cv.IdVacante
                }).ToList()
            };

            return Ok(vacanteDTO);
        }

        // PUT: api/Vacantes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Empresarial")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVacante(int id, Vacante vacante)
        {
            if (id != vacante.Id)
            {
                return BadRequest();
            }

            _context.Entry(vacante).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VacanteExists(id))
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

        // POST: api/Vacantes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Empresarial")]
        [HttpPost]
        public async Task<ActionResult<Vacante>> PostVacante([FromBody] VacantesDTO vacantedto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var vacante = new Vacante
            {
                Nombre = vacantedto.Nombre,
                Descripcion = vacantedto.Descripcion,
                Salario = vacantedto.Salario,
                Horario = vacantedto.Horario,
                FechaPublicacion = DateTime.Now,
                FechaExpiracion = vacantedto.FechaExpiracion,
                HabilidadesRequeridas = vacantedto.HabilidadesRequeridas,
                Ubicacion = vacantedto.Ubicacion,
                TipoTrabajo = vacantedto.TipoTrabajo,
                UsuarioId = userId
            };

            _context.Vacantes.Add(vacante);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVacante", new { id = vacante.Id }, vacante);
        }

        // DELETE: api/Vacantes/5
        [Authorize(Roles = "Empresarial")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVacante(int id)
        {
            var vacante = await _context.Vacantes.FindAsync(id);
            if (vacante == null)
            {
                return NotFound();
            }

            _context.Vacantes.Remove(vacante);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VacanteExists(int id)
        {
            return _context.Vacantes.Any(e => e.Id == id);
        }

        // GET: api/Vacantes/mis-vacantes
        [Authorize(Roles = "Empresarial")]
        [HttpGet("mis-vacantes")]
        public async Task<ActionResult<IEnumerable<VacantesDTO>>> GetMisVacantes()
        {
            // ID del usuario autenticado
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Obtener solo las vacantes del empresarial logeado
            var vacantes = await _context.Vacantes
                .Where(v => v.UsuarioId == userId)
                .Include(v => v.CVs)
                .ToListAsync();

            var vacantesDTO = vacantes.Select(v => new VacantesDTO
            {
                Id = v.Id,
                Nombre = v.Nombre,
                Descripcion = v.Descripcion,
                Salario = v.Salario,
                Horario = v.Horario,
                FechaPublicacion = v.FechaPublicacion,
                FechaExpiracion = v.FechaExpiracion,
                HabilidadesRequeridas = v.HabilidadesRequeridas,
                Ubicacion = v.Ubicacion,
                TipoTrabajo = v.TipoTrabajo,
                UsuarioId = v.UsuarioId,
                CVs = v.CVs.Select(cv => new CVDTO
                {
                    Id = cv.Id,
                    RutaArchivo = cv.RutaArchivo,
                    FechaSubida = cv.FechaSubida,
                    IdUsuario = cv.IdUsuario,
                    IdVacante = cv.IdVacante
                }).ToList()
            }).ToList();

            return Ok(vacantesDTO);
        }
    }
}
