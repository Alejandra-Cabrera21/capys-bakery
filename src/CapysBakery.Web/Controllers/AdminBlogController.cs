using CapysBakery.Web.Models;
using CapysBakery.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

// "Publicar en el blog" está permitido a Administrador (vendedor) y Dueño
// según capys-roles-especificacion.docx — mismo par de roles que gestiona
// el catálogo de productos.
[Authorize(Roles = "Administrador,Dueño")]
public class AdminBlogController : Controller
{
    private const string CarpetaImagenes = "img/blog";

    private readonly IPublicacionRepository _publicacionRepository;
    private readonly IWebHostEnvironment _entorno;

    public AdminBlogController(IPublicacionRepository publicacionRepository, IWebHostEnvironment entorno)
    {
        _publicacionRepository = publicacionRepository;
        _entorno = entorno;
    }

    // GET /AdminBlog
    public IActionResult Index()
    {
        return View(_publicacionRepository.ObtenerTodas());
    }

    // GET /AdminBlog/Crear
    public IActionResult Crear()
    {
        ViewBag.EsEdicion = false;
        return View("Formulario", new Publicacion { Publicada = true });
    }

    // POST /AdminBlog/Crear
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Publicacion publicacion, IFormFile? imagen)
    {
        if (string.IsNullOrWhiteSpace(publicacion.Titulo) || string.IsNullOrWhiteSpace(publicacion.Contenido))
        {
            ModelState.AddModelError(string.Empty, "Título y contenido son obligatorios.");
            ViewBag.EsEdicion = false;
            return View("Formulario", publicacion);
        }

        publicacion.AutorCorreo = User.Identity?.Name;

        if (imagen is not null && imagen.Length > 0)
        {
            publicacion.ImagenUrl = await GuardarImagenAsync(imagen);
        }

        _publicacionRepository.Agregar(publicacion);
        TempData["Mensaje"] = $"“{publicacion.Titulo}” se publicó correctamente en el blog.";
        return RedirectToAction(nameof(Index));
    }

    // GET /AdminBlog/Editar/3
    public IActionResult Editar(int id)
    {
        var publicacion = _publicacionRepository.ObtenerPorId(id);
        if (publicacion is null) return NotFound();

        ViewBag.EsEdicion = true;
        return View("Formulario", publicacion);
    }

    // POST /AdminBlog/Editar/3
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Publicacion publicacion, IFormFile? imagen)
    {
        publicacion.Id = id;

        if (string.IsNullOrWhiteSpace(publicacion.Titulo) || string.IsNullOrWhiteSpace(publicacion.Contenido))
        {
            ModelState.AddModelError(string.Empty, "Título y contenido son obligatorios.");
            ViewBag.EsEdicion = true;
            return View("Formulario", publicacion);
        }

        if (imagen is not null && imagen.Length > 0)
        {
            publicacion.ImagenUrl = await GuardarImagenAsync(imagen);
        }

        var actualizado = _publicacionRepository.Actualizar(publicacion);
        if (!actualizado) return NotFound();

        TempData["Mensaje"] = $"“{publicacion.Titulo}” se actualizó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // POST /AdminBlog/CambiarPublicada/3 — retira/vuelve a mostrar una
    // entrada del blog público sin borrarla.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CambiarPublicada(int id)
    {
        var publicacion = _publicacionRepository.ObtenerPorId(id);
        if (publicacion is null) return NotFound();

        publicacion.Publicada = !publicacion.Publicada;
        _publicacionRepository.Actualizar(publicacion);
        return RedirectToAction(nameof(Index));
    }

    private async Task<string> GuardarImagenAsync(IFormFile imagen)
    {
        var carpetaFisica = Path.Combine(_entorno.WebRootPath, CarpetaImagenes);
        Directory.CreateDirectory(carpetaFisica);

        var extension = Path.GetExtension(imagen.FileName);
        var nombreArchivo = $"{Guid.NewGuid()}{extension}";
        var rutaFisica = Path.Combine(carpetaFisica, nombreArchivo);

        await using var flujo = new FileStream(rutaFisica, FileMode.Create);
        await imagen.CopyToAsync(flujo);

        return $"/{CarpetaImagenes}/{nombreArchivo}";
    }
}
