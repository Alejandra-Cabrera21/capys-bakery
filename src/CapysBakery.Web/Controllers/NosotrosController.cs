using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

// Contenido estático de marca — no depende de base de datos.
public class NosotrosController : Controller
{
    public IActionResult Index() => View();
}
