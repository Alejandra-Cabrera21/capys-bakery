namespace CapysBakery.Web.Models;

// Corresponde a los mensajes que un visitante envía desde /Contacto. No
// forma parte del diseño original de 18 tablas — es una extensión
// pragmática para que el formulario sea funcional de verdad (antes solo
// mostraba una confirmación visual sin guardar nada, ver
// ContactoController.cs original).
public class MensajeContacto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string TipoConsulta { get; set; } = "Otro";
    public string Mensaje { get; set; } = string.Empty;
    public DateTime FechaEnvio { get; set; } = DateTime.Now;

    // Permite al Dueño/Administrador distinguir de un vistazo qué mensajes
    // ya revisó, en el panel de AdminMensajes.
    public bool Leido { get; set; } = false;
}
