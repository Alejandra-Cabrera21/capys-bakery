namespace CapysBakery.Web.Models;

// Coincide con pedido_detalle: una línea del pedido (un producto en una
// presentación específica) con su precio congelado al momento de la
// compra — cambios futuros en el catálogo no deben alterar pedidos ya
// hechos, tal como documenta el diseño de BD.
public class PedidoDetalle
{
    public int Id { get; set; }
    public int PedidoId { get; set; }

    public int PresentacionId { get; set; }
    public ProductoPresentacion? Presentacion { get; set; }

    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }

    public List<PedidoDetallePersonalizacion> Personalizaciones { get; set; } = new();
}
