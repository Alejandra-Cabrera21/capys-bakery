using System.Security.Cryptography;
using System.Text;
using CapysBakery.Web.Data;
using CapysBakery.Web.Models;

namespace CapysBakery.Web.Services;

// Implementación REAL sobre CapysBakeryDb. Reemplaza a
// MockUsuarioRepository (Fase 6). Las 3 cuentas de prueba ya no viven en
// memoria — son las mismas filas cargadas por database/scripts/004_seed_usuarios.sql.
public class EfUsuarioRepository : IUsuarioRepository
{
    private readonly CapysBakeryDbContext _db;

    public EfUsuarioRepository(CapysBakeryDbContext db)
    {
        _db = db;
    }

    public List<Usuario> ObtenerTodos() =>
        _db.Usuarios.OrderBy(u => u.Rol).ThenBy(u => u.Nombre).ToList();

    public Usuario? ObtenerPorCorreo(string correo) =>
        _db.Usuarios.FirstOrDefault(u => u.Correo.ToLower() == correo.ToLower());

    public bool ExisteCorreo(string correo) => ObtenerPorCorreo(correo) is not null;

    public Usuario RegistrarCliente(string nombre, string correo, string? telefono, string password)
    {
        var usuario = new Usuario
        {
            Nombre = nombre,
            Correo = correo,
            Telefono = telefono,
            Rol = RolUsuario.Cliente,
            PasswordHash = HashPassword(password),
            FechaRegistro = DateTime.Now,
        };
        _db.Usuarios.Add(usuario);
        _db.SaveChanges();
        return usuario;
    }

    public Usuario CrearCuentaConRol(string nombre, string correo, string? telefono, string password, RolUsuario rol, string creadaPorCorreo)
    {
        var usuario = new Usuario
        {
            Nombre = nombre,
            Correo = correo,
            Telefono = telefono,
            Rol = rol,
            PasswordHash = HashPassword(password),
            CreadaPorCorreo = creadaPorCorreo,
            FechaRegistro = DateTime.Now,
        };
        _db.Usuarios.Add(usuario);
        _db.SaveChanges();
        return usuario;
    }

    public Usuario? ValidarCredenciales(string correo, string password)
    {
        var usuario = ObtenerPorCorreo(correo);
        if (usuario is null) return null;
        return usuario.PasswordHash == HashPassword(password) ? usuario : null;
    }

    public bool RestablecerPassword(string correo, string telefono, string passwordNueva)
    {
        var usuario = ObtenerPorCorreo(correo);
        if (usuario is null) return false;

        // El teléfono debe coincidir con el que ya está guardado en la
        // cuenta — es la verificación de identidad mientras no exista un
        // servicio de correo para mandar un link de recuperación real.
        if (string.IsNullOrWhiteSpace(usuario.Telefono) ||
            !usuario.Telefono.Trim().Equals(telefono.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        usuario.PasswordHash = HashPassword(passwordNueva);
        _db.SaveChanges();
        return true;
    }

    public bool CambiarPassword(int usuarioId, string passwordActual, string passwordNueva)
    {
        var usuario = _db.Usuarios.FirstOrDefault(u => u.Id == usuarioId);
        if (usuario is null) return false;
        if (usuario.PasswordHash != HashPassword(passwordActual)) return false;

        usuario.PasswordHash = HashPassword(passwordNueva);
        _db.SaveChanges();
        return true;
    }

    // Mismo algoritmo (SHA-256 en Base64) que usaba MockUsuarioRepository,
    // para que las contraseñas ya guardadas en la tabla usuario sigan
    // funcionando sin tener que recrear las cuentas de prueba.
    private static string HashPassword(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}
