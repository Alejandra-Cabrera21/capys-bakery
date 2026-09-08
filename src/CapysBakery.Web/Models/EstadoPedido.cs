namespace CapysBakery.Web.Models;

// Coincide con estado_pedido — los 6 estados confirmados por el cliente.
public class EstadoPedido
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
