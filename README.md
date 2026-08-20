# Capys Bakery — Sitio Web

Sitio web de e-commerce y marca para **Capys Bakery**, una pastelería artesanal (tartaletas, pies, pasteles y mermeladas). Proyecto académico desarrollado en equipo bajo el marco de trabajo **Scrum**.

## Sobre el proyecto

El sitio permite a los clientes navegar el catálogo, personalizar productos (tamaño, color de decoración, toppings/flores comestibles), armar un carrito de compras y finalizar su pedido, el cual se coordina y confirma vía WhatsApp (no hay pasarela de pago en línea real). Incluye además páginas de marca (Inicio, Nosotros, Eventos y Catering, Blog/Recetas, Contacto) y un panel administrativo con distintos niveles de acceso.

Ver el detalle completo de alcance, roles y reglas de negocio en [`docs/capys-roles-especificacion.docx`](docs/capys-roles-especificacion.docx).

## Roles del sistema

| Rol | Descripción |
|---|---|
| **Visitante** | Navega el sitio y arma su carrito sin necesidad de cuenta. |
| **Cliente** | Cuenta registrada; puede finalizar pedidos, ver su historial y guardar favoritos. |
| **Administrador** | Gestiona catálogo, pedidos, blog y ve reportes de ventas. |
| **Dueño** | Todo lo del Administrador, más gestión de cuentas de administrador y configuración del sitio (número de WhatsApp, costos de envío, etc.). |

## Stack técnico

**Backend:** C# · ASP.NET Core MVC · .NET 10 LTS
**Frontend:** Razor Views · HTML5 · CSS3 · Bootstrap · JavaScript
**Base de datos:** SQL Server Express 2025 (desarrollo) · MSSQL del hosting (producción)
**Acceso a datos:** Entity Framework Core 10, o Repository + Stored Procedures (a definir en Sprint 0)
**Autenticación y roles:** ASP.NET Core Identity (Visitante = sin sesión, Cliente, Administrador, Dueño)
**Desarrollo:** Visual Studio (recomendado, por el publish con un clic a MonsterASP.NET) o Visual Studio Code con la extensión C# Dev Kit
**Control de versiones:** Git + GitHub, con el flujo de ramas descrito en [`CONTRIBUTING.md`](CONTRIBUTING.md)
**Hosting inicial:** [MonsterASP.NET](https://www.monsterasp.net/) — plan gratuito, incluye MSSQL 2025 y HTTPS gratis vía Let's Encrypt

## Hosting y despliegue

El plan gratuito de MonsterASP.NET permite:
- Crear el sitio como "ASP.NET Core Web App (Model-View-Controller)" desde su panel de control.
- Publicar directo desde **Visual Studio** usando WebDeploy (clic derecho en el proyecto → Publish → importar el perfil `.publishSettings` descargado del panel de MonsterASP.NET).
- Crear la base de datos MSSQL desde el mismo panel y copiar su cadena de conexión a `appsettings.Production.json` (o como variable de entorno).
- Dominio gratuito tipo `*.runasp.net` mientras no se compre un dominio propio.

⚠️ Según la documentación de la comunidad, el despliegue automático vía GitHub en MonsterASP.NET puede ser inestable — se recomienda usar **WebDeploy desde Visual Studio** como método principal de publicación, al menos para este proyecto.

## Estructura del repositorio

```
capys-bakery/
├── README.md              → Este archivo
├── CONTRIBUTING.md         → Flujo de trabajo con Git y ramas del equipo
├── .gitignore
├── docs/                   → Documentación del proyecto (roles, cronograma, backlog)
├── design/
│   └── mockups/            → Mockups HTML y capturas de las 8 pantallas del sitio
├── database/                → Diagrama entidad-relación y notas del esquema (documentación, no código)
└── src/                      → Vacía por ahora — ver src/README.md para el plan de módulos
```

> `src/` está vacía a propósito: el proyecto ASP.NET Core MVC (`CapysBakery.Web`) se genera ahí mismo desde Visual Studio (`File → New Project`), no se pre-crea a mano. `src/README.md` documenta qué controlador/módulo le toca a cada integrante, para que quede claro apenas exista el proyecto real.

## Cronograma

El proyecto sigue el mismo cronograma Scrum documentado en [`docs/capys-cronograma-sprints.xlsx`](docs/capys-cronograma-sprints.xlsx): inicio 23 de julio de 2026, Sprints 0–6 de dos semanas cada uno, seguidos de Deploy, Support y Closure, con presentación final el 12 de noviembre de 2026.

## Equipo

| Integrante | Rama de trabajo | Módulo principal |
|---|---|---|
| Alejandra | `alejandra` | Catálogo y Carrito |
| Angie | `angie` | Checkout y WhatsApp |
| Rafa | `rafa` | Panel de Administrador |
| Sergio | `sergio` | Autenticación, panel del Dueño y páginas de contenido |

Ver el flujo completo de trabajo con estas ramas en [`CONTRIBUTING.md`](CONTRIBUTING.md).
