using CapysBakery.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Registra los servicios de MVC (Controllers + Views)
builder.Services.AddControllersWithViews();

// Fuente de datos de productos. Hoy es la versión "mock" (datos fijos en
// memoria) porque la base de datos aún no existe. El día que se conecte
// SQL Server, se cambia SOLO esta línea por la implementación real con
// Entity Framework (ej. builder.Services.AddScoped<IProductoRepository, EfProductoRepository>())
// y ningún Controller ni View se modifica.
builder.Services.AddSingleton<IProductoRepository, MockProductoRepository>();

// A partir de Sprint 1, aquí se agregan:
// - DbContext (Entity Framework Core) apuntando a SQL Server
// - ASP.NET Core Identity (roles: Cliente, Administrador, Dueño)

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
