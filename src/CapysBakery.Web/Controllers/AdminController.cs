using CapysBakery.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

// Protegido según capys-roles-especificacion.docx: "Cambiar estado de
// pedidos" está permitido a Administrador (vendedor) y Dueño, pero no a
// Cliente ni Visitante.
[Authorize(Roles = "Administrador,Dueño")]
public class AdminController : Controller
{
    private readonly IPedidoRepository _pedidoRepository;

    public AdminController(IPedidoRepository pedidoRepository)
    {
        _pedidoRepository = pedidoRepository;
    }

    // GET /Admin/Pedidos
    // Fase 6: ya lee de verdad de la base de datos (antes leía localStorage
    // del navegador) — Dueño y Vendedor ahora sí ven TODOS los pedidos, sin
    // importar desde qué dispositivo se hicieron.
    public IActionResult Pedidos()
    {
        var pedidos = _pedidoRepository.ObtenerTodos();
        ViewBag.Estados = _pedidoRepository.ObtenerEstados();
        return View(pedidos);
    }

    // POST /Admin/CambiarEstadoPedido
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CambiarEstadoPedido(int id, int nuevoEstadoId)
    {
        var actualizado = _pedidoRepository.CambiarEstado(id, nuevoEstadoId);
        if (!actualizado) return NotFound();

        return RedirectToAction(nameof(Pedidos));
    }
}
