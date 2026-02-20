namespace InventarioAPI.Models
{
    public class Movimiento
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public int TipoMovimientoId { get; set; }
        public int Cantidad { get; set; }
        public int StockAnterior { get; set; }
        public int StockNuevo { get; set; }
        public string? Motivo { get; set; }
        public int? UsuarioId { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public Producto? Producto { get; set; }
    }
}