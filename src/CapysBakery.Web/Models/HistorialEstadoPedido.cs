namespace CapysBakery.Web.Models;

// Coincide con historial_estado_pedido. Registra cada cambio de estado
// para trazabilidad — se inserta en la misma operación en que cambia
// pedido.EstadoPedidoId (regla de integridad documentada en el diseño de BD).
public class HistorialEstadoPedido
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int EstadoPedidoId { get; set; }
    public EstadoPedido? EstadoPedido { get; set; }
    public DateTime FechaCambio { get; set; } = DateTime.Now;
}
