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
            .Include(p => p.Historial)
            .Include(p => p.Calificacion);

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
        // Se recorta a 20 caracteres del GUID (+ "TEMP-" = 25 en total)
        // para que quepa dentro del límite de 30 caracteres de la columna
        // codigo_pedido (antes se usaba el GUID completo, 37 caracteres,
        // lo que causaba un error de truncamiento al guardar).
        pedido.CodigoPedido = $"TEMP-{Guid.NewGuid():N}"[..25];

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

    // Solo se permite calificar un pedido "Entregado" que aún no tenga
    // calificación — se valida aquí (no solo en el formulario), porque
    // este método puede recibir llamadas directas que se salten la interfaz.
    public bool Calificar(int pedidoId, int estrellas, string? comentario)
    {
        if (estrellas < 1 || estrellas > 5) return false;

        var pedido = _db.Pedidos
            .Include(p => p.EstadoPedido)
            .Include(p => p.Calificacion)
            .FirstOrDefault(p => p.Id == pedidoId);

        if (pedido is null) return false;
        if (pedido.EstadoPedido?.Nombre != EstadosPedido.Entregado) return false;
        if (pedido.Calificacion is not null) return false; // ya calificado, no se sobreescribe

        _db.CalificacionesPedido.Add(new CalificacionPedido
        {
            PedidoId = pedidoId,
            Estrellas = estrellas,
            Comentario = comentario,
        });

        _db.SaveChanges();
        return true;
    }

    public (double Promedio, int Total) ObtenerEstadisticasCalificacion()
    {
        var estrellas = _db.CalificacionesPedido.Select(c => c.Estrellas).ToList();
        return estrellas.Count == 0 ? (0, 0) : (estrellas.Average(), estrellas.Count);
    }

    public List<CalificacionPedido> ObtenerCalificacionesConComentario() =>
        _db.CalificacionesPedido
            .Include(c => c.Pedido)
            .Where(c => c.Comentario != null && c.Comentario != "")
            .OrderByDescending(c => c.FechaCalificacion)
            .ToList();

    public bool MarcarComoDestacada(int calificacionId, bool destacado)
    {
        var calificacion = _db.CalificacionesPedido.FirstOrDefault(c => c.Id == calificacionId);
        if (calificacion is null) return false;

        calificacion.Destacado = destacado;
        _db.SaveChanges();
        return true;
    }

    public List<CalificacionPedido> ObtenerCalificacionesDestacadas(int cantidad) =>
        _db.CalificacionesPedido
            .Include(c => c.Pedido)
            .Where(c => c.Destacado && c.Comentario != null && c.Comentario != "")
            .OrderByDescending(c => c.FechaCalificacion)
            .Take(cantidad)
            .ToList();
}
