# Código fuente

Esta carpeta está vacía a propósito. El proyecto **ASP.NET Core MVC** se crea aquí mismo cuando el equipo lo genere desde Visual Studio (`File → New Project → ASP.NET Core Web App (Model-View-Controller)`, nombrado `CapysBakery.Web`, con esta carpeta `src/` como ubicación).

No pre-creamos `Controllers/`, `Views/`, `Models/`, etc. a mano porque Visual Studio los genera automáticamente con los archivos reales del proyecto (`.csproj`, `Program.cs`, `_Layout.cshtml`, Bootstrap ya instalado, etc.). Crearlos antes solo generaría carpetas vacías que luego se mezclarían con las reales sin aportar nada.

Lo que sí documentamos de antemano es **el plan de organización**, para que cuando el proyecto exista, cada quien sepa exactamente dónde trabaja.

## Plan de Controllers y Views por módulo

| Controller (a crear) | Responsable | Vistas relacionadas |
|---|---|---|
| `HomeController` | Sergio | Views/Home |
| `CuentaController` (login/registro, Identity) | Sergio | Views/Cuenta |
| `CatalogoController` | Alejandra | Views/Catalogo |
| `CarritoController` | Alejandra | Views/Carrito |
| `CheckoutController` (incluye redirección a WhatsApp) | Angie | Views/Checkout |
| `AdminController` | Rafa | Views/Admin |
| `DuenoController` | Sergio | Views/Dueño |
| `NosotrosController`, `EventosController`, `BlogController`, `ContactoController` | Sergio | Views/Nosotros, Eventos, Blog, Contacto |

## Otras carpetas que se generarán dentro de `CapysBakery.Web/`

- **`Models/`** — Clases de datos y ViewModels (Producto, Pedido, Usuario, etc.).
- **`Data/`** — DbContext de Entity Framework Core y conexión a SQL Server, o la capa de Repository + Stored Procedures si el equipo elige ese enfoque.
- **`wwwroot/`** — CSS, JS e imágenes. Bootstrap ya viene incluido en la plantilla de Visual Studio; sobre esa base se aplica la identidad visual definida en `../design/mockups/` (paleta cálida: crema `#FBF1E4`, dorado `#C9982E`, ciruela `#7C2B3B`; tipografía *Fraunces* para títulos).

## Roles y autenticación

Gestionados con **ASP.NET Core Identity** (Visitante = sin autenticar, Cliente, Administrador, Dueño). Detalle completo de permisos en [`../docs/capys-roles-especificacion.docx`](../docs/capys-roles-especificacion.docx).

## Acceso a datos

El equipo decide en Sprint 0 entre **Entity Framework Core 10** o **Repository + Stored Procedures**. Cualquiera de las dos convive bien con SQL Server; lo importante es que todo el equipo use el mismo enfoque para no mezclar estilos dentro del mismo proyecto.
