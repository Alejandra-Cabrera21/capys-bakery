using CapysBakery.Web.Data;
using CapysBakery.Web.Models;

namespace CapysBakery.Web.Services;

public class EfContenidoSitioRepository : IContenidoSitioRepository
{
    private readonly CapysBakeryDbContext _db;

    public EfContenidoSitioRepository(CapysBakeryDbContext db)
    {
        _db = db;
    }

    public string? ObtenerImagen(string clave) =>
        _db.ContenidosSitio.FirstOrDefault(c => c.Clave == clave)?.UrlImagen;

    public Dictionary<string, string?> ObtenerTodas() =>
        _db.ContenidosSitio.ToDictionary(c => c.Clave, c => c.UrlImagen);

    public void ActualizarImagen(string clave, string urlImagen)
    {
        var existente = _db.ContenidosSitio.FirstOrDefault(c => c.Clave == clave);
        if (existente is not null)
        {
            existente.UrlImagen = urlImagen;
            existente.FechaActualizacion = DateTime.Now;
        }
        else
        {
            _db.ContenidosSitio.Add(new ContenidoSitio { Clave = clave, UrlImagen = urlImagen });
        }

        _db.SaveChanges();
    }

    public void EliminarImagen(string clave)
    {
        var existente = _db.ContenidosSitio.FirstOrDefault(c => c.Clave == clave);
        if (existente is null) return;

        existente.UrlImagen = null;
        existente.FechaActualizacion = DateTime.Now;
        _db.SaveChanges();
    }
}
