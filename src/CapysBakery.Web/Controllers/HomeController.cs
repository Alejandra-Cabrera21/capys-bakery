using Microsoft.AspNetCore.Mvc;
using CapysBakery.Web.Services;

namespace CapysBakery.Web.Controllers;

public class HomeController : Controller
{
    private readonly IProductoRepository _productoRepository;

    // ASP.NET Core inyecta automáticamente la implementación registrada
    // en Program.cs (hoy MockProductoRepository, mañana la real con EF Core).
    public HomeController(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public IActionResult Index()
    {
        var productosDestacados = _productoRepository.ObtenerDestacados(4);
        ViewBag.Promociones = _productoRepository.ObtenerPromociones();
        return View(productosDestacados);
    }

    public IActionResult Error()
    {
        return View();
    }
}
