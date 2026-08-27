using CapysBakery.Web.Models;

namespace CapysBakery.Web.Services;

// Implementación temporal en memoria (ver el mismo TODO que
// MockProductoRepository). Se precarga con las publicaciones de ejemplo
// que antes estaban escritas directamente en Views/Blog/Index.cshtml, para
// no perder ese contenido de demostración al pasar a un modelo real.
public class MockPublicacionRepository : IPublicacionRepository
{
    private readonly List<Publicacion> _publicaciones;

    public MockPublicacionRepository()
    {
        _publicaciones = new List<Publicacion>
        {
            new Publicacion
            {
                Id = 1,
                Titulo = "Cómo hacemos nuestro merengue perfecto para el pie de limón",
                Categoria = "Recetas",
                Resumen = "Después de docenas de intentos fallidos, así es como logramos un merengue firme, brillante y sin que se baje.",
                Contenido = "Después de docenas de intentos fallidos, así es como logramos un merengue firme, brillante y sin que se baje.\n\n" +
                            "El truco principal está en la temperatura del jarabe de azúcar y en batir las claras justo hasta el punto de picos firmes, ni un segundo más. " +
                            "Usamos claras a temperatura ambiente y un tazón completamente libre de grasa — cualquier resto de yema puede arruinar el batido.\n\n" +
                            "Una vez armado, lo doramos con soplete en vez de horno, para controlar mejor el color sin cocinar de más el relleno de abajo.",
                ImagenUrl = null,
                Publicada = true,
                FechaPublicacion = new DateTime(2026, 8, 3),
            },
            new Publicacion
            {
                Id = 2,
                Titulo = "3 formas de decorar con flores comestibles",
                Categoria = "Recetas",
                Resumen = "Guía rápida para principiantes.",
                Contenido = "Guía rápida para principiantes.\n\nLas flores comestibles son una forma fácil de darle un toque especial a cualquier pastel sin necesitar mangas ni boquillas. " +
                            "Aquí van tres formas sencillas: en cascada sobre un lateral, formando una corona en el centro, o esparcidas junto con hojas de menta alrededor del borde.",
                ImagenUrl = null,
                Publicada = true,
                FechaPublicacion = new DateTime(2026, 7, 28),
            },
            new Publicacion
            {
                Id = 3,
                Titulo = "Un día en la cocina de Capys",
                Categoria = "Detrás de cámaras",
                Resumen = "Una jornada completa de horneado.",
                Contenido = "Una jornada completa de horneado.\n\nEmpezamos antes de las 6am pesando harina y horneando los primeros loafs del día. " +
                            "A media mañana se arman los pies, y por la tarde se preparan los pedidos para entrega o recogida del día siguiente.",
                ImagenUrl = null,
                Publicada = true,
                FechaPublicacion = new DateTime(2026, 7, 20),
            },
            new Publicacion
            {
                Id = 4,
                Titulo = "Cómo conservar tu pastel fresco por más días",
                Categoria = "Tips",
                Resumen = "Errores comunes al guardar postres.",
                Contenido = "Errores comunes al guardar postres.\n\nGuardar un pastel recién horneado todavía tibio hace que se condense humedad dentro del empaque, arruinando la textura. " +
                            "Deja enfriar por completo, guarda en un recipiente hermético, y si lleva relleno de crema, refrigéralo — pero sácalo con tiempo antes de servir para que recupere su textura.",
                ImagenUrl = null,
                Publicada = true,
                FechaPublicacion = new DateTime(2026, 7, 12),
            },
        };
    }

    public List<Publicacion> ObtenerPublicadas() =>
        _publicaciones.Where(p => p.Publicada).OrderByDescending(p => p.FechaPublicacion).ToList();

    public List<Publicacion> ObtenerTodas() =>
        _publicaciones.OrderByDescending(p => p.FechaPublicacion).ToList();

    public Publicacion? ObtenerPorId(int id) => _publicaciones.FirstOrDefault(p => p.Id == id);

    public Publicacion Agregar(Publicacion publicacion)
    {
        publicacion.Id = _publicaciones.Count == 0 ? 1 : _publicaciones.Max(p => p.Id) + 1;
        publicacion.FechaPublicacion = DateTime.Now;
        _publicaciones.Add(publicacion);
        return publicacion;
    }

    public bool Actualizar(Publicacion publicacion)
    {
        var existente = _publicaciones.FirstOrDefault(p => p.Id == publicacion.Id);
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

        return true;
    }
}
