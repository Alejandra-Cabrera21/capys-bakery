using System.Security.Cryptography;
using System.Text;
using CapysBakery.Web.Models;

namespace CapysBakery.Web.Services;

// Implementación TEMPORAL mientras no exista base de datos ni ASP.NET Core
// Identity (ver TODO en Program.cs). Los usuarios viven en memoria durante
// la ejecución de la aplicación; se reinician si el proyecto se detiene.
//
// Cuentas de prueba precargadas para poder probar los tres roles sin tener
// que crear nada a mano:
//   Dueño          -> dueño@capysbakery.com   / Dueño123!
//   Administrador  -> vendedor@capysbakery.com / Vendedor123!
//   Cliente        -> cliente@capysbakery.com  / Cliente123!
public class MockUsuarioRepository : IUsuarioRepository
{
    private readonly List<Usuario> _usuarios;
    private int _siguienteId;

    public MockUsuarioRepository()
    {
        _usuarios = new List<Usuario>
        {
            new Usuario
            {
                Id = 1, Nombre = "Capy (Dueño)", Correo = "dueño@capysbakery.com",
                Telefono = "+502 5555 1234", Rol = RolUsuario.Dueño,
                PasswordHash = HashPassword("Dueño123!"),
            },
            new Usuario
            {
                Id = 2, Nombre = "Vendedor de prueba", Correo = "vendedor@capysbakery.com",
                Telefono = "+502 5555 5678", Rol = RolUsuario.Administrador,
                PasswordHash = HashPassword("Vendedor123!"), CreadaPorCorreo = "dueño@capysbakery.com",
            },
            new Usuario
            {
                Id = 3, Nombre = "Cliente de prueba", Correo = "cliente@capysbakery.com",
                Telefono = "+502 5555 9012", Rol = RolUsuario.Cliente,
                PasswordHash = HashPassword("Cliente123!"),
            },
        };
        _siguienteId = _usuarios.Max(u => u.Id) + 1;
    }

    public List<Usuario> ObtenerTodos() => _usuarios.OrderBy(u => u.Rol).ThenBy(u => u.Nombre).ToList();

    public Usuario? ObtenerPorCorreo(string correo) =>
        _usuarios.FirstOrDefault(u => u.Correo.Equals(correo, StringComparison.OrdinalIgnoreCase));

    public bool ExisteCorreo(string correo) => ObtenerPorCorreo(correo) is not null;

    public Usuario RegistrarCliente(string nombre, string correo, string? telefono, string password)
    {
        var usuario = new Usuario
        {
            Id = _siguienteId++,
            Nombre = nombre,
            Correo = correo,
            Telefono = telefono,
            Rol = RolUsuario.Cliente,
            PasswordHash = HashPassword(password),
        };
        _usuarios.Add(usuario);
        return usuario;
    }

    public Usuario CrearCuentaConRol(string nombre, string correo, string? telefono, string password, RolUsuario rol, string creadaPorCorreo)
    {
        var usuario = new Usuario
        {
            Id = _siguienteId++,
            Nombre = nombre,
            Correo = correo,
            Telefono = telefono,
            Rol = rol,
            PasswordHash = HashPassword(password),
            CreadaPorCorreo = creadaPorCorreo,
        };
        _usuarios.Add(usuario);
        return usuario;
    }

    public Usuario? ValidarCredenciales(string correo, string password)
    {
        var usuario = ObtenerPorCorreo(correo);
        if (usuario is null) return null;
        return usuario.PasswordHash == HashPassword(password) ? usuario : null;
    }

    private static string HashPassword(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}
