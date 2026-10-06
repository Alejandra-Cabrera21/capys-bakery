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

    // El formulario de publicar producto sigue pidiendo la categoría y los
    // alérgenos como texto simple (un campo de categoría, una lista separada
    // por comas para alérgenos) para no complicar la interfaz. Estos
    // métodos traducen ese texto a las entidades reales (Categoria,
    // Alergeno), creándolas en el catálogo compartido si todavía no existen
    // — igual que ya hacía Agregar() para categorías antes de este cambio.
    Categoria ObtenerOCrearCategoria(string nombre);
    List<Alergeno> ObtenerOCrearAlergenos(List<string> nombres);

    // Gestión directa de categorías (crear/editar/desactivar), tal como
    // el cliente confirmó que necesita en "Análisis funcional de las
    // categorías" — independiente de crearlas "al vuelo" al publicar un
    // producto.
    List<Categoria> ObtenerTodasLasCategorias(); // incluye las desactivadas
    Categoria? ObtenerCategoriaPorId(int id);
    Categoria CrearCategoria(string nombre);
    bool ActualizarCategoria(int id, string nuevoNombre, bool disponible);

    // Elimina la imagen de un slot (1 a 5) de un producto ya publicado,
    // sin tocar las demás. El slot 1 es la principal — se puede borrar
    // igual; si un producto se queda sin ninguna imagen, la vista ya
    // maneja ese caso mostrando el ícono de marcador (mismo comportamiento
    // que un producto que nunca tuvo fotos).
    bool EliminarImagen(int productoId, int slot);

    // Catálogo global de personalización (tipos + sus opciones: Decoración
    // -> Ciruela/Dorado/..., Topping -> Flores comestibles/...), para que
    // el panel de AdminProductos pueda ofrecer checkboxes de qué opciones
    // aplican a cada producto y a qué precio adicional (ver
    // ProductoOpcionPersonalizacion). Antes esto no lo usaba ninguna
    // pantalla; ahora es lo que reemplaza los botones de color/toppings
    // fijos de Catalogo/Detalle.
    List<TipoPersonalizacion> ObtenerTiposPersonalizacion();
}
