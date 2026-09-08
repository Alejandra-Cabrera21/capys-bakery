namespace CapysBakery.Web.Models;

// Los cuatro niveles de acceso definidos en capys-roles-especificacion.docx
// son Visitante, Cliente, Administrador y Dueño. "Visitante" no necesita
// cuenta (es cualquiera sin sesión), así que aquí solo modelamos los tres
// roles que sí requieren una cuenta guardada.
//
// Nota de nombres: en las conversaciones del equipo, al rol "Administrador"
// también se le llama "Vendedor" (quien publica productos y gestiona
// pedidos del día a día). Se deja el nombre "Administrador" en el código
// para que coincida exactamente con la especificación de roles y con la
// tabla de permisos ya documentada; la interfaz puede mostrar "Vendedor"
// como etiqueta amigable.
public enum RolUsuario
{
    Cliente,
    Administrador,
    Dueño
}

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }

    // Hash simple (SHA-256) SOLO para poder probar el flujo de login/roles
    // antes de que exista la base de datos. TODO (Sprint 1): reemplazar por
    // ASP.NET Core Identity con hashing real (PBKDF2/Argon2) en cuanto
    // Identity quede conectado a SQL Server — ninguna vista ni controlador
    // que use IUsuarioRepository debería necesitar cambios más allá de eso.
    public string PasswordHash { get; set; } = string.Empty;

    public RolUsuario Rol { get; set; } = RolUsuario.Cliente;

    // Auditoría: qué cuenta (Dueño) creó esta cuenta de staff, y cuándo.
    // Para cuentas de Cliente (autorregistro) queda null.
    public string? CreadaPorCorreo { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
