namespace CapysBakery.Web.Models;

// Coincide con pedido_detalle_personalizacion: personalizaciones elegidas
// para una línea del pedido, con su costo adicional congelado.
public class PedidoDetallePersonalizacion
{
    public int Id { get; set; }
    public int PedidoDetalleId { get; set; }
    public int ProductoOpcionId { get; set; }
    public ProductoOpcionPersonalizacion? ProductoOpcion { get; set; }
    public decimal PrecioAdicionalUnitario { get; set; }
}
