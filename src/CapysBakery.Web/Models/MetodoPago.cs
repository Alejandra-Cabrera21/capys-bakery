namespace CapysBakery.Web.Models;

// Coincide con metodo_pago.
public class MetodoPago
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty; // "Transferencia bancaria" o "Pago al recoger"

    // Regla de negocio documentada: "Pago al recoger" solo puede usarse si
    // la modalidad de entrega es Recoger.
    public bool SoloRecoger { get; set; }

    // Permite desactivar un método sin borrarlo (igual que producto.Disponible).
    public bool Disponible { get; set; } = true;
}
