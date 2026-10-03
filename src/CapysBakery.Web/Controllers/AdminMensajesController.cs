using CapysBakery.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

// Permite al Dueño o Administrador ver los mensajes que los visitantes
// envían desde el formulario público de /Contacto.
[Authorize(Roles = "Administrador,Dueño")]
public class AdminMensajesController : Controller
{
    private readonly IContactoRepository _contactoRepository;

    public AdminMensajesController(IContactoRepository contactoRepository)
    {
        _contactoRepository = contactoRepository;
    }

    // GET /AdminMensajes
    public IActionResult Index()
    {
        var mensajes = _contactoRepository.ObtenerTodos();
        return View(mensajes);
    }

    // POST /AdminMensajes/MarcarLeido/3
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult MarcarLeido(int id)
    {
        _contactoRepository.MarcarComoLeido(id);
        return RedirectToAction(nameof(Index));
    }
}
