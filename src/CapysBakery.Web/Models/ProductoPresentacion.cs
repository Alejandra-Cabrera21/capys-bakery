namespace CapysBakery.Web.Models;

// Coincide con producto_presentacion. Antes era un objeto embebido sin Id
// propio (PresentacionOpcion); ahora es una fila real con su propia llave
// primaria y su FK al producto, tal como se documentó en el diseño de BD.
public class ProductoPresentacion
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }
    public string Nombre { get; set; } = string.Empty; // Ej. "12 personas", "Individual"
    public int? Porciones { get; set; }
    public decimal Precio { get; set; }
}
