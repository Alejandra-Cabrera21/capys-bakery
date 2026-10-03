using CapysBakery.Web.Models;

namespace CapysBakery.Web.Services;

// Mismo patrón que IProductoRepository / IContenidoSitioRepository: el
// controlador solo conoce esta interfaz, EfEventoRepository la implementa
// contra CapysBakeryDb.
public interface IEventoRepository
{
    // Para /AdminEventos — incluye paquetes inactivos (ocultos).
    List<PaqueteEvento> ObtenerTodos();

    // Para /Eventos (público) — solo los activos, ya ordenados.
    List<PaqueteEvento> ObtenerActivos();

    PaqueteEvento? ObtenerPorId(int id);

    void Crear(PaqueteEvento paquete);
    void Actualizar(PaqueteEvento paquete);
    void Eliminar(int id);
    void CambiarActivo(int id, bool activo);
}
