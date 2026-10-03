using CapysBakery.Web.Models;
using CapysBakery.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

public class ContactoController : Controller
{
    // Mismo número que ya usa checkout.js para el pago por WhatsApp — es
    // el mismo canal de atención del negocio (ver pie de página / sección
    // "Teléfono / WhatsApp" de esta misma página).
    private const string NumeroWhatsApp = "50248036717";

    private readonly IContactoRepository _contactoRepository;

    public ContactoController(IContactoRepository contactoRepository)
    {
        _contactoRepository = contactoRepository;
    }

    public IActionResult Index() => View();

    // El mensaje se guarda en mensaje_contacto (para que el Dueño/
    // Administrador lo vea en /AdminMensajes) Y, además, se abre WhatsApp
    // con el mensaje ya redactado — así el negocio se entera al instante,
    // sin depender de que alguien revise el panel a diario.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(string nombre, string correo, string telefono, string tipo, string mensaje)
    {
        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(mensaje))
        {
            ModelState.AddModelError(string.Empty, "Nombre, correo y mensaje son obligatorios.");
            ViewBag.Enviado = false;
            return View();
        }

        var tipoConsulta = string.IsNullOrWhiteSpace(tipo) ? "Otro" : tipo;

        _contactoRepository.Guardar(new MensajeContacto
        {
            Nombre = nombre,
            Correo = correo,
            Telefono = telefono,
            TipoConsulta = tipoConsulta,
            Mensaje = mensaje,
        });

        var textoWhatsApp =
            $"Hola, soy {nombre}.\n" +
            $"Tipo de consulta: {tipoConsulta}\n" +
            $"Correo: {correo}\n" +
            (string.IsNullOrWhiteSpace(telefono) ? "" : $"Teléfono: {telefono}\n") +
            $"\nMensaje: {mensaje}";

        ViewBag.UrlWhatsApp = $"https://wa.me/{NumeroWhatsApp}?text={Uri.EscapeDataString(textoWhatsApp)}";
        ViewBag.Enviado = true;
        return View();
    }
}
