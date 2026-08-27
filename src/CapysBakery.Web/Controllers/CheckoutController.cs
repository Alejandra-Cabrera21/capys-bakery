using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

// Según capys-roles-especificacion.docx, sección 2: la sesión NUNCA se
// valida antes de que el visitante haga clic en "Finalizar pedido". El
// carrito se arma libremente en localStorage sin cuenta. Por eso la
// validación vive aquí (al entrar a DatosCliente) y no en Carrito.
public class CheckoutController : Controller
{
    // GET /Checkout/DatosCliente
    [Authorize]
    public IActionResult DatosCliente()
    {
        // [Authorize] ya se encarga de redirigir a /Cuenta/Login con
        // ?ReturnUrl=/Checkout/DatosCliente cuando no hay sesión, gracias a
        // options.LoginPath configurado en Program.cs. Al iniciar sesión o
        // registrarse, CuentaController regresa aquí mismo — el carrito en
        // localStorage no se pierde en el camino.
        ViewBag.NombreUsuario = User.Identity?.Name;
        ViewBag.CorreoUsuario = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        return View();
    }

    // GET /Checkout/Confirmacion
    public IActionResult Confirmacion()
    {
        return View();
    }
}
