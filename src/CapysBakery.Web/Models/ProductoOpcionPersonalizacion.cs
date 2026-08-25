namespace CapysBakery.Web.Models;

// Coincide con producto_opcion_personalizacion: qué opciones están
// habilitadas para cada producto y su costo adicional.
public class ProductoOpcionPersonalizacion
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public int OpcionId { get; set; }
    public OpcionPersonalizacion? Opcion { get; set; }
    public decimal PrecioAdicional { get; set; }
    public bool Disponible { get; set; } = true;
}
