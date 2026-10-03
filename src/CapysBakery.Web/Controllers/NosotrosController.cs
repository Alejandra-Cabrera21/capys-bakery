using CapysBakery.Web.Models;
using CapysBakery.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

// Contenido estático de marca (texto) — pero la imagen de "Todo empezó en
// una cocina" sí es editable desde /AdminContenido, sin tocar código.
public class NosotrosController : Controller
{
    private readonly IContenidoSitioRepository _contenidoRepository;

    public NosotrosController(IContenidoSitioRepository contenidoRepository)
    {
        _contenidoRepository = contenidoRepository;
    }

    public IActionResult Index()
    {
        ViewBag.ImagenHistoria = _contenidoRepository.ObtenerImagen(ClavesContenidoSitio.NosotrosHistoria);
        return View();
    }
}
