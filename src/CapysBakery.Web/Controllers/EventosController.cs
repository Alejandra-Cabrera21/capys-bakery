using CapysBakery.Web.Models;
using CapysBakery.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

// Los paquetes de eventos ya no están fijos en la vista: ahora viven en la
// base de datos (tabla paquete_evento) y se administran desde /AdminEventos.
// Las 4 fotos de la galería siguen editándose desde /AdminContenido.
public class EventosController : Controller
{
    private readonly IContenidoSitioRepository _contenidoRepository;
    private readonly IEventoRepository _eventoRepository;

    public EventosController(
        IContenidoSitioRepository contenidoRepository,
        IEventoRepository eventoRepository)
    {
        _contenidoRepository = contenidoRepository;
        _eventoRepository = eventoRepository;
    }

    public IActionResult Index()
    {
        var imagenes = _contenidoRepository.ObtenerTodas();
        ViewBag.ImagenesGaleria = new[]
        {
            imagenes.GetValueOrDefault(ClavesContenidoSitio.EventosGaleria1),
            imagenes.GetValueOrDefault(ClavesContenidoSitio.EventosGaleria2),
            imagenes.GetValueOrDefault(ClavesContenidoSitio.EventosGaleria3),
            imagenes.GetValueOrDefault(ClavesContenidoSitio.EventosGaleria4),
        };

        // Solo los paquetes activos, ya ordenados por Orden.
        ViewBag.Paquetes = _eventoRepository.ObtenerActivos();

        return View();
    }
}
