namespace CapysBakery.Web.Models;

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;

    // Precio "desde" que se muestra en catálogo/inicio (el de la
    // presentación más económica). El precio real a cobrar depende de la
    // presentación elegida en el detalle (ver Presentaciones).
    public decimal Precio { get; set; }

    public bool EsPromocion { get; set; }

    // Permite ocultar el producto del catálogo público sin borrarlo,
    // tal como se documentó en el diseño de base de datos (columna
    // "disponible" de la tabla producto).
    public bool Disponible { get; set; } = true;

    // Auditoría básica: quién publicó el producto y cuándo. Cuando exista
    // Identity real, esto se vuelve una FK a Usuario en vez de un correo.
    public string? CreadoPorCorreo { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    // --- Relaciones reales, tal como quedaron en el diagrama E/R ---

    // N:M vía producto_categoria. Un producto puede pertenecer a varias
    // categorías (ej. un loaf personalizado puede estar en "Loafs" y
    // "Personalizados" a la vez).
    public List<Categoria> Categorias { get; set; } = new();

    // N:M vía producto_alergeno.
    public List<Alergeno> Alergenos { get; set; } = new();

    // 1:N — cada presentación es su propia fila con su propio precio.
    public List<ProductoPresentacion> Presentaciones { get; set; } = new();

    // 1:N — el MVP limita esto a la práctica a 1-2 imágenes desde el panel,
    // pero el modelo ya soporta varias sin otro cambio de esquema.
    public List<ImagenProducto> Imagenes { get; set; } = new();

    // Opciones de personalización habilitadas para este producto (aún sin
    // pantalla que las use — ver TipoPersonalizacion.cs).
    public List<ProductoOpcionPersonalizacion> OpcionesPersonalizacion { get; set; } = new();

    // --- Propiedades de conveniencia (NO se guardan como columnas) ---
    // Existen para no tener que reescribir todas las vistas que hasta ahora
    // mostraban "una" categoría y "una" imagen por producto. La regla de
    // negocio (mostrar la categoría/imagen principal) vive aquí, a nivel de
    // aplicación, igual que documenta el diseño de BD para imagen_producto.

    public string CategoriaPrincipal => Categorias.FirstOrDefault()?.Nombre ?? string.Empty;

    public string? ImagenUrl =>
        Imagenes.FirstOrDefault(i => i.EsPrincipal)?.UrlImagen
        ?? Imagenes.FirstOrDefault()?.UrlImagen;
}
