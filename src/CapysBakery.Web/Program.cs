using CapysBakery.Web.Data;
using CapysBakery.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Registra los servicios de MVC (Controllers + Views)
builder.Services.AddControllersWithViews();

// Conexión real a SQL Server (Fase 5).
builder.Services.AddDbContext<CapysBakeryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// FASE 6: a partir de aquí, la app YA lee y escribe de verdad en
// CapysBakeryDb — se reemplazó cada Mock...Repository (datos fijos en
// memoria) por su versión Ef...Repository (Entity Framework Core sobre el
// DbContext de arriba). Ningún Controller ni View tuvo que cambiar para
// esto, que era justo el punto de programar contra las interfaces desde
// el principio.
//
// AddScoped (no AddSingleton): el DbContext es "scoped" por convención de
// ASP.NET Core (una instancia por solicitud HTTP), así que los
// repositorios que lo usan deben tener el mismo ciclo de vida.
builder.Services.AddScoped<IProductoRepository, EfProductoRepository>();
builder.Services.AddScoped<IUsuarioRepository, EfUsuarioRepository>();
builder.Services.AddScoped<IPublicacionRepository, EfPublicacionRepository>();
builder.Services.AddScoped<IEntregaPagoRepository, EfEntregaPagoRepository>();
builder.Services.AddScoped<IPedidoRepository, EfPedidoRepository>();

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
