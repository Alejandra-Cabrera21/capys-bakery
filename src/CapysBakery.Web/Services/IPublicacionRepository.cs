using CapysBakery.Web.Models;

namespace CapysBakery.Web.Services;

// Mismo patrón que IProductoRepository: los controladores solo conocen
// este contrato. Hoy la implementa MockPublicacionRepository (en memoria).
// Cuando exista Entity Framework, se reemplaza por una implementación real
// sin tocar BlogController ni AdminBlogController.
public interface IPublicacionRepository
{
    // Para el blog público: solo lo publicado, más reciente primero.
    List<Publicacion> ObtenerPublicadas();

    // Para el panel de administración: incluye borradores/ocultas.
    List<Publicacion> ObtenerTodas();

    Publicacion? ObtenerPorId(int id);

    Publicacion Agregar(Publicacion publicacion);
    bool Actualizar(Publicacion publicacion);
}
