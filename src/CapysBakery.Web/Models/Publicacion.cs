namespace CapysBakery.Web.Models;

// Una entrada del blog. Sigue el mismo patrón que Producto: se puede
// ocultar sin borrar (Publicada), y se guarda quién la escribió y cuándo.
public class Publicacion
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;

    // Texto corto para la tarjeta del listado.
    public string Resumen { get; set; } = string.Empty;

    // Cuerpo completo, mostrado en Blog/Detalle. Se guarda como texto plano
    // con párrafos separados por saltos de línea (la vista los separa en
    // <p>); no es HTML enriquecido en esta primera versión.
    public string Contenido { get; set; } = string.Empty;

    public string? ImagenUrl { get; set; }

    // Permite guardar un borrador o retirar una publicación sin borrarla,
    // igual que Producto.Disponible.
    public bool Publicada { get; set; } = true;

    public string? AutorCorreo { get; set; }
    public DateTime FechaPublicacion { get; set; } = DateTime.Now;
}
