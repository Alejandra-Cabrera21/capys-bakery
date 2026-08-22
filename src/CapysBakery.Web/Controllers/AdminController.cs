using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

// Protegido según capys-roles-especificacion.docx: "Cambiar estado de
// pedidos" está permitido a Administrador (vendedor) y Dueño, pero no a
// Cliente ni Visitante. Con [Authorize] en el controlador, cualquier
// endpoint nuevo que se agregue aquí queda protegido por defecto.
[Authorize(Roles = "Administrador,Dueño")]
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
