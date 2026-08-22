using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

// Autenticación real (ASP.NET Core Identity) depende de que exista la
// base de datos, ya que Identity guarda los usuarios en SQL Server.
// Mientras tanto, estas vistas muestran la interfaz final para que el
// equipo pueda revisar el diseño y flujo, pero el formulario todavía
// no crea ni valida cuentas de verdad.
public class CuentaController : Controller
{
    // GET /Cuenta/Login
    public IActionResult Login(string? returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    // GET /Cuenta/Registro
    public IActionResult Registro(string? returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    // TODO (cuando exista Identity + base de datos):
    // [HttpPost] public async Task<IActionResult> Login(LoginViewModel modelo) { ... }
    // [HttpPost] public async Task<IActionResult> Registro(RegistroViewModel modelo) { ... }
    // Ambos deben respetar returnUrl para regresar al carrito/checkout
    // desde donde vino el visitante, según la especificación de roles.
}
