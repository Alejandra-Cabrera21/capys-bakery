using CapysBakery.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

// Permite al Dueño o Administrador elegir qué calificaciones de clientes
// (con comentario) se muestran como testimonios reales en el Home, en vez
// de los 3 comentarios fijos de ejemplo que traía el diseño original.
[Authorize(Roles = "Administrador,Dueño")]
public class AdminTestimoniosController : Controller
{
    private readonly IPedidoRepository _pedidoRepository;

    public AdminTestimoniosController(IPedidoRepository pedidoRepository)
    {
        _pedidoRepository = pedidoRepository;
    }

    // GET /AdminTestimonios
    public IActionResult Index()
    {
        var calificaciones = _pedidoRepository.ObtenerCalificacionesConComentario();
        return View(calificaciones);
    }

    // POST /AdminTestimonios/CambiarDestacado
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CambiarDestacado(int id, bool destacado)
    {
        _pedidoRepository.MarcarComoDestacada(id, destacado);
        return RedirectToAction(nameof(Index));
    }
}
