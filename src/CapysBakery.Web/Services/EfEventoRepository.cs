using CapysBakery.Web.Data;
using CapysBakery.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CapysBakery.Web.Services;

public class EfEventoRepository : IEventoRepository
{
    private readonly CapysBakeryDbContext _db;

    public EfEventoRepository(CapysBakeryDbContext db)
    {
        _db = db;
    }

    public List<PaqueteEvento> ObtenerTodos() =>
        _db.PaquetesEvento
            .OrderBy(p => p.Orden)
            .ThenBy(p => p.Id)
            .ToList();

    public List<PaqueteEvento> ObtenerActivos() =>
        _db.PaquetesEvento
            .Where(p => p.Activo)
            .OrderBy(p => p.Orden)
            .ThenBy(p => p.Id)
            .ToList();

    public PaqueteEvento? ObtenerPorId(int id) =>
        _db.PaquetesEvento.FirstOrDefault(p => p.Id == id);

    public void Crear(PaqueteEvento paquete)
    {
        _db.PaquetesEvento.Add(paquete);
        _db.SaveChanges();
    }

    public void Actualizar(PaqueteEvento paquete)
    {
        var existente = _db.PaquetesEvento.FirstOrDefault(p => p.Id == paquete.Id);
        if (existente is null) return;

        existente.Nombre = paquete.Nombre;
        existente.Descripcion = paquete.Descripcion;
        existente.Precio = paquete.Precio;
        existente.Incluye = paquete.Incluye;
        existente.Etiqueta = paquete.Etiqueta;
        existente.Orden = paquete.Orden;
        existente.Activo = paquete.Activo;

        _db.SaveChanges();
    }

    public void Eliminar(int id)
    {
        var existente = _db.PaquetesEvento.FirstOrDefault(p => p.Id == id);
        if (existente is null) return;

        _db.PaquetesEvento.Remove(existente);
        _db.SaveChanges();
    }

    public void CambiarActivo(int id, bool activo)
    {
        var existente = _db.PaquetesEvento.FirstOrDefault(p => p.Id == id);
        if (existente is null) return;

        existente.Activo = activo;
        _db.SaveChanges();
    }
}
