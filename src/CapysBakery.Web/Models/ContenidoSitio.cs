namespace CapysBakery.Web.Models;

// Corresponde a contenido_sitio. No forma parte del diseño original de 18
// tablas — es un almacén simple "clave -> imagen" para que el Dueño/
// Administrador reemplace imágenes de contenido estático del sitio (hero
// del Home, imagen de Nosotros, galería de Eventos) sin tocar código.
public class ContenidoSitio
{
    public int Id { get; set; }
    public string Clave { get; set; } = string.Empty; // ej. "home_hero"
    public string? UrlImagen { get; set; }
    public DateTime FechaActualizacion { get; set; } = DateTime.Now;
}

// Claves fijas conocidas por el sistema — cada una corresponde a un lugar
// específico del sitio. Agregar una imagen nueva en el futuro (ej. otra
// sección) significa agregar una constante aquí + su fila en el panel de
// AdminContenido, nada más.
public static class ClavesContenidoSitio
{
    public const string HomeHero = "home_hero";
    public const string HomeHistoria = "home_historia";
    public const string NosotrosHistoria = "nosotros_historia";
    public const string EventosGaleria1 = "eventos_galeria_1";
    public const string EventosGaleria2 = "eventos_galeria_2";
    public const string EventosGaleria3 = "eventos_galeria_3";
    public const string EventosGaleria4 = "eventos_galeria_4";

    // (Clave, Etiqueta legible) para pintar el panel de administración.
    public static readonly (string Clave, string Etiqueta)[] Todas =
    {
        (HomeHero, "Inicio — Imagen principal (\"hechos a mano\")"),
        (HomeHistoria, "Inicio — \"De la cocina de casa a tu mesa\""),
        (NosotrosHistoria, "Nosotros — \"Todo empezó en una cocina\""),
        (EventosGaleria1, "Eventos y catering — Foto de galería 1"),
        (EventosGaleria2, "Eventos y catering — Foto de galería 2"),
        (EventosGaleria3, "Eventos y catering — Foto de galería 3"),
        (EventosGaleria4, "Eventos y catering — Foto de galería 4"),
    };
}
