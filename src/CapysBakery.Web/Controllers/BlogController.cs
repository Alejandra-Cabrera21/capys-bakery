using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

// TODO (futuro): cuando exista la base de datos, esto pasa a tener una
// tabla de Publicaciones real en vez de contenido fijo en la vista.
public class BlogController : Controller
{
    public IActionResult Index() => View();
}
