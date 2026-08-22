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

    public List<Producto> ObtenerTodos() => _productos;

    public List<Producto> ObtenerDestacados(int cantidad) => _productos.Take(cantidad).ToList();

    public List<Producto> ObtenerPromociones() => _productos.Where(p => p.EsPromocion).ToList();

    public Producto? ObtenerPorId(int id) => _productos.FirstOrDefault(p => p.Id == id);

    public List<Producto> ObtenerPorCategoria(string categoria) =>
        _productos.Where(p => p.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase)).ToList();

    public List<Categoria> ObtenerCategorias() => _categorias;
}
