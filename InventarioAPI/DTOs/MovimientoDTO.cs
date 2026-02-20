namespace InventarioAPI.DTOs
{
    public class MovimientoDTO
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public string? Motivo { get; set; }
    }
}