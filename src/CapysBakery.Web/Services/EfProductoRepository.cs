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
    // alérgenos, presentaciones, imágenes, y ahora también las opciones de
    // personalización habilitadas para el producto, con su precio) en una
    // sola consulta.
    private IQueryable<Producto> ConIncludes() =>
        _db.Productos
            .Include(p => p.Categorias)
            .Include(p => p.Alergenos)
            .Include(p => p.Presentaciones)
            .Include(p => p.Imagenes)
            .Include(p => p.OpcionesPersonalizacion).ThenInclude(o => o.Opcion!).ThenInclude(op => op.TipoPersonalizacion);

    public List<Producto> ObtenerTodos() =>
        ConIncludes().Where(p => p.Disponible).ToList();

    // Antes tomaba los primeros N productos disponibles sin ningún
    // criterio real (ni ventas, ni selección) — el texto "Lo más pedido
    // esta semana" del Home no reflejaba nada de verdad. Ahora sí muestra
    // los que el Dueño/Administrador marcó como destacados desde el
    // panel, para poder cambiar la selección cada semana.
    public List<Producto> ObtenerDestacados(int cantidad) =>
        ConIncludes().Where(p => p.Disponible && p.EsDestacado).Take(cantidad).ToList();

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
        // producto.OpcionesPersonalizacion (si el formulario marcó alguna)
        // viaja como parte del mismo grafo — EF Core la inserta sola junto
        // con el producto, sin necesitar código extra aquí.
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
        existente.EsDestacado = producto.EsDestacado;
        existente.Disponible = producto.Disponible;
        existente.Categorias = producto.Categorias;
        existente.Alergenos = producto.Alergenos;

        // Antes esto borraba TODAS las presentaciones y las volvía a crear
        // desde cero — funcionaba mientras nadie había hecho un pedido
        // real todavía. En cuanto un pedido ya referencia una presentación
        // (pedido_detalle.id_presentacion), SQL Server rechaza el borrado
        // por la llave foránea. Ahora se sincroniza: actualiza las que ya
        // existían (por Id), agrega las nuevas, y solo borra las quitadas
        // si ningún pedido las está usando.
        SincronizarPresentaciones(existente, producto.Presentaciones);

        // Mismo cuidado para las opciones de personalización: un pedido ya
        // hecho puede referenciar una (pedido_detalle_personalizacion), así
        // que nunca se borran físicamente — si el admin las desmarca, solo
        // se "apagan" con Disponible = false.
        SincronizarOpcionesPersonalizacion(existente, producto.OpcionesPersonalizacion);

        // Cada imagen se reemplaza por separado según su "slot" (Orden 1
        // a 5, donde el 1 es la principal) — subir una imagen nueva en un
        // slot NO debe borrar las que ya había en los demás slots. Esto
        // reemplaza la lógica anterior que solo distinguía 2 casos
        // (principal/secundaria); ahora funciona igual para cualquier
        // cantidad de slots sin tener que tocar este método de nuevo.
        foreach (var nueva in producto.Imagenes)
        {
            var actual = existente.Imagenes.FirstOrDefault(i => i.Orden == nueva.Orden);
            if (actual is not null) _db.ImagenesProducto.Remove(actual);
            existente.Imagenes.Add(nueva);
        }

        _db.SaveChanges();
        return true;
    }

    private void SincronizarPresentaciones(Producto existente, List<ProductoPresentacion> nuevas)
    {
        var idsEnviados = nuevas.Where(p => p.Id != 0).Select(p => p.Id).ToHashSet();

        // Elimina las que ya no están en el formulario — pero solo si
        // ningún pedido real las usa. Si algún pedido_detalle ya la
        // referencia, se conserva tal cual (aunque el admin la haya
        // quitado del formulario) para no perder el historial de esos
        // pedidos ni volver a tronar por la llave foránea.
        foreach (var actual in existente.Presentaciones.Where(p => !idsEnviados.Contains(p.Id)).ToList())
        {
            var tienePedidos = _db.PedidoDetalles.Any(d => d.PresentacionId == actual.Id);
            if (!tienePedidos)
            {
                _db.ProductoPresentaciones.Remove(actual);
            }
        }

        // Actualiza las que ya existían (por Id) o agrega las nuevas (Id
        // en 0, o un Id que no coincidió con ninguna presentación real).
        foreach (var nueva in nuevas)
        {
            var actual = nueva.Id != 0
                ? existente.Presentaciones.FirstOrDefault(p => p.Id == nueva.Id)
                : null;

            if (actual is not null)
            {
                actual.Nombre = nueva.Nombre;
                actual.Porciones = nueva.Porciones;
                actual.Precio = nueva.Precio;
            }
            else
            {
                existente.Presentaciones.Add(new ProductoPresentacion
                {
                    Nombre = nueva.Nombre,
                    Porciones = nueva.Porciones,
                    Precio = nueva.Precio,
                });
            }
        }
    }

    // Sincroniza qué opciones de personalización (color, topping, etc.)
    // están habilitadas para este producto y a qué precio adicional.
    // Nunca borra físicamente una fila producto_opcion_personalizacion:
    // si el admin desmarca una opción que ya se usó en algún pedido
    // (pedido_detalle_personalizacion la referencia), borrarla rompería
    // ese historial por la llave foránea. En vez de eso se "apaga" con
    // Disponible = false — mismo patrón que ya usa el resto del catálogo
    // (producto.Disponible, categoria.Disponible, etc.).
    private void SincronizarOpcionesPersonalizacion(Producto existente, List<ProductoOpcionPersonalizacion> seleccionadas)
    {
        var seleccionadasPorOpcionId = seleccionadas.ToDictionary(o => o.OpcionId);

        foreach (var actual in existente.OpcionesPersonalizacion)
        {
            if (seleccionadasPorOpcionId.TryGetValue(actual.OpcionId, out var seleccionada))
            {
                actual.Disponible = true;
                actual.PrecioAdicional = seleccionada.PrecioAdicional;
            }
            else
            {
                actual.Disponible = false;
            }
        }

        foreach (var nueva in seleccionadas)
        {
            var yaExiste = existente.OpcionesPersonalizacion.Any(o => o.OpcionId == nueva.OpcionId);
            if (!yaExiste)
            {
                existente.OpcionesPersonalizacion.Add(new ProductoOpcionPersonalizacion
                {
                    OpcionId = nueva.OpcionId,
                    PrecioAdicional = nueva.PrecioAdicional,
                    Disponible = true,
                });
            }
        }
    }

    public bool EliminarImagen(int productoId, int slot)
    {
        var producto = ConIncludes().FirstOrDefault(p => p.Id == productoId);
        if (producto is null) return false;

        var imagen = producto.Imagenes.FirstOrDefault(i => i.Orden == slot);
        if (imagen is null) return false;

        _db.ImagenesProducto.Remove(imagen);
        _db.SaveChanges();
        return true;
    }

    public List<TipoPersonalizacion> ObtenerTiposPersonalizacion() =>
        _db.TiposPersonalizacion.Include(t => t.Opciones).OrderBy(t => t.Id).ToList();
}
