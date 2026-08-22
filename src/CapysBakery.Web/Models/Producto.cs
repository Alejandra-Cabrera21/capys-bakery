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

    public List<PresentacionOpcion> Presentaciones { get; set; } = new();
    public List<string> Alergenos { get; set; } = new();
}
