using CapysBakery.Web.Models;
using CapysBakery.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

// Permite al Dueño o Administrador reemplazar imágenes de contenido
// estático del sitio (hero del Home, imagen de Nosotros, galería de
// Eventos) sin tener que tocar código.
[Authorize(Roles = "Administrador,Dueño")]
public class AdminContenidoController : Controller
{
    private const string CarpetaImagenes = "img/sitio";

    private readonly IContenidoSitioRepository _contenidoRepository;
    private readonly IWebHostEnvironment _entorno;

    public AdminContenidoController(IContenidoSitioRepository contenidoRepository, IWebHostEnvironment entorno)
    {
        _contenidoRepository = contenidoRepository;
        _entorno = entorno;
    }

    // GET /AdminContenido
    public IActionResult Index()
    {
        var imagenesActuales = _contenidoRepository.ObtenerTodas();

        var slots = ClavesContenidoSitio.Todas
            .Select(s => new SlotContenidoVm
            {
                Clave = s.Clave,
                Etiqueta = s.Etiqueta,
                UrlImagenActual = imagenesActuales.TryGetValue(s.Clave, out var url) ? url : null,
            })
            .ToList();

        return View(slots);
    }

    // POST /AdminContenido/Actualizar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Actualizar(string clave, IFormFile imagen)
    {
        if (imagen is null || imagen.Length == 0)
        {
            TempData["Mensaje"] = "Selecciona un archivo de imagen antes de subir.";
            return RedirectToAction(nameof(Index));
        }

        var carpetaFisica = Path.Combine(_entorno.WebRootPath, CarpetaImagenes);
        Directory.CreateDirectory(carpetaFisica);

        var extension = Path.GetExtension(imagen.FileName);
        var nombreArchivo = $"{Guid.NewGuid()}{extension}";
        var rutaFisica = Path.Combine(carpetaFisica, nombreArchivo);

        await using (var flujo = new FileStream(rutaFisica, FileMode.Create))
        {
            await imagen.CopyToAsync(flujo);
        }

        var urlRelativa = $"/{CarpetaImagenes}/{nombreArchivo}";
        _contenidoRepository.ActualizarImagen(clave, urlRelativa);

        TempData["Mensaje"] = "Imagen actualizada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // POST /AdminContenido/Eliminar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Eliminar(string clave)
    {
        _contenidoRepository.EliminarImagen(clave);
        TempData["Mensaje"] = "Imagen eliminada correctamente.";
        return RedirectToAction(nameof(Index));
    }
}

// Solo para pintar la vista — no es una entidad de base de datos.
public class SlotContenidoVm
{
    public string Clave { get; set; } = string.Empty;
    public string Etiqueta { get; set; } = string.Empty;
    public string? UrlImagenActual { get; set; }
}
