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
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Productos
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .Where(p => p.Activo)
                .ToListAsync();

            return Ok(productos);
        }

        // GET: api/Productos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .FirstOrDefaultAsync(p => p.Id == id && p.Activo);

            if (producto == null)
                return NotFound("Producto no encontrado.");

            return Ok(producto);
        }

        // GET: api/Productos/bajo-stock
        [HttpGet("bajo-stock")]
        public async Task<IActionResult> GetBajoStock()
        {
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .Where(p => p.StockActual <= p.StockMinimo && p.Activo)
                .ToListAsync();

            return Ok(productos);
        }

        // POST: api/Productos
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ProductoDTO dto)
        {
            if (await _context.Productos.AnyAsync(p => p.Codigo == dto.Codigo))
                return BadRequest("El código ya existe.");

            var producto = new Producto
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                CategoriaId = dto.CategoriaId,
                ProveedorId = dto.ProveedorId,
                PrecioCompra = dto.PrecioCompra,
                PrecioVenta = dto.PrecioVenta,
                StockActual = 0,
                StockMinimo = dto.StockMinimo,
                Activo = true,
                FechaCreacion = DateTime.Now
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return Ok(producto);
        }

        // PUT: api/Productos/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, ProductoDTO dto)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return NotFound("Producto no encontrado.");

            producto.Nombre = dto.Nombre;
            producto.Descripcion = dto.Descripcion;
            producto.CategoriaId = dto.CategoriaId;
            producto.ProveedorId = dto.ProveedorId;
            producto.PrecioCompra = dto.PrecioCompra;
            producto.PrecioVenta = dto.PrecioVenta;
            producto.StockMinimo = dto.StockMinimo;

            await _context.SaveChangesAsync();

            return Ok(producto);
        }

        // DELETE: api/Productos/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return NotFound("Producto no encontrado.");

            producto.Activo = false;
            await _context.SaveChangesAsync();

            return Ok("Producto eliminado correctamente.");
        }
    }
}