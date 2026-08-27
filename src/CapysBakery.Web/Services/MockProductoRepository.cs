using CapysBakery.Web.Models;

namespace CapysBakery.Web.Services;

// Implementación TEMPORAL mientras no exista la base de datos.
// TODO (cuando SQL Server esté listo): crear EfProductoRepository
// que implemente IProductoRepository usando el DbContext, y registrarla
// en Program.cs en lugar de esta clase. Ningún Controller ni View cambia.
public class MockProductoRepository : IProductoRepository
{
    private readonly List<Producto> _productos = new()
    {
        new Producto
        {
            Id = 1, Nombre = "Frutos Rojos", Categoria = "Tartaleta",
            Descripcion = "Crema pastelera, frutos rojos frescos de temporada.",
            Precio = 65,
            Alergenos = new() { "Gluten", "Lácteos", "Huevo" },
            Presentaciones = new()
            {
                new PresentacionOpcion { Nombre = "Individual", Porciones = 1, Precio = 65 },
                new PresentacionOpcion { Nombre = "Familiar (6 porciones)", Porciones = 6, Precio = 320 },
            }
        },
        new Producto
        {
            Id = 2, Nombre = "Pie de Limón", Categoria = "Pie",
            Descripcion = "Base crujiente, relleno cítrico, merengue tostado.",
            Precio = 120,
            Alergenos = new() { "Gluten", "Lácteos", "Huevo" },
            Presentaciones = new()
            {
                new PresentacionOpcion { Nombre = "Mediano (8 porciones)", Porciones = 8, Precio = 120 },
                new PresentacionOpcion { Nombre = "Grande (12 porciones)", Porciones = 12, Precio = 165 },
            }
        },
        new Producto
        {
            Id = 3, Nombre = "Chocolate Intenso", Categoria = "Pastel",
            Descripcion = "Tres capas de bizcocho de chocolate belga con ganache y relleno a elección.",
            Precio = 280, EsPromocion = true,
            Alergenos = new() { "Gluten", "Lácteos", "Huevo", "Frutos secos (trazas)" },
            Presentaciones = new()
            {
                new PresentacionOpcion { Nombre = "6 personas", Porciones = 6, Precio = 180 },
                new PresentacionOpcion { Nombre = "12 personas", Porciones = 12, Precio = 280 },
                new PresentacionOpcion { Nombre = "20 personas", Porciones = 20, Precio = 420 },
            }
        },
        new Producto
        {
            Id = 4, Nombre = "Fresa Artesanal", Categoria = "Mermelada",
            Descripcion = "Frasco 250g, sin conservantes, receta original.",
            Precio = 45,
            Alergenos = new() { "Ninguno declarado" },
            Presentaciones = new()
            {
                new PresentacionOpcion { Nombre = "Frasco 250g", Porciones = 1, Precio = 45 },
            }
        },
        new Producto
        {
            Id = 5, Nombre = "Duraznos y Almendra", Categoria = "Tartaleta",
            Descripcion = "Crema de almendra, duraznos glaseados.",
            Precio = 68, EsPromocion = true,
            Alergenos = new() { "Gluten", "Lácteos", "Huevo", "Frutos secos" },
            Presentaciones = new()
            {
                new PresentacionOpcion { Nombre = "Individual", Porciones = 1, Precio = 68 },
                new PresentacionOpcion { Nombre = "Familiar (6 porciones)", Porciones = 6, Precio = 340 },
            }
        },
        new Producto
        {
            Id = 6, Nombre = "Red Velvet", Categoria = "Pastel",
            Descripcion = "Queso crema y toque de cacao.",
            Precio = 260,
            Alergenos = new() { "Gluten", "Lácteos", "Huevo" },
            Presentaciones = new()
            {
                new PresentacionOpcion { Nombre = "6 personas", Porciones = 6, Precio = 170 },
                new PresentacionOpcion { Nombre = "12 personas", Porciones = 12, Precio = 260 },
                new PresentacionOpcion { Nombre = "20 personas", Porciones = 20, Precio = 395 },
            }
        },
    };

    private readonly List<Categoria> _categorias = new()
    {
        new Categoria { Id = 1, Nombre = "Tartaletas" },
        new Categoria { Id = 2, Nombre = "Pies" },
        new Categoria { Id = 3, Nombre = "Pasteles" },
        new Categoria { Id = 4, Nombre = "Mermeladas" },
    };

    // El catálogo público solo debe mostrar productos con Disponible = true
    // (el vendedor/dueño puede ocultar un producto sin borrarlo, igual que
    // se documentó en el diseño de base de datos).
    public List<Producto> ObtenerTodos() => _productos.Where(p => p.Disponible).ToList();

    public List<Producto> ObtenerDestacados(int cantidad) =>
        _productos.Where(p => p.Disponible).Take(cantidad).ToList();

    public List<Producto> ObtenerPromociones() =>
        _productos.Where(p => p.Disponible && p.EsPromocion).ToList();

    // Para el panel de administración: el Dueño/vendedor debe poder ver y
    // reactivar productos ocultos, así que aquí sí se incluyen.
    public Producto? ObtenerPorId(int id) => _productos.FirstOrDefault(p => p.Id == id);

    public List<Producto> ObtenerPorCategoria(string categoria) =>
        _productos.Where(p => p.Disponible && p.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase)).ToList();

    public List<Categoria> ObtenerCategorias() => _categorias;

    public List<Producto> ObtenerTodosIncluyendoNoDisponibles() => _productos;

    public Producto Agregar(Producto producto)
    {
        producto.Id = _productos.Count == 0 ? 1 : _productos.Max(p => p.Id) + 1;
        producto.FechaCreacion = DateTime.Now;
        _productos.Add(producto);

        // Si el producto llega con una categoría que todavía no existe en
        // el catálogo de categorías, se registra automáticamente para que
        // aparezca en los filtros. TODO (BD real): esto pasaría a ser una
        // fila normal en la tabla categoria en vez de crearse "al vuelo".
        if (!string.IsNullOrWhiteSpace(producto.Categoria) &&
            !_categorias.Any(c => c.Nombre.Equals(producto.Categoria, StringComparison.OrdinalIgnoreCase)))
        {
            _categorias.Add(new Categoria { Id = _categorias.Max(c => c.Id) + 1, Nombre = producto.Categoria });
        }

        return producto;
    }

    public bool Actualizar(Producto producto)
    {
        var existente = _productos.FirstOrDefault(p => p.Id == producto.Id);
        if (existente is null) return false;

        existente.Nombre = producto.Nombre;
        existente.Categoria = producto.Categoria;
        existente.Descripcion = producto.Descripcion;
        existente.Precio = producto.Precio;
        existente.EsPromocion = producto.EsPromocion;
        existente.Disponible = producto.Disponible;
        existente.Alergenos = producto.Alergenos;
        existente.Presentaciones = producto.Presentaciones;

        // Solo se reemplaza la imagen si se subió una nueva; si no, se
        // conserva la que ya tenía el producto.
        if (!string.IsNullOrWhiteSpace(producto.ImagenUrl))
        {
            existente.ImagenUrl = producto.ImagenUrl;
        }

        return true;
    }
}
