namespace CapysBakery.Web.Models;

public class Categoria
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    // Coincide con categoria.disponible: permite ocultar/mostrar una
    // categoría del catálogo sin borrarla.
    public bool Disponible { get; set; } = true;

    // Lado N:M con Producto (tabla intermedia producto_categoria).
    public List<Producto> Productos { get; set; } = new();
}
