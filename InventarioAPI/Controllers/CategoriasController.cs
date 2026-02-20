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
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categorias = await _context.Categorias
                .Where(c => c.Activo)
                .ToListAsync();
            return Ok(categorias);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id && c.Activo);

            if (categoria == null)
                return NotFound("Categoría no encontrada.");

            return Ok(categoria);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CategoriaDTO dto)
        {
            var categoria = new Categoria
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Activo = true
            };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            return Ok(categoria);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, CategoriaDTO dto)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                return NotFound("Categoría no encontrada.");

            categoria.Nombre = dto.Nombre;
            categoria.Descripcion = dto.Descripcion;

            await _context.SaveChangesAsync();
            return Ok(categoria);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                return NotFound("Categoría no encontrada.");

            categoria.Activo = false;
            await _context.SaveChangesAsync();
            return Ok("Categoría eliminada correctamente.");
        }
    }
}