using CapysBakery.Web.Data;
using CapysBakery.Web.Models;

namespace CapysBakery.Web.Services;

public class EfContactoRepository : IContactoRepository
{
    private readonly CapysBakeryDbContext _db;

    public EfContactoRepository(CapysBakeryDbContext db)
    {
        _db = db;
    }

    public void Guardar(MensajeContacto mensaje)
    {
        _db.MensajesContacto.Add(mensaje);
        _db.SaveChanges();
    }

    public List<MensajeContacto> ObtenerTodos() =>
        _db.MensajesContacto
            .OrderByDescending(m => m.FechaEnvio)
            .ToList();

    public void MarcarComoLeido(int id)
    {
        var mensaje = _db.MensajesContacto.FirstOrDefault(m => m.Id == id);
        if (mensaje is null) return;

        mensaje.Leido = true;
        _db.SaveChanges();
    }
}
