using System.Security.Claims;
using CapysBakery.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

public class BlogController : Controller
{
    private readonly IPublicacionRepository _publicacionRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public BlogController(IPublicacionRepository publicacionRepository, IUsuarioRepository usuarioRepository)
    {
        _publicacionRepository = publicacionRepository;
        _usuarioRepository = usuarioRepository;
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

        // Se resuelve el nombre del autor a partir del correo guardado en
        // la publicación (AutorCorreo), para mostrar algo más amigable que
        // un correo en la vista pública.
        ViewBag.NombreAutor = string.IsNullOrEmpty(publicacion.AutorCorreo)
            ? null
            : _usuarioRepository.ObtenerPorCorreo(publicacion.AutorCorreo)?.Nombre;

        ViewBag.Comentarios = _publicacionRepository.ObtenerComentarios(id);
        return View(publicacion);
    }

    // POST /Blog/Comentar
    // Cualquier usuario con sesión iniciada (Cliente, Administrador o
    // Dueño) puede comentar en una entrada — "publicar" sigue siendo solo
    // de Administrador/Dueño (ver AdminBlogController), pero comentar es
    // la forma en que el Cliente sí participa en el blog.
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public IActionResult Comentar(int publicacionId, string contenido)
    {
        if (string.IsNullOrWhiteSpace(contenido))
        {
            return RedirectToAction(nameof(Detalle), new { id = publicacionId });
        }

        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        _publicacionRepository.AgregarComentario(publicacionId, usuarioId, contenido.Trim());

        return RedirectToAction(nameof(Detalle), new { id = publicacionId });
    }

    // POST /Blog/EliminarComentario — moderación: solo Administrador/Dueño
    // pueden borrar un comentario inapropiado.
    [HttpPost]
    [Authorize(Roles = "Administrador,Dueño")]
    [ValidateAntiForgeryToken]
    public IActionResult EliminarComentario(int comentarioId, int publicacionId)
    {
        _publicacionRepository.EliminarComentario(comentarioId);
        return RedirectToAction(nameof(Detalle), new { id = publicacionId });
    }
}
