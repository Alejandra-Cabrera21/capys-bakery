using CapysBakery.Web.Models;

namespace CapysBakery.Web.Services;

// Esta interfaz es la pieza clave: los controladores solo conocen ESTE contrato.
// Hoy la implementa MockProductoRepository (datos fijos en memoria).
// Cuando la base de datos esté lista, se crea EfProductoRepository que
// implemente lo mismo usando Entity Framework, y se cambia UNA línea en
// Program.cs (el registro de DI) — nada más se toca.
public interface IProductoRepository
{
    List<Producto> ObtenerTodos();
    List<Producto> ObtenerDestacados(int cantidad);
    List<Producto> ObtenerPromociones();
    Producto? ObtenerPorId(int id);
    List<Producto> ObtenerPorCategoria(string categoria);
    List<Categoria> ObtenerCategorias();

    // Usados por el panel de administración (Dueño o Administrador/vendedor)
    // para publicar y mantener el catálogo. Hoy escriben sobre la lista en
    // memoria; cuando exista Entity Framework, la implementación real hace
    // INSERT/UPDATE contra SQL Server sin que los controladores cambien.
    Producto Agregar(Producto producto);
    bool Actualizar(Producto producto);

    // Incluye productos no disponibles — a diferencia de ObtenerTodos(),
    // pensado para el catálogo público. El panel de administración necesita
    // ver también lo que está oculto para poder reactivarlo.
    List<Producto> ObtenerTodosIncluyendoNoDisponibles();
}
