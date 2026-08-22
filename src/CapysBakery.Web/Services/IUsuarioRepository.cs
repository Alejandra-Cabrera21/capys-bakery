using CapysBakery.Web.Models;

namespace CapysBakery.Web.Services;

// Mismo patrón que IProductoRepository: los controladores solo conocen
// este contrato. Hoy lo implementa MockUsuarioRepository (en memoria).
// Cuando exista ASP.NET Core Identity + SQL Server, se reemplaza por una
// implementación real sin tocar los controladores.
public interface IUsuarioRepository
{
    List<Usuario> ObtenerTodos();
    Usuario? ObtenerPorCorreo(string correo);
    bool ExisteCorreo(string correo);

    // Autorregistro público: siempre crea el usuario como Cliente.
    Usuario RegistrarCliente(string nombre, string correo, string? telefono, string password);

    // Solo debe invocarse desde una acción protegida con
    // [Authorize(Roles = "Dueño")]: el Dueño es el único que puede crear
    // cuentas de Administrador (vendedor) u otro Dueño.
    Usuario CrearCuentaConRol(string nombre, string correo, string? telefono, string password, RolUsuario rol, string creadaPorCorreo);

    // Devuelve el usuario si la contraseña es correcta; null en caso contrario.
    Usuario? ValidarCredenciales(string correo, string password);
}
