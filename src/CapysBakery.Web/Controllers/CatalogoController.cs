using Microsoft.AspNetCore.Mvc;
using CapysBakery.Web.Services;

namespace CapysBakery.Web.Controllers;

public class CatalogoController : Controller
{
    private readonly IProductoRepository _productoRepository;

    public CatalogoController(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    // GET /Catalogo
    public IActionResult Index(string? categoria)
    {
        var productos = string.IsNullOrEmpty(categoria)
            ? _productoRepository.ObtenerTodos()
            : _productoRepository.ObtenerPorCategoria(categoria);

        ViewBag.Categorias = _productoRepository.ObtenerCategorias();
        ViewBag.CategoriaSeleccionada = categoria;

        return View(productos);
    }

    // GET /Catalogo/Detalle/3
    public IActionResult Detalle(int id)
    {
        var producto = _productoRepository.ObtenerPorId(id);

        if (producto is null)
        {
            return NotFound();
        }

        return View(producto);
    }
}
