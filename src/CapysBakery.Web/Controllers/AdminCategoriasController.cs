using CapysBakery.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

// El cliente confirmó en "Análisis funcional de las categorías" que
// necesita poder crear, modificar y desactivar categorías (ej. una
// "Ediciones especiales" para Navidad, desactivada el resto del año).
// Mismo par de roles que gestiona el catálogo de productos.
[Authorize(Roles = "Administrador,Dueño")]
public class AdminCategoriasController : Controller
{
    private readonly IProductoRepository _productoRepository;

    public AdminCategoriasController(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    // GET /AdminCategorias
    public IActionResult Index()
    {
        return View(_productoRepository.ObtenerTodasLasCategorias());
    }

    // GET /AdminCategorias/Crear
    public IActionResult Crear() => View();

    // POST /AdminCategorias/Crear
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            ModelState.AddModelError(string.Empty, "El nombre de la categoría es obligatorio.");
            return View();
        }

        _productoRepository.CrearCategoria(nombre.Trim());
        TempData["Mensaje"] = $"Categoría “{nombre}” creada.";
        return RedirectToAction(nameof(Index));
    }

    // GET /AdminCategorias/Editar/3
    public IActionResult Editar(int id)
    {
        var categoria = _productoRepository.ObtenerCategoriaPorId(id);
        if (categoria is null) return NotFound();
        return View(categoria);
    }

    // POST /AdminCategorias/Editar/3
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(int id, string nombre, bool disponible)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            ModelState.AddModelError(string.Empty, "El nombre de la categoría es obligatorio.");
            return View(_productoRepository.ObtenerCategoriaPorId(id));
        }

        var actualizado = _productoRepository.ActualizarCategoria(id, nombre.Trim(), disponible);
        if (!actualizado) return NotFound();

        TempData["Mensaje"] = $"Categoría “{nombre}” actualizada.";
        return RedirectToAction(nameof(Index));
    }

    // POST /AdminCategorias/CambiarDisponibilidad/3 — activar/desactivar
    // sin borrar, tal como pidió el cliente (ej. "Ediciones especiales"
    // que se desactiva cuando termina la temporada).
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CambiarDisponibilidad(int id)
    {
        var categoria = _productoRepository.ObtenerCategoriaPorId(id);
        if (categoria is null) return NotFound();

        _productoRepository.ActualizarCategoria(id, categoria.Nombre, !categoria.Disponible);
        return RedirectToAction(nameof(Index));
    }
}
