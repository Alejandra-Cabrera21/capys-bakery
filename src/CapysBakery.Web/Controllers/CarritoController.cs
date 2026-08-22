using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

// El carrito NO usa base de datos ni sesión de servidor: vive por completo
// en localStorage del navegador (ver wwwroot/js/carrito.js). Esta vista es
// solo el "cascarón" HTML; el JS la llena con lo que haya en localStorage.
// Esto es intencional: así el visitante puede armar su carrito sin cuenta,
// tal como se definió en la especificación de roles.
public class CarritoController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
