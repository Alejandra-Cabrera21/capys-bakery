using System.ComponentModel.DataAnnotations;

namespace CapysBakery.Web.Models;

// Representa una tarjeta de paquete en /Eventos (ej. "Paquete Básico",
// "Paquete Premium"). Antes esto era HTML fijo en la vista; ahora se
// administra desde /AdminEventos igual que el resto del contenido del sitio.
public class PaqueteEvento
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    // Frase corta bajo el nombre, ej. "Para reuniones íntimas".
    [StringLength(150)]
    public string? Subtitulo { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(500)]
    public string Descripcion { get; set; } = string.Empty;

    [Range(0, 999999, ErrorMessage = "El precio debe ser un número válido.")]
    public decimal Precio { get; set; }

    // Texto que acompaña al precio, ej. "hasta 15 personas" o "persona".
    // En la tarjeta se muestra como "Q450 / hasta 15 personas".
    [StringLength(50)]
    public string? UnidadPrecio { get; set; }

    // Un ítem incluido por línea (ej. "Pastel de 2 pisos\nMesa de dulces\n...").
    // Se guarda como texto plano para no crear otra tabla; ItemsIncluidos
    // lo separa en lista para pintarlo como <li> en la vista.
    [Required(ErrorMessage = "Agrega al menos un ítem incluido.")]
    public string Incluye { get; set; } = string.Empty;

    // Etiqueta opcional tipo "Más popular" que se muestra como badge en la
    // tarjeta. Puede quedar vacía.
    [StringLength(50)]
    public string? Etiqueta { get; set; }

    // Controla el orden en que aparecen las tarjetas en /Eventos.
    public int Orden { get; set; }

    // Permite "ocultar" un paquete sin borrarlo (por si se quiere reactivar
    // después, ej. un paquete de temporada).
    public bool Activo { get; set; } = true;

    public List<string> ItemsIncluidos =>
        Incluye
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
}
