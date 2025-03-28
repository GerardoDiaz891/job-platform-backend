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

namespace project_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RolsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Rols
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolDTO>>> GetRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            var rolesDTO = roles.Select(r => new RolDTO
            {
                Id = r.Id,
                Nombre = r.Nombre
            }).ToList();

            return Ok(rolesDTO);
        }

        // GET: api/Rols/5
        [Authorize(Roles = "Administrador")]
        [HttpGet("{id}")]
        public async Task<ActionResult<RolDTO>> GetRol(int id)
        {
            var rol = await _context.Roles.FindAsync(id);

            if (rol == null)
            {
                return NotFound();
            }

            var rolDTO = new RolDTO
            {
                Id = rol.Id,
                Nombre = rol.Nombre
            };

            return Ok(rolDTO);
        }

        // PUT: api/Rols/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRol(int id, RolDTO rolDTO)
        {
            if (id != rolDTO.Id)
            {
                return BadRequest();
            }

            var rol = new Rol
            {
                Id = rolDTO.Id,
                Nombre = rolDTO.Nombre
            };

            _context.Entry(rol).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolExists(id))
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

        // POST: api/Rols
        //[Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<ActionResult<RolDTO>> PostRol(RolDTO rolDTO)
        {
            var rol = new Rol
            {
                Nombre = rolDTO.Nombre
            };

            _context.Roles.Add(rol);
            await _context.SaveChangesAsync();

            var nuevoRolDTO = new RolDTO
            {
                Id = rol.Id,
                Nombre = rol.Nombre
            };

            return CreatedAtAction("GetRol", new { id = nuevoRolDTO.Id }, nuevoRolDTO);
        }

        // DELETE: api/Rols/5
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRol(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(rol);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RolExists(int id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }
}