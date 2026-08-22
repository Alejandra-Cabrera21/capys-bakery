namespace CapysBakery.Web.Models;

// Representa la estructura de un pedido tal como la definió el cliente
// en el "Análisis funcional del flujo de realización del pedido".
// IMPORTANTE: mientras no exista base de datos, los pedidos NO se guardan
// aquí en el servidor — viven en localStorage del navegador del cliente
// (ver wwwroot/js/checkout.js). Esta clase documenta la forma que van a
// tener cuando se creen las tablas reales con Entity Framework Core.
public class Pedido
{
    public string Identificador { get; set; } = string.Empty; // Ej. "CB-1042"
    public string NombreCliente { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string FechaEntrega { get; set; } = string.Empty;
    public string FormaEntrega { get; set; } = string.Empty; // "Envío" o "Recoger"
    public string? Direccion { get; set; }
    public string ModalidadPago { get; set; } = string.Empty; // "Transferencia bancaria" o "Pago al recoger"
    public string? Comentarios { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = EstadosPedido.Pendiente;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
}

// Estados definidos por el cliente en el análisis funcional del flujo de pedido.
public static class EstadosPedido
{
    public const string Pendiente = "Pendiente";
    public const string Confirmado = "Confirmado";
    public const string EnPreparacion = "En preparación";
    public const string Listo = "Listo";
    public const string Entregado = "Entregado";
    public const string Cancelado = "Cancelado";

    public static readonly string[] Todos =
    {
        Pendiente, Confirmado, EnPreparacion, Listo, Entregado, Cancelado
    };
}
