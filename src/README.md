# Código fuente

Proyecto **ASP.NET Core MVC** (.NET 10), en `CapysBakery.Web/`.

## Ya construido (funciona sin base de datos)

| Módulo | Controller | Vistas | Cómo obtiene datos |
|---|---|---|---|
| Inicio | `HomeController` | Views/Home | `IProductoRepository` (mock) |
| Catálogo + Detalle/Configurador | `CatalogoController` | Views/Catalogo | `IProductoRepository` (mock) |
| Carrito | `CarritoController` | Views/Carrito | `localStorage` (JS, `carrito.js`) |
| Checkout (datos cliente + WhatsApp + confirmación) | `CheckoutController` | Views/Checkout | `localStorage` + genera link `wa.me` (JS, `checkout.js`) |
| Cuenta (login/registro) | `CuentaController` | Views/Cuenta | Interfaz sin lógica real (pendiente de Identity) |
| Gestión de pedidos (Administrador) | `AdminController` | Views/Admin/Pedidos | `localStorage` — **solo pedidos del mismo navegador**, ver nota abajo |
| Nosotros, Eventos, Blog, Contacto | `NosotrosController`, `EventosController`, `BlogController`, `ContactoController` | Views/(mismo nombre) | Contenido fijo en la vista |

## Alineado con los documentos de análisis funcional del cliente

Esta versión incorpora las precisiones que el cliente confirmó en sus documentos de análisis funcional:

- **Número de WhatsApp real**: `+502 4803 6717` (personal del cliente, mientras no tenga uno dedicado al negocio) — configurado en `checkout.js`.
- **Terminología exacta**: "Envío" y "Recoger" (no "domicilio"/"tienda"), y "Comentarios" (no "Notas"), tal como los nombra el cliente.
- **Transferencia bancaria** ahora muestra Banco, Tipo de cuenta, Número de cuenta y Titular (datos de ejemplo — `TODO` marcado para reemplazarlos por los reales del Dueño).
- **"Pago al recoger"** solo aparece si la forma de entrega es "Recoger" — nunca para "Envío" (regla explícita del cliente).
- **Estados del pedido completos**: Pendiente → Confirmado → En preparación → **Listo** → Entregado, o Cancelado (antes faltaba "Listo").
- **Panel de gestión de pedidos** (`/Admin/Pedidos`): el cliente lo definió como funcionalidad **crítica** del MVP (sección 3.9 del documento de alcance). Como no hay base de datos, esta versión solo puede leer pedidos guardados en el `localStorage` del mismo navegador donde se probó la compra — **no ve pedidos de otros clientes/dispositivos**. Está marcado con un aviso visible en la propia página para que nadie lo confunda con la versión real. Cuando exista la base de datos, esto se reemplaza por una consulta real a la tabla `Pedidos`.
- Sin enlaces públicos hacia `/Admin/Pedidos` (según la especificación de roles) — se accede escribiendo la URL directamente mientras se prueba.


## La pieza clave: `Services/IProductoRepository`

Ningún Controller ni View sabe si los productos vienen de una lista fija en memoria o de SQL Server — todos dependen únicamente de la interfaz `IProductoRepository`. Hoy está registrada en `Program.cs` la implementación `MockProductoRepository` (datos fijos). Cuando la base de datos esté lista:

1. Crear `Services/EfProductoRepository.cs` implementando la misma interfaz, usando el `DbContext`.
2. Cambiar **una sola línea** en `Program.cs`:
   ```csharp
   builder.Services.AddSingleton<IProductoRepository, MockProductoRepository>();
   // se cambia por:
   builder.Services.AddScoped<IProductoRepository, EfProductoRepository>();
   ```
3. Ningún Controller ni View se toca.

Este mismo patrón (Repository) se debe repetir para Pedidos, Usuarios, etc. conforme se conecte la base de datos — es la práctica que evita tener que reescribir la aplicación cuando pasamos de datos falsos a datos reales.

## Carrito y Checkout: por qué viven en JavaScript / localStorage

Como se definió en la especificación de roles, el carrito debe funcionar **sin necesidad de cuenta**. Por eso:

- `wwwroot/js/carrito.js` — guarda/lee el carrito en `localStorage` del navegador (nada en el servidor).
- `wwwroot/js/checkout.js` — arma el mensaje del pedido y el link `https://wa.me/...`, y guarda una copia del pedido en `localStorage` para mostrarla en la pantalla de Confirmación.
- El número de WhatsApp está momentáneamente fijo en `checkout.js` (`NUMERO_WHATSAPP`) — hay un `TODO` marcado para moverlo a configuración real cuando exista el panel del Dueño conectado a base de datos.
- La validación de "¿tiene sesión iniciada?" antes de mostrar el formulario de datos del cliente está marcada como `TODO` en `CheckoutController.DatosCliente()` — se activa en Sprint 1 cuando se conecte ASP.NET Core Identity.

## Pendiente (depende de la base de datos)

- Login/registro real (ASP.NET Core Identity).
- Guardar pedidos de forma permanente (hoy solo existen en `localStorage` del navegador de quien compra).
- Panel de Administrador y Dueño con datos reales (`AdminController`, `DuenoController` — aún no creados).
- Reemplazar `MockProductoRepository` por la versión con Entity Framework Core.

## Cómo correrlo

```bash
cd src/CapysBakery.Web
dotnet restore
dotnet run
```
