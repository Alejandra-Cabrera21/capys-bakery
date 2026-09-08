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
    private readonly IPedidoRepository _pedidoRepository;

    public CuentaController(IUsuarioRepository usuarioRepository, IProductoRepository productoRepository, IPedidoRepository pedidoRepository)
    {
        _usuarioRepository = usuarioRepository;
        _productoRepository = productoRepository;
        _pedidoRepository = pedidoRepository;
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

    // GET /Cuenta/RecuperarPassword — "olvidé mi contraseña". Mientras no
    // exista un servicio de correo para mandar un link real, se verifica
    // identidad con correo + teléfono (ambos ya guardados en la cuenta).
    public IActionResult RecuperarPassword()
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    // POST /Cuenta/RecuperarPassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RecuperarPassword(string correo, string telefono, string passwordNueva, string confirmarPassword)
    {
        if (passwordNueva != confirmarPassword)
        {
            ViewBag.Error = "Las contraseñas no coinciden.";
            return View();
        }

        var listo = _usuarioRepository.RestablecerPassword(correo, telefono, passwordNueva);
        if (!listo)
        {
            ViewBag.Error = "No encontramos una cuenta con ese correo y teléfono. Verifica los datos e intenta de nuevo.";
            return View();
        }

        TempData["Mensaje"] = "Tu contraseña se actualizó. Ya puedes iniciar sesión con la nueva.";
        return RedirectToAction(nameof(Login));
    }

    // GET /Cuenta/MiPerfil
    [Authorize]
    public IActionResult MiPerfil()
    {
        var usuario = _usuarioRepository.ObtenerPorCorreo(User.FindFirstValue(ClaimTypes.Email)!);
        if (usuario is null) return NotFound();
        return View(usuario);
    }

    // POST /Cuenta/CambiarPassword — desde "Mi perfil", ya con sesión.
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public IActionResult CambiarPassword(string passwordActual, string passwordNueva, string confirmarPassword)
    {
        var usuario = _usuarioRepository.ObtenerPorCorreo(User.FindFirstValue(ClaimTypes.Email)!);
        if (usuario is null) return NotFound();

        if (passwordNueva != confirmarPassword)
        {
            ViewBag.Error = "Las contraseñas nuevas no coinciden.";
            return View("MiPerfil", usuario);
        }

        var listo = _usuarioRepository.CambiarPassword(usuario.Id, passwordActual, passwordNueva);
        if (!listo)
        {
            ViewBag.Error = "La contraseña actual no es correcta.";
            return View("MiPerfil", usuario);
        }

        ViewBag.Mensaje = "Tu contraseña se actualizó correctamente.";
        return View("MiPerfil", usuario);
    }

    // GET /Cuenta/MisPedidos — historial del comprador.
    // Fase 6: ya lee de verdad de la base de datos, filtrado por el
    // id_usuario real de la cuenta con sesión iniciada (antes filtraba por
    // un correo guardado en localStorage, y solo veía lo hecho en ESE
    // navegador).
    [Authorize]
    public IActionResult MisPedidos()
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var pedidos = _pedidoRepository.ObtenerPorUsuario(usuarioId);
        return View(pedidos);
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
