using InventarioAPI.Data;
using InventarioAPI.DTOs;
using InventarioAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProveedoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProveedoresController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var proveedores = await _context.Proveedores
                .Where(p => p.Activo)
                .ToListAsync();
            return Ok(proveedores);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var proveedor = await _context.Proveedores
                .FirstOrDefaultAsync(p => p.Id == id && p.Activo);

            if (proveedor == null)
                return NotFound("Proveedor no encontrado.");

            return Ok(proveedor);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ProveedorDTO dto)
        {
            var proveedor = new Proveedor
            {
                Nombre = dto.Nombre,
                Contacto = dto.Contacto,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Direccion = dto.Direccion,
                Activo = true,
                FechaCreacion = DateTime.Now
            };

            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();
            return Ok(proveedor);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, ProveedorDTO dto)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor == null)
                return NotFound("Proveedor no encontrado.");

            proveedor.Nombre = dto.Nombre;
            proveedor.Contacto = dto.Contacto;
            proveedor.Telefono = dto.Telefono;
            proveedor.Email = dto.Email;
            proveedor.Direccion = dto.Direccion;

            await _context.SaveChangesAsync();
            return Ok(proveedor);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor == null)
                return NotFound("Proveedor no encontrado.");

            proveedor.Activo = false;
            await _context.SaveChangesAsync();
            return Ok("Proveedor eliminado correctamente.");
        }
    }
}