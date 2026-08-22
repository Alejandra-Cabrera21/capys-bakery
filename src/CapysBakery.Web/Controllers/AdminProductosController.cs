using CapysBakery.Web.Models;
using CapysBakery.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

// Permite publicar nuevos productos desde la página (Dueño o Administrador
// /vendedor, según la tabla de permisos: "Gestionar catálogo" = Sí para
// ambos). El producto —incluida la imagen— queda guardado en el
// repositorio de productos (hoy en memoria; el día que exista SQL Server,
// IProductoRepository.Agregar/Actualizar hacen el INSERT/UPDATE real y
// este controlador no cambia).
[Authorize(Roles = "Administrador,Dueño")]
public class AdminProductosController : Controller
{
    // Carpeta pública donde quedan las imágenes subidas. wwwroot ya se
    // sirve como estático (ver Program.cs -> app.UseStaticFiles()).
    private const string CarpetaImagenes = "img/productos";

    private readonly IProductoRepository _productoRepository;
    private readonly IWebHostEnvironment _entorno;

    public AdminProductosController(IProductoRepository productoRepository, IWebHostEnvironment entorno)
    {
        _productoRepository = productoRepository;
        _entorno = entorno;
    }

    // GET /AdminProductos
    public IActionResult Index()
    {
        var productos = _productoRepository.ObtenerTodosIncluyendoNoDisponibles()
            .OrderByDescending(p => p.FechaCreacion)
            .ToList();
        return View(productos);
    }

    // GET /AdminProductos/Crear
    public IActionResult Crear()
    {
        ViewBag.Categorias = _productoRepository.ObtenerCategorias();
        ViewBag.EsEdicion = false;
        return View("Formulario", new Producto { Disponible = true });
    }

    // POST /AdminProductos/Crear
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Producto producto, IFormFile? imagen, string? alergenosTexto)
    {
        if (string.IsNullOrWhiteSpace(producto.Nombre) || string.IsNullOrWhiteSpace(producto.Categoria))
        {
            ModelState.AddModelError(string.Empty, "Nombre y categoría son obligatorios.");
            ViewBag.Categorias = _productoRepository.ObtenerCategorias();
            ViewBag.EsEdicion = false;
            return View("Formulario", producto);
        }

        if (!producto.Presentaciones.Any())
        {
            ModelState.AddModelError(string.Empty, "Agrega al menos una presentación con su precio.");
            ViewBag.Categorias = _productoRepository.ObtenerCategorias();
            ViewBag.EsEdicion = false;
            return View("Formulario", producto);
        }

        // El "precio desde" mostrado en catálogo/inicio es el de la
        // presentación más económica, igual que documenta el diseño de BD.
        producto.Precio = producto.Presentaciones.Min(p => p.Precio);
        producto.Alergenos = DividirAlergenos(alergenosTexto);
        producto.CreadoPorCorreo = User.Identity?.Name;

        if (imagen is not null && imagen.Length > 0)
        {
            producto.ImagenUrl = await GuardarImagenAsync(imagen);
        }

        _productoRepository.Agregar(producto);
        TempData["Mensaje"] = $"“{producto.Nombre}” se publicó correctamente en el catálogo.";
        return RedirectToAction(nameof(Index));
    }

    // GET /AdminProductos/Editar/3
    public IActionResult Editar(int id)
    {
        var producto = _productoRepository.ObtenerPorId(id);
        if (producto is null) return NotFound();

        ViewBag.Categorias = _productoRepository.ObtenerCategorias();
        ViewBag.EsEdicion = true;
        return View("Formulario", producto);
    }

    // POST /AdminProductos/Editar/3
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Producto producto, IFormFile? imagen, string? alergenosTexto)
    {
        producto.Id = id;

        if (string.IsNullOrWhiteSpace(producto.Nombre) || !producto.Presentaciones.Any())
        {
            ModelState.AddModelError(string.Empty, "Revisa el nombre y que exista al menos una presentación.");
            ViewBag.Categorias = _productoRepository.ObtenerCategorias();
            ViewBag.EsEdicion = true;
            return View("Formulario", producto);
        }

        producto.Precio = producto.Presentaciones.Min(p => p.Precio);
        producto.Alergenos = DividirAlergenos(alergenosTexto);

        if (imagen is not null && imagen.Length > 0)
        {
            producto.ImagenUrl = await GuardarImagenAsync(imagen);
        }

        var actualizado = _productoRepository.Actualizar(producto);
        if (!actualizado) return NotFound();

        TempData["Mensaje"] = $"“{producto.Nombre}” se actualizó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // POST /AdminProductos/CambiarDisponibilidad/3 — ocultar/mostrar sin
    // borrar, igual que la columna "disponible" del diseño de BD.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CambiarDisponibilidad(int id)
    {
        var producto = _productoRepository.ObtenerPorId(id);
        if (producto is null) return NotFound();

        producto.Disponible = !producto.Disponible;
        _productoRepository.Actualizar(producto);
        return RedirectToAction(nameof(Index));
    }

    private List<string> DividirAlergenos(string? texto) =>
        (texto ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

    private async Task<string> GuardarImagenAsync(IFormFile imagen)
    {
        var carpetaFisica = Path.Combine(_entorno.WebRootPath, CarpetaImagenes);
        Directory.CreateDirectory(carpetaFisica);

        var extension = Path.GetExtension(imagen.FileName);
        var nombreArchivo = $"{Guid.NewGuid()}{extension}";
        var rutaFisica = Path.Combine(carpetaFisica, nombreArchivo);

        await using var flujo = new FileStream(rutaFisica, FileMode.Create);
        await imagen.CopyToAsync(flujo);

        // TODO (BD real / almacenamiento en la nube): esto guarda el
        // archivo en el disco del servidor y su ruta relativa en
        // Producto.ImagenUrl, tal como documenta imagen_producto.url_imagen
        // en el diseño de base de datos. Si el hosting no persiste disco
        // (ej. algunos PaaS), esto debe migrar a un proveedor de storage.
        return $"/{CarpetaImagenes}/{nombreArchivo}";
    }
}
