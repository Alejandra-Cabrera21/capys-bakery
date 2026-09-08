namespace CapysBakery.Web.Models;

// Coincide con tipo_personalizacion. Todavía ninguna pantalla de la app usa
// esto (las opciones de "color"/"toppings" en Catalogo/Detalle son solo
// decorativas, no están ligadas a precio ni a este catálogo) — se agrega
// para que el modelo de datos completo quede reflejado en el código, tal
// como se documentó en el diseño de BD.
public class TipoPersonalizacion
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public List<OpcionPersonalizacion> Opciones { get; set; } = new();
}
