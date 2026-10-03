using CapysBakery.Web.Models;
using CapysBakery.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

// Mismo patrón/autorización que AdminProductos, AdminContenido y
// AdminTestimonios: solo Administrador (vendedor) o Dueño.
[Authorize(Roles = "Administrador,Dueño")]
public class AdminEventosController : Controller
{
    private readonly IEventoRepository _eventoRepository;

    public AdminEventosController(IEventoRepository eventoRepository)
    {
        _eventoRepository = eventoRepository;
    }

    // GET /AdminEventos
    public IActionResult Index()
    {
        var paquetes = _eventoRepository.ObtenerTodos();
        return View(paquetes);
    }

    // GET /AdminEventos/Crear
    public IActionResult Crear()
    {
        var nuevo = new PaqueteEvento { Activo = true, Orden = 0 };
        return View("Formulario", nuevo);
    }

    // POST /AdminEventos/Crear
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(PaqueteEvento paquete)
    {
        if (!ModelState.IsValid)
        {
            return View("Formulario", paquete);
        }

        _eventoRepository.Crear(paquete);
        TempData["Mensaje"] = "Paquete creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // GET /AdminEventos/Editar/5
    public IActionResult Editar(int id)
    {
        var paquete = _eventoRepository.ObtenerPorId(id);
        if (paquete is null) return NotFound();

        return View("Formulario", paquete);
    }

    // POST /AdminEventos/Editar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(PaqueteEvento paquete)
    {
        if (!ModelState.IsValid)
        {
            return View("Formulario", paquete);
        }

        _eventoRepository.Actualizar(paquete);
        TempData["Mensaje"] = "Paquete actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // POST /AdminEventos/Eliminar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Eliminar(int id)
    {
        _eventoRepository.Eliminar(id);
        TempData["Mensaje"] = "Paquete eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // POST /AdminEventos/CambiarActivo — mostrar/ocultar un paquete sin
    // borrarlo (ej. temporada baja) sin perder su información.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CambiarActivo(int id, bool activo)
    {
        _eventoRepository.CambiarActivo(id, activo);
        TempData["Mensaje"] = activo ? "Paquete mostrado nuevamente." : "Paquete ocultado.";
        return RedirectToAction(nameof(Index));
    }
}
