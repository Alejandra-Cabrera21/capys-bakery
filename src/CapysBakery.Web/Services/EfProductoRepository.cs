using CapysBakery.Web.Data;
using CapysBakery.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CapysBakery.Web.Services;

// Implementación REAL: lee y escribe de verdad en CapysBakeryDb a través
// de CapysBakeryDbContext. Reemplaza a MockProductoRepository (Fase 6).
public class EfProductoRepository : IProductoRepository
{
    private readonly CapysBakeryDbContext _db;

    public EfProductoRepository(CapysBakeryDbContext db)
    {
        _db = db;
    }

    // Trae siempre las relaciones que la app necesita mostrar (categorías,
    // alérgenos, presentaciones, imágenes) en una sola consulta.
    private IQueryable<Producto> ConIncludes() =>
        _db.Productos
            .Include(p => p.Categorias)
            .Include(p => p.Alergenos)
            .Include(p => p.Presentaciones)
            .Include(p => p.Imagenes);

    public List<Producto> ObtenerTodos() =>
        ConIncludes().Where(p => p.Disponible).ToList();

    public List<Producto> ObtenerDestacados(int cantidad) =>
        ConIncludes().Where(p => p.Disponible).Take(cantidad).ToList();

    public List<Producto> ObtenerPromociones() =>
        ConIncludes().Where(p => p.Disponible && p.EsPromocion).ToList();

    public Producto? ObtenerPorId(int id) =>
        ConIncludes().FirstOrDefault(p => p.Id == id);

    public List<Producto> ObtenerPorCategoria(string categoria) =>
        ConIncludes().Where(p => p.Disponible && p.Categorias.Any(c => c.Nombre == categoria)).ToList();

    public List<Categoria> ObtenerCategorias() =>
        _db.Categorias.Where(c => c.Disponible).ToList();

    public List<Producto> ObtenerTodosIncluyendoNoDisponibles() =>
        ConIncludes().ToList();

    public Categoria ObtenerOCrearCategoria(string nombre)
    {
        var existente = _db.Categorias.FirstOrDefault(c => c.Nombre == nombre);
        if (existente is not null) return existente;

        var nueva = new Categoria { Nombre = nombre, Disponible = true };
        _db.Categorias.Add(nueva);
        _db.SaveChanges();
        return nueva;
    }

    public List<Alergeno> ObtenerOCrearAlergenos(List<string> nombres)
    {
        var resultado = new List<Alergeno>();
        foreach (var nombre in nombres)
        {
            var existente = _db.Alergenos.FirstOrDefault(a => a.Nombre == nombre);
            if (existente is null)
            {
                existente = new Alergeno { Nombre = nombre };
                _db.Alergenos.Add(existente);
            }
            resultado.Add(existente);
        }
        _db.SaveChanges();
        return resultado;
    }

    public List<Categoria> ObtenerTodasLasCategorias() =>
        _db.Categorias.OrderBy(c => c.Nombre).ToList();

    public Categoria? ObtenerCategoriaPorId(int id) =>
        _db.Categorias.FirstOrDefault(c => c.Id == id);

    public Categoria CrearCategoria(string nombre)
    {
        var existente = _db.Categorias.FirstOrDefault(c => c.Nombre == nombre);
        if (existente is not null) return existente;

        var nueva = new Categoria { Nombre = nombre, Disponible = true };
        _db.Categorias.Add(nueva);
        _db.SaveChanges();
        return nueva;
    }

    public bool ActualizarCategoria(int id, string nuevoNombre, bool disponible)
    {
        var categoria = _db.Categorias.FirstOrDefault(c => c.Id == id);
        if (categoria is null) return false;

        categoria.Nombre = nuevoNombre;
        categoria.Disponible = disponible;
        _db.SaveChanges();
        return true;
    }

    public Producto Agregar(Producto producto)
    {
        _db.Productos.Add(producto);
        _db.SaveChanges();
        return producto;
    }

    public bool Actualizar(Producto producto)
    {
        var existente = ConIncludes().FirstOrDefault(p => p.Id == producto.Id);
        if (existente is null) return false;

        existente.Nombre = producto.Nombre;
        existente.Descripcion = producto.Descripcion;
        existente.Precio = producto.Precio;
        existente.EsPromocion = producto.EsPromocion;
        existente.Disponible = producto.Disponible;
        existente.Categorias = producto.Categorias;
        existente.Alergenos = producto.Alergenos;

        // NOTA (misma limitación que ya tenía MockProductoRepository): al
        // no venir un Id por cada presentación desde el formulario, se
        // reemplazan todas. Si en el futuro los pedidos ya referencian
        // producto_presentacion de verdad, esto deberá volverse un
        // "actualizar si ya existe, insertar si es nueva".
        _db.ProductoPresentaciones.RemoveRange(existente.Presentaciones);
        existente.Presentaciones = producto.Presentaciones;

        // Cada imagen se reemplaza por separado según su "slot" (principal
        // o secundaria) — subir una nueva imagen principal NO debe borrar
        // la secundaria que ya tenía, y viceversa.
        var nuevaPrincipal = producto.Imagenes.FirstOrDefault(i => i.EsPrincipal);
        if (nuevaPrincipal is not null)
        {
            var actual = existente.Imagenes.FirstOrDefault(i => i.EsPrincipal);
            if (actual is not null) _db.ImagenesProducto.Remove(actual);
            existente.Imagenes.Add(nuevaPrincipal);
        }

        var nuevaSecundaria = producto.Imagenes.FirstOrDefault(i => !i.EsPrincipal);
        if (nuevaSecundaria is not null)
        {
            var actual = existente.Imagenes.FirstOrDefault(i => !i.EsPrincipal);
            if (actual is not null) _db.ImagenesProducto.Remove(actual);
            existente.Imagenes.Add(nuevaSecundaria);
        }

        _db.SaveChanges();
        return true;
    }
}
