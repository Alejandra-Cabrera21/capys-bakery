using CapysBakery.Web.Data;
using CapysBakery.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CapysBakery.Web.Services;

// Implementación REAL sobre CapysBakeryDb. Reemplaza a
// MockPedidoRepository (Fase 6) — a partir de aquí, un pedido sobrevive a
// un reinicio del servidor.
public class EfPedidoRepository : IPedidoRepository
{
    private readonly CapysBakeryDbContext _db;

    public EfPedidoRepository(CapysBakeryDbContext db)
    {
        _db = db;
    }

    private IQueryable<Pedido> ConIncludes() =>
        _db.Pedidos
            .Include(p => p.ModalidadEntrega)
            .Include(p => p.MetodoPago)
            .Include(p => p.EstadoPedido)
            .Include(p => p.Detalles).ThenInclude(d => d.Presentacion!).ThenInclude(pp => pp.Producto)
            .Include(p => p.Historial);

    public List<Pedido> ObtenerTodos() =>
        ConIncludes().OrderByDescending(p => p.FechaRegistro).ToList();

    public List<Pedido> ObtenerPorUsuario(int usuarioId) =>
        ConIncludes().Where(p => p.UsuarioId == usuarioId).OrderByDescending(p => p.FechaRegistro).ToList();

    public Pedido? ObtenerPorId(int id) => ConIncludes().FirstOrDefault(p => p.Id == id);

    public List<EstadoPedido> ObtenerEstados() => _db.EstadosPedido.OrderBy(e => e.Id).ToList();

    public Pedido CrearPedido(Pedido pedido)
    {
        var estadoPendiente = _db.EstadosPedido.First(e => e.Nombre == EstadosPedido.Pendiente);
        pedido.EstadoPedidoId = estadoPendiente.Id;
        pedido.FechaRegistro = DateTime.Now;

        // El código final (CB-00125) depende del Id real que asigna SQL
        // Server, así que primero se guarda con un valor temporal único.
        pedido.CodigoPedido = $"TEMP-{Guid.NewGuid():N}";

        _db.Pedidos.Add(pedido);
        _db.SaveChanges(); // a partir de aquí, pedido.Id ya es el real

        pedido.CodigoPedido = $"CB-{pedido.Id:00000}";
        pedido.Historial.Add(new HistorialEstadoPedido
        {
            PedidoId = pedido.Id,
            EstadoPedidoId = estadoPendiente.Id,
            FechaCambio = pedido.FechaRegistro,
        });

        _db.SaveChanges();
        return pedido;
    }

    public bool CambiarEstado(int pedidoId, int nuevoEstadoId)
    {
        var pedido = _db.Pedidos.FirstOrDefault(p => p.Id == pedidoId);
        if (pedido is null) return false;
        if (!_db.EstadosPedido.Any(e => e.Id == nuevoEstadoId)) return false;

        pedido.EstadoPedidoId = nuevoEstadoId;
        _db.HistorialEstadoPedido.Add(new HistorialEstadoPedido
        {
            PedidoId = pedidoId,
            EstadoPedidoId = nuevoEstadoId,
            FechaCambio = DateTime.Now,
        });

        _db.SaveChanges();
        return true;
    }
}
