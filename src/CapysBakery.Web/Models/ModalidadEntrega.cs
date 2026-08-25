namespace CapysBakery.Web.Models;

// Coincide con modalidad_entrega. Antes "Envío"/"Recoger" eran solo texto
// suelto escrito en el HTML del checkout; ahora son datos reales.
public class ModalidadEntrega
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty; // "Envío" o "Recoger"

    // Si es true, el pedido debe traer dirección/punto de entrega.
    public bool RequiereDireccion { get; set; }
}
