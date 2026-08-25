using CapysBakery.Web.Models;

namespace CapysBakery.Web.Services;

// Mismo patrón que los demás repositorios: los controladores solo conocen
// este contrato. Hoy la implementa MockEntregaPagoRepository (en memoria).
public interface IEntregaPagoRepository
{
    List<ModalidadEntrega> ObtenerModalidadesEntrega();
    List<MetodoPago> ObtenerMetodosPago();

    // El MVP solo necesita mostrar una cuenta bancaria en el checkout; el
    // diseño de BD deja abierta la posibilidad de varias (1:N desde
    // metodo_pago), pero aquí basta con la principal.
    CuentaBancaria? ObtenerCuentaBancariaPrincipal();
}
