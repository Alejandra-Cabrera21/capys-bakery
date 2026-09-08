using CapysBakery.Web.Data;
using CapysBakery.Web.Models;

namespace CapysBakery.Web.Services;

// Implementación REAL sobre CapysBakeryDb. Reemplaza a
// MockEntregaPagoRepository (Fase 6).
public class EfEntregaPagoRepository : IEntregaPagoRepository
{
    private readonly CapysBakeryDbContext _db;

    public EfEntregaPagoRepository(CapysBakeryDbContext db)
    {
        _db = db;
    }

    // "Envío" primero (para que quede activo por defecto en el checkout,
    // igual que antes): se ordena por RequiereDireccion descendente, ya que
    // Envío = true y Recoger = false.
    public List<ModalidadEntrega> ObtenerModalidadesEntrega() =>
        _db.ModalidadesEntrega.OrderByDescending(m => m.RequiereDireccion).ToList();

    public List<MetodoPago> ObtenerMetodosPago() =>
        _db.MetodosPago.Where(m => m.Disponible).OrderBy(m => m.Id).ToList();

    public CuentaBancaria? ObtenerCuentaBancariaPrincipal() =>
        _db.CuentasBancarias.Where(c => c.Disponible).OrderBy(c => c.Id).FirstOrDefault();
}
