using CapysBakery.Web.Models;
using CapysBakery.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

// Permite publicar nuevos productos desde la página (Dueño o Administrador
// /vendedor, según la tabla de permisos: "Gestionar catálogo" = Sí para
// ambos). El producto —incluidas las imágenes— queda guardado en el
// repositorio de productos.
[Authorize(Roles = "Administrador,Dueño")]
public class AdminProductosController : Controller
{
    // Carpeta pública donde quedan las imágenes subidas. wwwroot ya se
    // sirve como estático (ver Program.cs -> app.UseStaticFiles()).
    private const string CarpetaImagenes = "img/productos";

    // Máximo de imágenes por producto (1 principal + 4 adicionales). El
    // documento del cliente solo pedía 2, pero el equipo decidió ampliarlo
    // a 5 — el modelo de datos (imagen_producto) ya lo soportaba sin
    // cambios de esquema.
    private const int MaxImagenes = 5;

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
        ViewBag.TiposPersonalizacion = _productoRepository.ObtenerTiposPersonalizacion();
        ViewBag.EsEdicion = false;
        return View("Formulario", new Producto { Disponible = true });
    }

    // POST /AdminProductos/Crear
    // categoriaTexto y alergenosTexto llegan como texto simple desde el
    // formulario; aquí se traducen a las relaciones reales (Categorias,
    // Alergenos) vía IProductoRepository.ObtenerOCrear*.
    //
    // imagen1 a imagen5: cada una corresponde a un "slot" fijo (Orden 1 a
    // 5). imagen1 es la principal y es obligatoria; imagen2-5 son
    // opcionales.
    //
    // personalizaciones: qué opciones de color/topping quedaron marcadas
    // para este producto y a qué precio adicional (ver sección
    // "Personalizaciones disponibles" de Formulario.cshtml). Se arma
    // producto.OpcionesPersonalizacion ANTES de cualquier validación, para
    // que si el formulario se recarga por un error, no se pierda lo que el
    // admin ya había marcado.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(
        Producto producto,
        IFormFile? imagen1, IFormFile? imagen2, IFormFile? imagen3, IFormFile? imagen4, IFormFile? imagen5,
        string? categoriaTexto, string? alergenosTexto,
        List<OpcionPersonalizacionFormDto>? personalizaciones)
    {
        producto.OpcionesPersonalizacion = ArmarOpcionesSeleccionadas(personalizaciones);

        if (string.IsNullOrWhiteSpace(producto.Nombre) || string.IsNullOrWhiteSpace(categoriaTexto))
        {
            ModelState.AddModelError(string.Empty, "Nombre y categoría son obligatorios.");
            ViewBag.Categorias = _productoRepository.ObtenerCategorias();
            ViewBag.TiposPersonalizacion = _productoRepository.ObtenerTiposPersonalizacion();
            ViewBag.EsEdicion = false;
            return View("Formulario", producto);
        }

        if (!producto.Presentaciones.Any())
        {
            ModelState.AddModelError(string.Empty, "Agrega al menos una presentación con su precio.");
            ViewBag.Categorias = _productoRepository.ObtenerCategorias();
            ViewBag.TiposPersonalizacion = _productoRepository.ObtenerTiposPersonalizacion();
            ViewBag.EsEdicion = false;
            return View("Formulario", producto);
        }

        // La imagen principal (slot 1) sigue siendo obligatoria.
        if (imagen1 is null || imagen1.Length == 0)
        {
            ModelState.AddModelError(string.Empty, "La imagen principal es obligatoria.");
            ViewBag.Categorias = _productoRepository.ObtenerCategorias();
            ViewBag.TiposPersonalizacion = _productoRepository.ObtenerTiposPersonalizacion();
            ViewBag.EsEdicion = false;
            return View("Formulario", producto);
        }

        // El "precio desde" mostrado en catálogo/inicio es el de la
        // presentación más económica, igual que documenta el diseño de BD.
        producto.Precio = producto.Presentaciones.Min(p => p.Precio);
        producto.Categorias = new() { _productoRepository.ObtenerOCrearCategoria(categoriaTexto) };
        producto.Alergenos = _productoRepository.ObtenerOCrearAlergenos(DividirTexto(alergenosTexto));
        producto.CreadoPorCorreo = User.Identity?.Name;

        await GuardarImagenesEnSlotsAsync(producto, imagen1, imagen2, imagen3, imagen4, imagen5);

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
        ViewBag.TiposPersonalizacion = _productoRepository.ObtenerTiposPersonalizacion();
        ViewBag.EsEdicion = true;
        return View("Formulario", producto);
    }

    // POST /AdminProductos/Editar/3
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        int id,
        Producto producto,
        IFormFile? imagen1, IFormFile? imagen2, IFormFile? imagen3, IFormFile? imagen4, IFormFile? imagen5,
        string? categoriaTexto, string? alergenosTexto,
        List<OpcionPersonalizacionFormDto>? personalizaciones)
    {
        producto.Id = id;
        producto.OpcionesPersonalizacion = ArmarOpcionesSeleccionadas(personalizaciones);

        if (string.IsNullOrWhiteSpace(producto.Nombre) || !producto.Presentaciones.Any())
        {
            ModelState.AddModelError(string.Empty, "Revisa el nombre y que exista al menos una presentación.");
            ViewBag.Categorias = _productoRepository.ObtenerCategorias();
            ViewBag.TiposPersonalizacion = _productoRepository.ObtenerTiposPersonalizacion();
            ViewBag.EsEdicion = true;
            return View("Formulario", producto);
        }

        producto.Precio = producto.Presentaciones.Min(p => p.Precio);
        if (!string.IsNullOrWhiteSpace(categoriaTexto))
        {
            producto.Categorias = new() { _productoRepository.ObtenerOCrearCategoria(categoriaTexto) };
        }
        producto.Alergenos = _productoRepository.ObtenerOCrearAlergenos(DividirTexto(alergenosTexto));

        // Solo se reemplaza cada imagen si llega un archivo nuevo para ese
        // slot; si no, el repositorio conserva la que ya tenía en ese slot
        // (ver EfProductoRepository.Actualizar, que ahora reemplaza por
        // posición/Orden en vez de solo principal/secundaria).
        await GuardarImagenesEnSlotsAsync(producto, imagen1, imagen2, imagen3, imagen4, imagen5);

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

    private List<string> DividirTexto(string? texto) =>
        (texto ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

    // Convierte lo que llegó del formulario (una fila por CADA opción del
    // catálogo global, marcada o no) en la lista real que se guarda: solo
    // las que quedaron con el checkbox marcado.
    private List<ProductoOpcionPersonalizacion> ArmarOpcionesSeleccionadas(List<OpcionPersonalizacionFormDto>? personalizaciones) =>
        (personalizaciones ?? new())
            .Where(p => p.Habilitada)
            .Select(p => new ProductoOpcionPersonalizacion
            {
                OpcionId = p.OpcionId,
                PrecioAdicional = p.PrecioAdicional,
                Disponible = true,
            })
            .ToList();

    // Guarda hasta 5 imágenes en el producto, cada una en su "slot" fijo
    // (Orden 1 a 5). El slot 1 es siempre la principal. Solo se agregan
    // los archivos que sí llegaron (los slots vacíos simplemente no se
    // tocan — EfProductoRepository.Actualizar conserva lo que ya había en
    // ese slot si no llega nada nuevo).
    private async Task GuardarImagenesEnSlotsAsync(Producto producto, params IFormFile?[] archivos)
    {
        for (int slot = 1; slot <= MaxImagenes; slot++)
        {
            var archivo = archivos[slot - 1];
            if (archivo is null || archivo.Length == 0) continue;

            var url = await GuardarImagenAsync(archivo);
            producto.Imagenes.Add(new ImagenProducto { UrlImagen = url, Orden = slot, EsPrincipal = slot == 1 });
        }
    }

    // POST /AdminProductos/EliminarImagen/3
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EliminarImagen(int id, int slot)
    {
        _productoRepository.EliminarImagen(id, slot);
        TempData["Mensaje"] = "Imagen eliminada correctamente.";
        return RedirectToAction(nameof(Editar), new { id });
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

        // TODO (BD real / almacenamiento en la nube): esto guarda el
        // archivo en el disco del servidor y su ruta relativa queda en una
        // fila de imagen_producto, tal como documenta el diseño de BD. Si
        // el hosting no persiste disco (ej. algunos PaaS), esto debe migrar
        // a un proveedor de storage.
        return $"/{CarpetaImagenes}/{nombreArchivo}";
    }
}

// DTO para leer del formulario qué opciones de personalización (color,
// topping, etc.) quedaron marcadas para este producto y con qué precio
// adicional (ver Formulario.cshtml, sección "Personalizaciones
// disponibles"). Llega UNA fila por CADA opción del catálogo global,
// marcada o no — Habilitada dice si aplica a este producto.
public class OpcionPersonalizacionFormDto
{
    public int OpcionId { get; set; }
    public bool Habilitada { get; set; }
    public decimal PrecioAdicional { get; set; }
}
