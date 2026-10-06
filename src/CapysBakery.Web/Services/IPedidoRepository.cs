using CapysBakery.Web.Models;

namespace CapysBakery.Web.Services;

// Mismo patrón que los demás repositorios. Hoy la implementa
// MockPedidoRepository (en memoria — se reinicia si se detiene la app).
// TODO (cuando SQL Server esté listo): EfPedidoRepository hace lo mismo
// contra la tabla pedido de verdad.
public interface IPedidoRepository
{
    // Para el panel de administración (Dueño/Administrador): todos los
    // pedidos, de cualquier comprador.
    List<Pedido> ObtenerTodos();

    // Para "Mis pedidos" (comprador): solo los suyos.
    List<Pedido> ObtenerPorUsuario(int usuarioId);

    Pedido? ObtenerPorId(int id);

    List<EstadoPedido> ObtenerEstados();

    // Crea el pedido en estado "Pendiente" y registra el primer evento en
    // el historial, en la misma operación (regla de integridad documentada
    // en el diseño de BD).
    Pedido CrearPedido(Pedido pedido);

    // Cambia el estado actual del pedido Y agrega el evento correspondiente
    // al historial — nunca se hace uno sin el otro.
    bool CambiarEstado(int pedidoId, int nuevoEstadoId);

    // Guarda la calificación (1-5 estrellas) que el cliente deja sobre su
    // pedido, una vez entregado. Devuelve false si el pedido no existe, no
    // está "Entregado" todavía, o ya fue calificado antes (una sola
    // calificación por pedido).
    bool Calificar(int pedidoId, int estrellas, string? comentario);

    // Promedio de estrellas y cantidad total de calificaciones — alimenta
    // el "Calificación de clientes" del Home, que antes era un número fijo
    // (4.9★) escrito a mano.
    (double Promedio, int Total) ObtenerEstadisticasCalificacion();

    // Todas las calificaciones que sí tienen comentario escrito, para que
    // el Dueño/Administrador elija cuáles destacar como testimonios reales
    // en el Home (ver /AdminTestimonios).
    List<CalificacionPedido> ObtenerCalificacionesConComentario();

    // Marca o quita una calificación como testimonio destacado.
    bool MarcarComoDestacada(int calificacionId, bool destacado);

    // Las calificaciones ya marcadas como destacadas — reemplazan los 3
    // comentarios fijos de ejemplo que traía el diseño original del Home.
    List<CalificacionPedido> ObtenerCalificacionesDestacadas(int cantidad);
}
