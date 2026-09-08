using CapysBakery.Web.Data;
using CapysBakery.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CapysBakery.Web.Services;

// Implementación REAL sobre CapysBakeryDb. Reemplaza a
// MockPublicacionRepository (Fase 6).
public class EfPublicacionRepository : IPublicacionRepository
{
    private readonly CapysBakeryDbContext _db;

    public EfPublicacionRepository(CapysBakeryDbContext db)
    {
        _db = db;
    }

    public List<Publicacion> ObtenerPublicadas() =>
        _db.Publicaciones.Where(p => p.Publicada).OrderByDescending(p => p.FechaPublicacion).ToList();

    public List<Publicacion> ObtenerTodas() =>
        _db.Publicaciones.OrderByDescending(p => p.FechaPublicacion).ToList();

    public Publicacion? ObtenerPorId(int id) => _db.Publicaciones.FirstOrDefault(p => p.Id == id);

    public Publicacion Agregar(Publicacion publicacion)
    {
        publicacion.FechaPublicacion = DateTime.Now;
        _db.Publicaciones.Add(publicacion);
        _db.SaveChanges();
        return publicacion;
    }

    public bool Actualizar(Publicacion publicacion)
    {
        var existente = _db.Publicaciones.FirstOrDefault(p => p.Id == publicacion.Id);
        if (existente is null) return false;

        existente.Titulo = publicacion.Titulo;
        existente.Categoria = publicacion.Categoria;
        existente.Resumen = publicacion.Resumen;
        existente.Contenido = publicacion.Contenido;
        existente.Publicada = publicacion.Publicada;

        if (!string.IsNullOrWhiteSpace(publicacion.ImagenUrl))
        {
            existente.ImagenUrl = publicacion.ImagenUrl;
        }

        _db.SaveChanges();
        return true;
    }

    public List<ComentarioPublicacion> ObtenerComentarios(int publicacionId) =>
        _db.ComentariosPublicacion
            .Include(c => c.Usuario)
            .Where(c => c.PublicacionId == publicacionId)
            .OrderBy(c => c.FechaCreacion)
            .ToList();

    public ComentarioPublicacion AgregarComentario(int publicacionId, int usuarioId, string contenido)
    {
        var comentario = new ComentarioPublicacion
        {
            PublicacionId = publicacionId,
            UsuarioId = usuarioId,
            Contenido = contenido,
            FechaCreacion = DateTime.Now,
        };
        _db.ComentariosPublicacion.Add(comentario);
        _db.SaveChanges();
        return comentario;
    }

    public bool EliminarComentario(int comentarioId)
    {
        var comentario = _db.ComentariosPublicacion.FirstOrDefault(c => c.Id == comentarioId);
        if (comentario is null) return false;

        _db.ComentariosPublicacion.Remove(comentario);
        _db.SaveChanges();
        return true;
    }
}
