using CapysBakery.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

public class BlogController : Controller
{
    private readonly IPublicacionRepository _publicacionRepository;

    public BlogController(IPublicacionRepository publicacionRepository)
    {
        _publicacionRepository = publicacionRepository;
    }

    // GET /Blog
    public IActionResult Index()
    {
        var publicaciones = _publicacionRepository.ObtenerPublicadas();
        return View(publicaciones);
    }

    // GET /Blog/Detalle/3
    public IActionResult Detalle(int id)
    {
        var publicacion = _publicacionRepository.ObtenerPorId(id);
        if (publicacion is null || !publicacion.Publicada) return NotFound();
        return View(publicacion);
    }
}
