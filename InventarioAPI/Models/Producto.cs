namespace InventarioAPI.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int? CategoriaId { get; set; }
        public int? ProveedorId { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public int StockActual { get; set; }
        public int StockMinimo { get; set; } = 5;
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public Categoria? Categoria { get; set; }
        public Proveedor? Proveedor { get; set; }
    }
}