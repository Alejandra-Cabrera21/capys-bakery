namespace CapysBakery.Web.Models;

// Coincide con cuenta_bancaria. Antes vivía como una constante fija dentro
// de checkout.js (DATOS_BANCARIOS); ahora es un dato real ligado a un
// método de pago.
public class CuentaBancaria
{
    public int Id { get; set; }
    public int MetodoPagoId { get; set; }
    public string Banco { get; set; } = string.Empty;
    public string TipoCuenta { get; set; } = string.Empty;
    public string NumeroCuenta { get; set; } = string.Empty;
    public string Titular { get; set; } = string.Empty;
    public bool Disponible { get; set; } = true;
}
