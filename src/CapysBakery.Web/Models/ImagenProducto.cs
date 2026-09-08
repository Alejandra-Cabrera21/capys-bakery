namespace CapysBakery.Web.Models;

// Coincide con imagen_producto: un producto puede tener varias fotos (el
// MVP limita esto a la práctica a 1-2 desde el panel de administración,
// pero el modelo de datos ya soporta más sin necesitar otro cambio de
// esquema).
public class ImagenProducto
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public string UrlImagen { get; set; } = string.Empty;
    public int Orden { get; set; } = 1;
    public bool EsPrincipal { get; set; } = true;
}
