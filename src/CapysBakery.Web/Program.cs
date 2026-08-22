using CapysBakery.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Registra los servicios de MVC (Controllers + Views)
builder.Services.AddControllersWithViews();

// Fuente de datos de productos. Hoy es la versión "mock" (datos fijos en
// memoria) porque la base de datos aún no existe. El día que se conecte
// SQL Server, se cambia SOLO esta línea por la implementación real con
// Entity Framework (ej. builder.Services.AddScoped<IProductoRepository, EfProductoRepository>())
// y ningún Controller ni View se modifica.
builder.Services.AddSingleton<IProductoRepository, MockProductoRepository>();

// Fuente de datos de usuarios/roles. Misma idea: mock en memoria hoy,
// EfUsuarioRepository + ASP.NET Core Identity real cuando exista SQL Server.
builder.Services.AddSingleton<IUsuarioRepository, MockUsuarioRepository>();

// Autenticación por cookie: permite tener sesión y roles (Cliente,
// Administrador, Dueño) ya funcionando desde ahora, sin depender de que
// exista la base de datos. Cuando se conecte ASP.NET Core Identity, este
// bloque se reemplaza por builder.Services.AddDefaultIdentity<...>(), pero
// los [Authorize(Roles = "...")] de los controladores no cambian.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Cuenta/Login";
        options.AccessDeniedPath = "/Cuenta/Login";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
