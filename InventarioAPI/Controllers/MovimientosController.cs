using InventarioAPI.Data;
using InventarioAPI.DTOs;
using InventarioAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventarioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MovimientosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MovimientosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Movimientos
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var movimientos = await _context.Movimientos
                .Include(m => m.Producto)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();

            return Ok(movimientos);
        }

        // GET: api/Movimientos/producto/5
        [HttpGet("producto/{productoId}")]
        public async Task<IActionResult> GetByProducto(int productoId)
        {
            var movimientos = await _context.Movimientos
                .Include(m => m.Producto)
                .Where(m => m.ProductoId == productoId)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();

            return Ok(movimientos);
        }

        // POST: api/Movimientos/entrada
        [HttpPost("entrada")]
        public async Task<IActionResult> Entrada(MovimientoDTO dto)
        {
            var producto = await _context.Productos.FindAsync(dto.ProductoId);

            if (producto == null)
                return NotFound("Producto no encontrado.");

            if (dto.Cantidad <= 0)
                return BadRequest("La cantidad debe ser mayor a 0.");

            int stockAnterior = producto.StockActual;
            producto.StockActual += dto.Cantidad;

            var movimiento = new Movimiento
            {
                ProductoId = dto.ProductoId,
                TipoMovimientoId = 1, // Entrada
                Cantidad = dto.Cantidad,
                StockAnterior = stockAnterior,
                StockNuevo = producto.StockActual,
                Motivo = dto.Motivo,
                UsuarioId = ObtenerUsuarioId(),
                Fecha = DateTime.Now
            };

            _context.Movimientos.Add(movimiento);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Entrada registrada correctamente.",
                stockAnterior,
                stockNuevo = producto.StockActual
            });
        }

        // POST: api/Movimientos/salida
        [HttpPost("salida")]
        public async Task<IActionResult> Salida(MovimientoDTO dto)
        {
            var producto = await _context.Productos.FindAsync(dto.ProductoId);

            if (producto == null)
                return NotFound("Producto no encontrado.");

            if (dto.Cantidad <= 0)
                return BadRequest("La cantidad debe ser mayor a 0.");

            if (producto.StockActual < dto.Cantidad)
                return BadRequest($"Stock insuficiente. Stock actual: {producto.StockActual}");

            int stockAnterior = producto.StockActual;
            producto.StockActual -= dto.Cantidad;

            var movimiento = new Movimiento
            {
                ProductoId = dto.ProductoId,
                TipoMovimientoId = 2, // Salida
                Cantidad = dto.Cantidad,
                StockAnterior = stockAnterior,
                StockNuevo = producto.StockActual,
                Motivo = dto.Motivo,
                UsuarioId = ObtenerUsuarioId(),
                Fecha = DateTime.Now
            };

            _context.Movimientos.Add(movimiento);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Salida registrada correctamente.",
                stockAnterior,
                stockNuevo = producto.StockActual,
                bajoStock = producto.StockActual <= producto.StockMinimo
            });
        }

        private int? ObtenerUsuarioId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int id))
                return id;
            return null;
        }
    }
}