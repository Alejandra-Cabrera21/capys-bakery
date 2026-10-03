using Microsoft.AspNetCore.Mvc;
using CapysBakery.Web.Services;
using CapysBakery.Web.Models;

namespace CapysBakery.Web.Controllers;

public class HomeController : Controller
{
    private readonly IProductoRepository _productoRepository;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IContenidoSitioRepository _contenidoRepository;

    // ASP.NET Core inyecta automáticamente la implementación registrada
    // en Program.cs (hoy MockProductoRepository, mañana la real con EF Core).
    public HomeController(
        IProductoRepository productoRepository,
        IPedidoRepository pedidoRepository,
        IContenidoSitioRepository contenidoRepository)
    {
        _productoRepository = productoRepository;
        _pedidoRepository = pedidoRepository;
        _contenidoRepository = contenidoRepository;
    }

    public IActionResult Index()
    {
        var productosDestacados = _productoRepository.ObtenerDestacados(4);
        ViewBag.Promociones = _productoRepository.ObtenerPromociones();

        // Reemplaza el "4.9★" fijo del diseño original: se calcula en
        // tiempo real a partir de las calificaciones que los clientes
        // dejan en /Cuenta/MisPedidos una vez que su pedido está
        // "Entregado" (ver ObtenerEstadisticasCalificacion).
        var (promedio, total) = _pedidoRepository.ObtenerEstadisticasCalificacion();
        ViewBag.CalificacionPromedio = promedio;
        ViewBag.TotalCalificaciones = total;

        // Imágenes editables desde /AdminContenido — null mientras el
        // Dueño no haya subido nada todavía, en cuyo caso la vista usa el
        // ícono de marcador del diseño original.
        ViewBag.ImagenHero = _contenidoRepository.ObtenerImagen(ClavesContenidoSitio.HomeHero);
        ViewBag.ImagenHistoria = _contenidoRepository.ObtenerImagen(ClavesContenidoSitio.HomeHistoria);

        // Testimonios reales que el Dueño/Administrador destacó desde
        // /AdminTestimonios — reemplazan los 3 comentarios fijos de
        // ejemplo que traía el diseño original.
        ViewBag.Testimonios = _pedidoRepository.ObtenerCalificacionesDestacadas(3);

        return View(productosDestacados);
    }

    public IActionResult Error()
    {
        return View();
    }
}
