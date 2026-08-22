using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

// TODO (cuando exista Identity + roles): proteger este controlador con
// [Authorize(Roles = "Administrador,Dueño")] y mover la ruta a /admin
// sin enlaces visibles desde el sitio público, tal como indica la
// especificación de roles.
public class AdminController : Controller
{
    // GET /Admin/Pedidos
    // IMPORTANTE: mientras no exista base de datos, esta vista lee los
    // pedidos desde localStorage del MISMO navegador donde se hizo la
    // compra (ver checkout.js). Esto es solo para probar el flujo
    // completo de gestión de pedidos definido como funcionalidad crítica
    // del MVP; no refleja pedidos hechos por clientes reales en otros
    // dispositivos. Eso requiere la tabla Pedidos en SQL Server.
    public IActionResult Pedidos()
    {
        return View();
    }
}
