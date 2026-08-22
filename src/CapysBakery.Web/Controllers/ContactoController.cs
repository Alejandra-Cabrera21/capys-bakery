using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

public class ContactoController : Controller
{
    public IActionResult Index() => View();

    // TODO (Sprint con formulario funcional): guardar el mensaje o
    // enviarlo por correo. Por ahora solo se muestra la interfaz.
    [HttpPost]
    public IActionResult Index(string nombre, string correo, string telefono, string mensaje)
    {
        ViewBag.Enviado = true;
        return View();
    }
}
