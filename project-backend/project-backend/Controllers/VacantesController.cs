using System;
using System.Collections.Generic;
using System.Linq;
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
        [Authorize(Roles = "Empresarial")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vacante>>> GetVacantes()
        {
            return await _context.Vacantes.ToListAsync();
        }

        // GET: api/Vacantes/5
        [Authorize(Roles = "Empresarial")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Vacante>> GetVacante(int id)
        {
            var vacante = await _context.Vacantes.FindAsync(id);

            if (vacante == null)
            {
                return NotFound();
            }

            return vacante;
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
        public async Task<ActionResult<Vacante>> PostVacante(VacantesDTO vacantedto)
        {   
            Vacante vacante = new ()
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
                
            };
            if(vacantedto.UsuarioId != null)
            {
                VacanteUsuario vacanteusuario = new()
                {

                    IdUsuario = (int)vacantedto.UsuarioId,
                    IdVacante = vacante.Id,
                };
            }
            
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
    }
}
