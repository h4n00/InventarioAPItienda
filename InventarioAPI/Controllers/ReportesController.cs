using InventarioAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace InventarioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("productos-excel")]
        public async Task<IActionResult> ExportarProductos()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .Where(p => p.Activo)
                .ToListAsync();

            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Productos");

            // Encabezados
            string[] headers = { "ID", "Código", "Nombre", "Categoría",
                                  "Proveedor", "Precio Compra", "Precio Venta",
                                  "Stock Actual", "Stock Mínimo" };

            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cells[1, i + 1].Value = headers[i];
                sheet.Cells[1, i + 1].Style.Font.Bold = true;
                sheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(41, 128, 185));
                sheet.Cells[1, i + 1].Style.Font.Color.SetColor(Color.White);
            }

            // Datos
            for (int i = 0; i < productos.Count; i++)
            {
                var p = productos[i];
                int row = i + 2;

                sheet.Cells[row, 1].Value = p.Id;
                sheet.Cells[row, 2].Value = p.Codigo;
                sheet.Cells[row, 3].Value = p.Nombre;
                sheet.Cells[row, 4].Value = p.Categoria?.Nombre ?? "";
                sheet.Cells[row, 5].Value = p.Proveedor?.Nombre ?? "";
                sheet.Cells[row, 6].Value = p.PrecioCompra;
                sheet.Cells[row, 7].Value = p.PrecioVenta;
                sheet.Cells[row, 8].Value = p.StockActual;
                sheet.Cells[row, 9].Value = p.StockMinimo;

                // Resaltar productos bajo stock
                if (p.StockActual <= p.StockMinimo)
                {
                    sheet.Cells[row, 1, row, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sheet.Cells[row, 1, row, 9].Style.Fill.BackgroundColor
                        .SetColor(Color.FromArgb(255, 200, 200));
                }
            }

            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

            var archivo = package.GetAsByteArray();
            var nombre = $"Productos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(archivo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                nombre);
        }

        [HttpGet("movimientos-excel")]
        public async Task<IActionResult> ExportarMovimientos()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var movimientos = await _context.Movimientos
                .Include(m => m.Producto)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();

            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Movimientos");

            string[] headers = { "ID", "Producto", "Tipo", "Cantidad",
                                  "Stock Anterior", "Stock Nuevo", "Motivo", "Fecha" };

            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cells[1, i + 1].Value = headers[i];
                sheet.Cells[1, i + 1].Style.Font.Bold = true;
                sheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(39, 174, 96));
                sheet.Cells[1, i + 1].Style.Font.Color.SetColor(Color.White);
            }

            for (int i = 0; i < movimientos.Count; i++)
            {
                var m = movimientos[i];
                int row = i + 2;

                sheet.Cells[row, 1].Value = m.Id;
                sheet.Cells[row, 2].Value = m.Producto?.Nombre ?? "";
                sheet.Cells[row, 3].Value = m.TipoMovimientoId == 1 ? "Entrada" : "Salida";
                sheet.Cells[row, 4].Value = m.Cantidad;
                sheet.Cells[row, 5].Value = m.StockAnterior;
                sheet.Cells[row, 6].Value = m.StockNuevo;
                sheet.Cells[row, 7].Value = m.Motivo ?? "";
                sheet.Cells[row, 8].Value = m.Fecha.ToString("dd/MM/yyyy HH:mm");
            }

            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

            var archivo = package.GetAsByteArray();
            var nombre = $"Movimientos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(archivo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                nombre);
        }
    }
}