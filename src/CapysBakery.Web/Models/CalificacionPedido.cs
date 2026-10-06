namespace CapysBakery.Web.Models;

// Corresponde a calificacion_pedido. No forma parte del diseño original
// de 18 tablas — permite que el cliente califique su pedido (1 a 5
// estrellas) una vez que está "Entregado". Reemplaza el "4.9★" fijo que
// tenía el Home por un promedio calculado en tiempo real.
public class CalificacionPedido
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public Pedido? Pedido { get; set; }
    public int Estrellas { get; set; } // 1 a 5
    public string? Comentario { get; set; }
    public DateTime FechaCalificacion { get; set; } = DateTime.Now;

    // El Dueño/Administrador decide cuáles calificaciones (con comentario)
    // se muestran como testimonios reales en el Home, desde
    // /AdminTestimonios — reemplaza los 3 comentarios fijos de ejemplo
    // que traía el diseño original.
    public bool Destacado { get; set; } = false;
}
