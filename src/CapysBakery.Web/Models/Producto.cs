namespace CapysBakery.Web.Models;

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;

    // Precio "desde" que se muestra en catálogo/inicio (normalmente el de
    // la presentación más económica). El precio real a cobrar depende de
    // la presentación elegida en el detalle (ver Presentaciones).
    public decimal Precio { get; set; }

    public string? ImagenUrl { get; set; }
    public bool EsPromocion { get; set; }

    // Permite ocultar el producto del catálogo público sin borrarlo,
    // tal como se documentó en el diseño de base de datos (columna
    // "disponible" de la tabla producto).
    public bool Disponible { get; set; } = true;

    // Auditoría básica: quién publicó el producto y cuándo. Cuando exista
    // Entity Framework + Identity real, esto se vuelve una FK a Usuario
    // en vez de guardar el correo como texto.
    public string? CreadoPorCorreo { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public List<PresentacionOpcion> Presentaciones { get; set; } = new();
    public List<string> Alergenos { get; set; } = new();
}
