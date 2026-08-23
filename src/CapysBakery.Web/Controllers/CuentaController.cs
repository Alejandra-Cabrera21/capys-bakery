using System.Security.Claims;
using CapysBakery.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

// Autenticación real vía cookie (ver Program.cs). Mientras no exista
// ASP.NET Core Identity + base de datos, los usuarios se validan contra
// MockUsuarioRepository (en memoria). El contrato (IUsuarioRepository) es
// el mismo que usará la implementación real, así que este controlador no
// debería necesitar cambios cuando se conecte SQL Server.
public class CuentaController : Controller
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IProductoRepository _productoRepository;

    public CuentaController(IUsuarioRepository usuarioRepository, IProductoRepository productoRepository)
    {
        _usuarioRepository = usuarioRepository;
        _productoRepository = productoRepository;
    }

    // GET /Cuenta/Login
    public IActionResult Login(string? returnUrl)
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    // POST /Cuenta/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string correo, string password, string? returnUrl)
    {
        var usuario = _usuarioRepository.ValidarCredenciales(correo, password);
        if (usuario is null)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Error = "Correo o contraseña incorrectos.";
            return View();
        }

        await IniciarSesionAsync(usuario.Id, usuario.Nombre, usuario.Correo, usuario.Rol.ToString());

        // El carrito ya vive en localStorage y sigue ahí después del login
        // (no se pierde); solo redirigimos de vuelta a donde el visitante
        // intentaba llegar, tal como pide la especificación de roles.
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    // GET /Cuenta/Registro
    public IActionResult Registro(string? returnUrl)
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    // POST /Cuenta/Registro
    // El autorregistro público SIEMPRE crea una cuenta de Cliente. Crear
    // cuentas de Administrador (vendedor) o Dueño solo lo puede hacer el
    // Dueño desde el panel (ver AdminUsuariosController).
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registro(string nombre, string correo, string? telefono, string password, string? returnUrl)
    {
        if (_usuarioRepository.ExisteCorreo(correo))
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Error = "Ya existe una cuenta con ese correo.";
            return View();
        }

        var usuario = _usuarioRepository.RegistrarCliente(nombre, correo, telefono, password);
        await IniciarSesionAsync(usuario.Id, usuario.Nombre, usuario.Correo, usuario.Rol.ToString());

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    // POST /Cuenta/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    // GET /Cuenta/MisPedidos — historial del comprador (rol Cliente y
    // también accesible para Administrador/Dueño sobre su propia cuenta).
    // Nota: mientras los pedidos se guarden en localStorage (ver
    // checkout.js) esta vista solo puede filtrar los pedidos hechos desde
    // ESTE navegador; el historial completo entre dispositivos requiere la
    // tabla pedido en SQL Server, ya documentada en el diseño de base de
    // datos (id_pedido, id_usuario en consideraciones futuras).
    [Authorize]
    public IActionResult MisPedidos()
    {
        ViewBag.CorreoUsuario = User.FindFirstValue(ClaimTypes.Email);
        ViewBag.NombreUsuario = User.Identity?.Name;
        return View();
    }

    // GET /Cuenta/MisFavoritos — productos que el comprador marcó con el
    // corazón desde la página de detalle (ver configurador.js). Igual que
    // los favoritos se guardan solo como una lista de ids en localStorage
    // (no en BD todavía), esta vista le pasa a mis-favoritos.js el catálogo
    // completo para que arme las tarjetas (nombre, precio, imagen) de los
    // productos que coincidan con esos ids.
    [Authorize]
    public IActionResult MisFavoritos()
    {
        var productos = _productoRepository.ObtenerTodos();
        return View(productos);
    }

    private async Task IniciarSesionAsync(int id, string nombre, string correo, string rol)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, id.ToString()),
            new(ClaimTypes.Name, nombre),
            new(ClaimTypes.Email, correo),
            new(ClaimTypes.Role, rol),
        };
        var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identidad));
    }
}
