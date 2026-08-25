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
}
