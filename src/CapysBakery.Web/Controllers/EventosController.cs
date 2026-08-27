using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

// TODO (futuro): los paquetes podrían moverse a la base de datos si el
// negocio empieza a cambiarlos seguido. Por ahora son fijos en la vista.
public class EventosController : Controller
{
    public IActionResult Index() => View();
}
