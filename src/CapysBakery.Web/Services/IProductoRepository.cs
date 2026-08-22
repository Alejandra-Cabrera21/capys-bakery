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
}
