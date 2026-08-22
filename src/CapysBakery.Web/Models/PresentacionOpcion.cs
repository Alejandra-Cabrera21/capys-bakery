namespace CapysBakery.Web.Models;

// Cada producto configurable (ej. un pastel) puede venir en distintos
// tamaños/presentaciones, y cada una tiene su propia cantidad de porciones
// y su propio precio (no todos cuestan lo mismo).
public class PresentacionOpcion
{
    public string Nombre { get; set; } = string.Empty; // Ej. "12 personas", "Individual"
    public int Porciones { get; set; }
    public decimal Precio { get; set; }
}
