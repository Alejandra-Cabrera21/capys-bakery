# Código fuente

Proyecto **ASP.NET Core MVC** (C#), un solo proyecto para todo el equipo.

- `CapysBakery.Web/Controllers/` — Un controlador por módulo (CatalogoController, CarritoController, CheckoutController, AdminController, DuenoController, CuentaController, etc.)
- `CapysBakery.Web/Views/` — Vistas Razor (.cshtml), organizadas en una carpeta por módulo, alineadas con los mockups en `../design/mockups/`.
- `CapysBakery.Web/Models/` — Clases de datos (Producto, Pedido, Usuario, etc.) y ViewModels.
- `CapysBakery.Web/Data/` — DbContext de Entity Framework Core y configuración de conexión a SQL Server (o capa Repository + Stored Procedures, según lo que decida el equipo).
- `CapysBakery.Web/wwwroot/` — Archivos estáticos: css, js, imágenes. Incluye Bootstrap (vía LibMan o CDN) como base de estilos, sobre la cual se aplica la paleta cálida (crema, dorado, ciruela) definida en los mockups.

Roles gestionados con **ASP.NET Core Identity** (Visitante = sin autenticar, Cliente, Administrador, Dueño). Ver detalle completo en `../docs/capys-roles-especificacion.docx`.

**Acceso a datos:** el equipo decidirá en Sprint 0 entre **Entity Framework Core 10** (Code First o Database First) o un enfoque de **Repository + Stored Procedures**. Cualquiera de las dos opciones convive con SQL Server sin problema; lo importante es que todo el equipo use la misma para evitar mezclar estilos en el mismo proyecto.

El proyecto de Visual Studio / Visual Studio Code (.sln, .csproj) se genera al iniciar el Sprint 0 con `dotnet new mvc`; no se incluye aún en este scaffold porque solo contiene documentación y estructura de carpetas, sin código.
