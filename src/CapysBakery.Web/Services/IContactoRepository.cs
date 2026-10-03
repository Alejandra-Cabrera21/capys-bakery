using CapysBakery.Web.Models;

namespace CapysBakery.Web.Services;

// Igual patrón que el resto del proyecto: el controlador solo conoce esta
// interfaz. EfContactoRepository la implementa contra CapysBakeryDb.
public interface IContactoRepository
{
    void Guardar(MensajeContacto mensaje);

    // Usado por el panel de administración (Dueño o Administrador) para
    // revisar los mensajes recibidos desde /Contacto.
    List<MensajeContacto> ObtenerTodos();
    void MarcarComoLeido(int id);
}
