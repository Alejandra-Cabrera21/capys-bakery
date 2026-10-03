using CapysBakery.Web.Models;

namespace CapysBakery.Web.Services;

public interface IContenidoSitioRepository
{
    // Devuelve la URL de la imagen para esa clave, o null si nunca se ha
    // subido nada — en ese caso la vista debe mostrar el ícono/placeholder
    // de diseño original, no romperse.
    string? ObtenerImagen(string clave);

    // Todas las claves conocidas con su imagen actual (o null), para
    // pintar el panel de AdminContenido completo de una sola vez.
    Dictionary<string, string?> ObtenerTodas();

    // Crea la fila si la clave no existía, o actualiza la imagen si ya
    // existía (upsert) — el panel de administración no necesita saber
    // cuál de los 2 casos es.
    void ActualizarImagen(string clave, string urlImagen);

    // Quita la imagen de esa clave (vuelve a null) — la vista pública
    // correspondiente entonces muestra su ícono de marcador de diseño
    // original, igual que si nunca se hubiera subido nada.
    void EliminarImagen(string clave);
}
