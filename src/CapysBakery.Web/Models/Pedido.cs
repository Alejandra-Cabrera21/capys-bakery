namespace CapysBakery.Web.Models;

// Coincide con pedido — antes era un borrador plano que nunca se guardaba
// en ningún lado (checkout.js solo lo escribía en localStorage). Ahora
// tiene relaciones reales y sí se persiste (ver IPedidoRepository y el
// endpoint nuevo Checkout/Confirmar, Fase 4).
public class Pedido
{
    public int Id { get; set; }
    public string CodigoPedido { get; set; } = string.Empty; // Ej. "CB-00125"
    public string NombreCliente { get; set; } = string.Empty;
    public string TelefonoCliente { get; set; } = string.Empty;
    public DateTime FechaEntregaSolicitada { get; set; }

    public int ModalidadEntregaId { get; set; }
    public ModalidadEntrega? ModalidadEntrega { get; set; }

    public string? DireccionOPuntoEntrega { get; set; }

    public int MetodoPagoId { get; set; }
    public MetodoPago? MetodoPago { get; set; }

    public int EstadoPedidoId { get; set; }
    public EstadoPedido? EstadoPedido { get; set; }

    public string? Comentarios { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    // Consideración futura ya anotada en el diseño de BD original: "Si
    // posteriormente se implementan cuentas de usuario, el pedido puede
    // incorporar id_usuario nullable sin perder los datos históricos
    // nombre_cliente y telefono_cliente". Ya existen cuentas (ver Fase 2),
    // así que se agrega desde ahora — sigue siendo NULL para pedidos de
    // invitados si alguna vez se permitiera comprar sin cuenta.
    public int? UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public List<PedidoDetalle> Detalles { get; set; } = new();
    public List<HistorialEstadoPedido> Historial { get; set; } = new();

    // Conveniencia (NO se guarda como columna): el diseño de BD documenta
    // explícitamente que el total NO se persiste — se calcula a partir de
    // pedido_detalle (sección 7 del documento de diseño).
    public decimal Total => Detalles.Sum(d => d.PrecioUnitario * d.Cantidad);
}

// Nombres oficiales de los 6 estados confirmados por el cliente (coinciden
// con los valores ya cargados en la tabla estado_pedido). Sirven para
// buscar el EstadoPedido correspondiente por nombre sin usar "texto mágico"
// repetido por todo el código.
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
