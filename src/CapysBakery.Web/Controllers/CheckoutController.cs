using System.Security.Claims;
using CapysBakery.Web.Models;
using CapysBakery.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapysBakery.Web.Controllers;

// Según capys-roles-especificacion.docx, sección 2: la sesión NUNCA se
// valida antes de que el visitante haga clic en "Finalizar pedido". El
// carrito se arma libremente en localStorage sin cuenta. Por eso la
// validación vive aquí (al entrar a DatosCliente) y no en Carrito.
public class CheckoutController : Controller
{
    private readonly IEntregaPagoRepository _entregaPagoRepository;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IProductoRepository _productoRepository;

    public CheckoutController(
        IEntregaPagoRepository entregaPagoRepository,
        IPedidoRepository pedidoRepository,
        IProductoRepository productoRepository)
    {
        _entregaPagoRepository = entregaPagoRepository;
        _pedidoRepository = pedidoRepository;
        _productoRepository = productoRepository;
    }

    // GET /Checkout/DatosCliente
    [Authorize]
    public IActionResult DatosCliente()
    {
        // [Authorize] ya se encarga de redirigir a /Cuenta/Login con
        // ?ReturnUrl=/Checkout/DatosCliente cuando no hay sesión, gracias a
        // options.LoginPath configurado en Program.cs. Al iniciar sesión o
        // registrarse, CuentaController regresa aquí mismo — el carrito en
        // localStorage no se pierde en el camino.
        ViewBag.NombreUsuario = User.Identity?.Name;
        ViewBag.CorreoUsuario = User.FindFirst(ClaimTypes.Email)?.Value;

        ViewBag.ModalidadesEntrega = _entregaPagoRepository.ObtenerModalidadesEntrega();
        ViewBag.MetodosPago = _entregaPagoRepository.ObtenerMetodosPago();
        ViewBag.CuentaBancaria = _entregaPagoRepository.ObtenerCuentaBancariaPrincipal();

        return View();
    }

    // POST /Checkout/Confirmar
    // Recibe el pedido armado en el navegador (checkout.js) y lo guarda de
    // verdad mediante IPedidoRepository — a diferencia de antes, cuando
    // TODO el pedido vivía únicamente en localStorage. checkout.js sigue
    // generando el mensaje de WhatsApp y mostrando la pantalla de
    // confirmación con lo que esta acción le devuelve.
    //
    // Nota: se omite [ValidateAntiForgeryToken] porque este endpoint recibe
    // JSON vía fetch (no un <form> tradicional); la protección normal de
    // ASP.NET Core contra cookies de otros sitios (SameSite) sigue vigente.
    [HttpPost]
    [Authorize]
    public IActionResult Confirmar([FromBody] PedidoEntradaDto entrada)
    {
        if (entrada.Productos is null || entrada.Productos.Count == 0)
        {
            return BadRequest(new { mensaje = "El carrito está vacío." });
        }

        var modalidad = _entregaPagoRepository.ObtenerModalidadesEntrega()
            .FirstOrDefault(m => m.Nombre == entrada.FormaEntrega);
        var metodoPago = _entregaPagoRepository.ObtenerMetodosPago()
            .FirstOrDefault(m => m.Nombre == entrada.MetodoPago);

        if (modalidad is null || metodoPago is null)
        {
            return BadRequest(new { mensaje = "Forma de entrega o método de pago inválido." });
        }

        var usuarioIdTexto = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int? usuarioId = int.TryParse(usuarioIdTexto, out var id) ? id : null;

        var pedido = new Pedido
        {
            NombreCliente = entrada.Nombre,
            TelefonoCliente = entrada.Telefono,
            FechaEntregaSolicitada = DateTime.TryParse(entrada.Fecha, out var fecha) ? fecha : DateTime.Today,
            ModalidadEntregaId = modalidad.Id,
            DireccionOPuntoEntrega = entrada.Direccion,
            MetodoPagoId = metodoPago.Id,
            Comentarios = entrada.Comentarios,
            UsuarioId = usuarioId,
        };

        foreach (var item in entrada.Productos)
        {
            // El carrito manda el id de la presentación elegida
            // (ver configurador.js); si por algún motivo no llega (ej.
            // "pedir de nuevo" de un pedido muy antiguo), se cae de vuelta
            // a la presentación más económica de ese producto.
            var presentacionId = item.PresentacionId
                ?? _productoRepository.ObtenerPorId(item.Id)?.Presentaciones
                    .OrderBy(p => p.Precio).Select(p => p.Id).FirstOrDefault();

            if (presentacionId is null or 0) continue;

            pedido.Detalles.Add(new PedidoDetalle
            {
                PresentacionId = presentacionId.Value,
                Cantidad = item.Cantidad,
                PrecioUnitario = item.Precio, // precio congelado al momento de la compra
            });
        }

        if (pedido.Detalles.Count == 0)
        {
            return BadRequest(new { mensaje = "No se pudo resolver ningún producto del carrito." });
        }

        _pedidoRepository.CrearPedido(pedido);

        return Json(new
        {
            id = pedido.Id,
            codigoPedido = pedido.CodigoPedido,
            total = pedido.Total,
        });
    }

    // GET /Checkout/Confirmacion
    public IActionResult Confirmacion()
    {
        return View();
    }
}

// --- DTOs de entrada: la forma exacta del JSON que manda checkout.js ---
// No son entidades de base de datos, solo el "molde" para leer el POST.

public class PedidoEntradaDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string FormaEntrega { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public string Fecha { get; set; } = string.Empty;
    public string? Comentarios { get; set; }
    public string MetodoPago { get; set; } = string.Empty;
    public List<PedidoItemEntradaDto> Productos { get; set; } = new();
}

public class PedidoItemEntradaDto
{
    public int Id { get; set; } // id del producto
    public int? PresentacionId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }
}
