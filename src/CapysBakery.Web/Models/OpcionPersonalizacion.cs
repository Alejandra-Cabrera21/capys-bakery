namespace CapysBakery.Web.Models;

// Coincide con opcion_personalizacion.
public class OpcionPersonalizacion
{
    public int Id { get; set; }
    public int TipoPersonalizacionId { get; set; }
    public TipoPersonalizacion? TipoPersonalizacion { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
