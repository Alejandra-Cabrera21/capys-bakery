using CapysBakery.Web.Models;
using CapysBakery.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

// Según la tabla de permisos de capys-roles-especificacion.docx: "Crear o
// eliminar cuentas de administrador" = Sí SOLO para Dueño. Por eso este
// controlador completo exige el rol Dueño, a diferencia de
// AdminProductosController que también permite Administrador.
[Authorize(Roles = "Dueño")]
public class AdminUsuariosController : Controller
{
    private readonly IUsuarioRepository _usuarioRepository;

    public AdminUsuariosController(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    // GET /AdminUsuarios
    public IActionResult Index()
    {
        return View(_usuarioRepository.ObtenerTodos());
    }

    // GET /AdminUsuarios/Crear
    public IActionResult Crear() => View();

    // POST /AdminUsuarios/Crear
    // El Dueño elige si la nueva cuenta es Administrador (vendedor) u otro
    // Dueño. El autorregistro público (CuentaController.Registro) nunca
    // puede asignar estos roles — solo llega aquí desde una acción ya
    // protegida con [Authorize(Roles = "Dueño")].
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(string nombre, string correo, string? telefono, string password, RolUsuario rol)
    {
        if (rol == RolUsuario.Cliente)
        {
            ModelState.AddModelError(string.Empty, "Desde aquí solo se crean cuentas de Vendedor o Dueño. Los clientes se registran ellos mismos desde el sitio.");
            return View();
        }

        if (_usuarioRepository.ExisteCorreo(correo))
        {
            ModelState.AddModelError(string.Empty, "Ya existe una cuenta con ese correo.");
            return View();
        }

        var creadaPor = User.Identity?.Name ?? "Dueño";
        _usuarioRepository.CrearCuentaConRol(nombre, correo, telefono, password, rol, creadaPor);

        TempData["Mensaje"] = $"Cuenta de {(rol == RolUsuario.Dueño ? "dueño" : "vendedor")} creada para {nombre}.";
        return RedirectToAction(nameof(Index));
    }
}
