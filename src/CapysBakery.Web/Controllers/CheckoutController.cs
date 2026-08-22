using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

// IMPORTANTE (ver capys-roles-especificacion.docx): antes de mostrar
// DatosCliente, el sistema debe validar si hay sesión iniciada. Como
// ASP.NET Core Identity todavía no está conectado (depende de la base de
// datos), esa validación real se agrega en Sprint 1. Por ahora esta
// pantalla es accesible directamente para poder probar el flujo completo.
public class CheckoutController : Controller
{
    // GET /Checkout/DatosCliente
    public IActionResult DatosCliente()
    {
        // TODO (Sprint 1): if (!User.Identity.IsAuthenticated)
        //     return RedirectToAction("Login", "Cuenta", new { returnUrl = "/Checkout/DatosCliente" });
        return View();
    }

    // GET /Checkout/Confirmacion
    public IActionResult Confirmacion()
    {
        return View();
    }
}
