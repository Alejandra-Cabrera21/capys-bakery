namespace CapysBakery.Web.Models;

// Comentario de un usuario con sesión iniciada en una entrada del blog.
// NO es parte del diseño original de 18 tablas — es una extensión para
// esta funcionalidad (igual que publicacion).
public class ComentarioPublicacion
{
    public int Id { get; set; }
    public int PublicacionId { get; set; }

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public string Contenido { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
}
