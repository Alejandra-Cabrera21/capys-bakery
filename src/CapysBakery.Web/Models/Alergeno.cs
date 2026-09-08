namespace CapysBakery.Web.Models;

// Catálogo normalizado de alérgenos (tabla alergeno). Se relaciona con
// Producto en N:M a través de producto_alergeno.
public class Alergeno
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public List<Producto> Productos { get; set; } = new();
}
