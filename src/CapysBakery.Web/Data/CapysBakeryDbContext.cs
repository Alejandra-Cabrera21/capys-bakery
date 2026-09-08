using CapysBakery.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CapysBakery.Web.Data;

// El "traductor" entre las clases de Models/ y las tablas reales de
// CapysBakeryDb. Aquí se configuran los nombres exactos de tabla/columna
// (snake_case, igual que en database/scripts/*.sql) para que todo calce.
//
// Nota de alcance: no todas las tablas documentadas tienen ya una pantalla
// que las use (ej. tipo_personalizacion/opcion_personalizacion — las
// opciones de "color"/"toppings" en el detalle de producto todavía son
// solo decorativas). Se mapean de todas formas para que el modelo de datos
// completo quede reflejado, tal como se documentó en el diseño de BD.
public class CapysBakeryDbContext : DbContext
{
    public CapysBakeryDbContext(DbContextOptions<CapysBakeryDbContext> options) : base(options) { }

    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Alergeno> Alergenos => Set<Alergeno>();
    public DbSet<ProductoPresentacion> ProductoPresentaciones => Set<ProductoPresentacion>();
    public DbSet<ImagenProducto> ImagenesProducto => Set<ImagenProducto>();
    public DbSet<TipoPersonalizacion> TiposPersonalizacion => Set<TipoPersonalizacion>();
    public DbSet<OpcionPersonalizacion> OpcionesPersonalizacion => Set<OpcionPersonalizacion>();
    public DbSet<ProductoOpcionPersonalizacion> ProductoOpcionesPersonalizacion => Set<ProductoOpcionPersonalizacion>();

    public DbSet<ModalidadEntrega> ModalidadesEntrega => Set<ModalidadEntrega>();
    public DbSet<MetodoPago> MetodosPago => Set<MetodoPago>();
    public DbSet<CuentaBancaria> CuentasBancarias => Set<CuentaBancaria>();

    public DbSet<EstadoPedido> EstadosPedido => Set<EstadoPedido>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<PedidoDetalle> PedidoDetalles => Set<PedidoDetalle>();
    public DbSet<PedidoDetallePersonalizacion> PedidoDetallePersonalizaciones => Set<PedidoDetallePersonalizacion>();
    public DbSet<HistorialEstadoPedido> HistorialEstadoPedido => Set<HistorialEstadoPedido>();

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    // Publicacion (blog): NO forma parte del diseño original de 18 tablas —
    // es una extensión para la función de blog que se agregó después. Se
    // documenta aquí explícitamente para que quede claro el porqué.
    public DbSet<Publicacion> Publicaciones => Set<Publicacion>();
    public DbSet<ComentarioPublicacion> ComentariosPublicacion => Set<ComentarioPublicacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ================= 5.1 Catálogo de productos =================

        modelBuilder.Entity<Categoria>(e =>
        {
            e.ToTable("categoria");
            e.HasKey(c => c.Id);
            e.Property(c => c.Id).HasColumnName("id_categoria");
            e.Property(c => c.Nombre).HasColumnName("nombre").HasMaxLength(80).IsRequired();
            e.Property(c => c.Disponible).HasColumnName("disponible").IsRequired();
            e.HasIndex(c => c.Nombre).IsUnique();
        });

        modelBuilder.Entity<Producto>(e =>
        {
            e.ToTable("producto");
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).HasColumnName("id_producto");
            e.Property(p => p.Nombre).HasColumnName("nombre").HasMaxLength(120).IsRequired();
            e.Property(p => p.Descripcion).HasColumnName("descripcion").IsRequired();
            e.Property(p => p.Disponible).HasColumnName("disponible");

            // Columnas que NO estaban en el diseño original de 18 tablas,
            // agregadas como extensión pragmática ya usada por la app
            // (AdminProductosController): precio "desde" (denormalizado,
            // se recalcula al guardar), promoción, y auditoría básica —
            // esta última ya estaba anotada como "consideración futura" en
            // la sección 8 del documento de diseño.
            e.Property(p => p.Precio).HasColumnName("precio").HasColumnType("decimal(10,2)");
            e.Property(p => p.EsPromocion).HasColumnName("es_promocion");
            e.Property(p => p.CreadoPorCorreo).HasColumnName("creado_por_correo").HasMaxLength(150);
            e.Property(p => p.FechaCreacion).HasColumnName("fecha_creacion");

            // Propiedades de conveniencia (no son columnas).
            e.Ignore(p => p.CategoriaPrincipal);
            e.Ignore(p => p.ImagenUrl);

            // N:M con categoria vía producto_categoria (tabla intermedia sin columnas extra).
            e.HasMany(p => p.Categorias)
                .WithMany(c => c.Productos)
                .UsingEntity<Dictionary<string, object>>(
                    "producto_categoria",
                    j => j.HasOne<Categoria>().WithMany().HasForeignKey("id_categoria").HasConstraintName("FK_prodcat_categoria"),
                    j => j.HasOne<Producto>().WithMany().HasForeignKey("id_producto").HasConstraintName("FK_prodcat_producto"),
                    j => j.HasKey("id_producto", "id_categoria"));

            // N:M con alergeno vía producto_alergeno.
            e.HasMany(p => p.Alergenos)
                .WithMany(a => a.Productos)
                .UsingEntity<Dictionary<string, object>>(
                    "producto_alergeno",
                    j => j.HasOne<Alergeno>().WithMany().HasForeignKey("id_alergeno").HasConstraintName("FK_prodalerg_alergeno"),
                    j => j.HasOne<Producto>().WithMany().HasForeignKey("id_producto").HasConstraintName("FK_prodalerg_producto"),
                    j => j.HasKey("id_producto", "id_alergeno"));

            e.HasMany(p => p.Presentaciones).WithOne(pp => pp.Producto).HasForeignKey(pp => pp.ProductoId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(p => p.Imagenes).WithOne().HasForeignKey(ip => ip.ProductoId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(p => p.OpcionesPersonalizacion).WithOne().HasForeignKey(o => o.ProductoId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProductoPresentacion>(e =>
        {
            e.ToTable("producto_presentacion");
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).HasColumnName("id_presentacion");
            e.Property(p => p.ProductoId).HasColumnName("id_producto");
            e.Property(p => p.Nombre).HasColumnName("nombre").HasMaxLength(80).IsRequired();
            e.Property(p => p.Porciones).HasColumnName("porciones");
            e.Property(p => p.Precio).HasColumnName("precio").HasColumnType("decimal(10,2)").IsRequired();
        });

        modelBuilder.Entity<ImagenProducto>(e =>
        {
            e.ToTable("imagen_producto");
            e.HasKey(i => i.Id);
            e.Property(i => i.Id).HasColumnName("id_imagen");
            e.Property(i => i.ProductoId).HasColumnName("id_producto");
            e.Property(i => i.UrlImagen).HasColumnName("url_imagen").HasMaxLength(500).IsRequired();
            e.Property(i => i.Orden).HasColumnName("orden");
            e.Property(i => i.EsPrincipal).HasColumnName("es_principal");
        });

        modelBuilder.Entity<Alergeno>(e =>
        {
            e.ToTable("alergeno");
            e.HasKey(a => a.Id);
            e.Property(a => a.Id).HasColumnName("id_alergeno");
            e.Property(a => a.Nombre).HasColumnName("nombre").HasMaxLength(80).IsRequired();
            e.HasIndex(a => a.Nombre).IsUnique();
        });

        // ================= 5.2 Personalización (aún sin pantalla) =================

        modelBuilder.Entity<TipoPersonalizacion>(e =>
        {
            e.ToTable("tipo_personalizacion");
            e.HasKey(t => t.Id);
            e.Property(t => t.Id).HasColumnName("id_tipo_personalizacion");
            e.Property(t => t.Nombre).HasColumnName("nombre").HasMaxLength(80).IsRequired();
            e.HasIndex(t => t.Nombre).IsUnique();
            e.HasMany(t => t.Opciones).WithOne(o => o.TipoPersonalizacion).HasForeignKey(o => o.TipoPersonalizacionId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OpcionPersonalizacion>(e =>
        {
            e.ToTable("opcion_personalizacion");
            e.HasKey(o => o.Id);
            e.Property(o => o.Id).HasColumnName("id_opcion");
            e.Property(o => o.TipoPersonalizacionId).HasColumnName("id_tipo_personalizacion");
            e.Property(o => o.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<ProductoOpcionPersonalizacion>(e =>
        {
            e.ToTable("producto_opcion_personalizacion");
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).HasColumnName("id_producto_opcion");
            e.Property(p => p.ProductoId).HasColumnName("id_producto");
            e.Property(p => p.OpcionId).HasColumnName("id_opcion");
            e.Property(p => p.PrecioAdicional).HasColumnName("precio_adicional").HasColumnType("decimal(10,2)");
            e.Property(p => p.Disponible).HasColumnName("disponible");
            e.HasOne(p => p.Opcion).WithMany().HasForeignKey(p => p.OpcionId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(p => new { p.ProductoId, p.OpcionId }).IsUnique();
        });

        // ================= 5.3 Entrega y pago =================

        modelBuilder.Entity<ModalidadEntrega>(e =>
        {
            e.ToTable("modalidad_entrega");
            e.HasKey(m => m.Id);
            e.Property(m => m.Id).HasColumnName("id_modalidad_entrega");
            e.Property(m => m.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
            e.Property(m => m.RequiereDireccion).HasColumnName("requiere_direccion");
            e.HasIndex(m => m.Nombre).IsUnique();
        });

        modelBuilder.Entity<MetodoPago>(e =>
        {
            e.ToTable("metodo_pago");
            e.HasKey(m => m.Id);
            e.Property(m => m.Id).HasColumnName("id_metodo_pago");
            e.Property(m => m.Nombre).HasColumnName("nombre").HasMaxLength(60).IsRequired();
            e.Property(m => m.SoloRecoger).HasColumnName("solo_recoger");
            e.Property(m => m.Disponible).HasColumnName("disponible");
            e.HasIndex(m => m.Nombre).IsUnique();
        });

        modelBuilder.Entity<CuentaBancaria>(e =>
        {
            e.ToTable("cuenta_bancaria");
            e.HasKey(c => c.Id);
            e.Property(c => c.Id).HasColumnName("id_cuenta_bancaria");
            e.Property(c => c.MetodoPagoId).HasColumnName("id_metodo_pago");
            e.Property(c => c.Banco).HasColumnName("banco").HasMaxLength(100).IsRequired();
            e.Property(c => c.TipoCuenta).HasColumnName("tipo_cuenta").HasMaxLength(50).IsRequired();
            e.Property(c => c.NumeroCuenta).HasColumnName("numero_cuenta").HasMaxLength(50).IsRequired();
            e.Property(c => c.Titular).HasColumnName("titular").HasMaxLength(150).IsRequired();
            e.Property(c => c.Disponible).HasColumnName("disponible");
        });

        // ================= 5.4 Pedidos =================

        modelBuilder.Entity<EstadoPedido>(e =>
        {
            e.ToTable("estado_pedido");
            e.HasKey(s => s.Id);
            e.Property(s => s.Id).HasColumnName("id_estado_pedido");
            e.Property(s => s.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
            e.HasIndex(s => s.Nombre).IsUnique();
        });

        modelBuilder.Entity<Pedido>(e =>
        {
            e.ToTable("pedido");
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).HasColumnName("id_pedido");
            e.Property(p => p.CodigoPedido).HasColumnName("codigo_pedido").HasMaxLength(30).IsRequired();
            e.Property(p => p.NombreCliente).HasColumnName("nombre_cliente").HasMaxLength(150).IsRequired();
            e.Property(p => p.TelefonoCliente).HasColumnName("telefono_cliente").HasMaxLength(25).IsRequired();
            e.Property(p => p.FechaEntregaSolicitada).HasColumnName("fecha_entrega_solicitada").HasColumnType("date");
            e.Property(p => p.ModalidadEntregaId).HasColumnName("id_modalidad_entrega");
            e.Property(p => p.DireccionOPuntoEntrega).HasColumnName("direccion_o_punto_entrega").HasMaxLength(300);
            e.Property(p => p.MetodoPagoId).HasColumnName("id_metodo_pago");
            e.Property(p => p.EstadoPedidoId).HasColumnName("id_estado_pedido");
            e.Property(p => p.Comentarios).HasColumnName("comentarios");
            e.Property(p => p.FechaRegistro).HasColumnName("fecha_registro");

            // Columna que NO estaba en el diseño original: ya la anotaba
            // el propio documento como "consideración futura" en cuanto
            // existieran cuentas de usuario (sección 5.4.2). Nullable para
            // no perder pedidos de invitados si algún día se permitieran.
            e.Property(p => p.UsuarioId).HasColumnName("id_usuario");

            e.HasIndex(p => p.CodigoPedido).IsUnique();
            e.Ignore(p => p.Total);

            e.HasOne(p => p.ModalidadEntrega).WithMany().HasForeignKey(p => p.ModalidadEntregaId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.MetodoPago).WithMany().HasForeignKey(p => p.MetodoPagoId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.EstadoPedido).WithMany().HasForeignKey(p => p.EstadoPedidoId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.Usuario).WithMany().HasForeignKey(p => p.UsuarioId).OnDelete(DeleteBehavior.Restrict);

            e.HasMany(p => p.Detalles).WithOne().HasForeignKey(d => d.PedidoId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(p => p.Historial).WithOne().HasForeignKey(h => h.PedidoId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PedidoDetalle>(e =>
        {
            e.ToTable("pedido_detalle");
            e.HasKey(d => d.Id);
            e.Property(d => d.Id).HasColumnName("id_detalle_pedido");
            e.Property(d => d.PedidoId).HasColumnName("id_pedido");
            e.Property(d => d.PresentacionId).HasColumnName("id_presentacion");
            e.Property(d => d.Cantidad).HasColumnName("cantidad");
            e.Property(d => d.PrecioUnitario).HasColumnName("precio_unitario").HasColumnType("decimal(10,2)");

            e.HasOne(d => d.Presentacion).WithMany().HasForeignKey(d => d.PresentacionId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(d => d.Personalizaciones).WithOne().HasForeignKey(p => p.PedidoDetalleId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PedidoDetallePersonalizacion>(e =>
        {
            e.ToTable("pedido_detalle_personalizacion");
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).HasColumnName("id_detalle_personalizacion");
            e.Property(p => p.PedidoDetalleId).HasColumnName("id_detalle_pedido");
            e.Property(p => p.ProductoOpcionId).HasColumnName("id_producto_opcion");
            e.Property(p => p.PrecioAdicionalUnitario).HasColumnName("precio_adicional_unitario").HasColumnType("decimal(10,2)");

            e.HasOne(p => p.ProductoOpcion).WithMany().HasForeignKey(p => p.ProductoOpcionId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<HistorialEstadoPedido>(e =>
        {
            e.ToTable("historial_estado_pedido");
            e.HasKey(h => h.Id);
            e.Property(h => h.Id).HasColumnName("id_historial_pedido");
            e.Property(h => h.PedidoId).HasColumnName("id_pedido");
            e.Property(h => h.EstadoPedidoId).HasColumnName("id_estado_pedido");
            e.Property(h => h.FechaCambio).HasColumnName("fecha_cambio");

            e.HasOne(h => h.EstadoPedido).WithMany().HasForeignKey(h => h.EstadoPedidoId).OnDelete(DeleteBehavior.Restrict);
        });

        // ================= Usuarios y roles =================

        modelBuilder.Entity<Usuario>(e =>
        {
            e.ToTable("usuario");
            e.HasKey(u => u.Id);
            e.Property(u => u.Id).HasColumnName("id_usuario");
            e.Property(u => u.Nombre).HasColumnName("nombre").HasMaxLength(150).IsRequired();
            e.Property(u => u.Correo).HasColumnName("correo").HasMaxLength(150).IsRequired();
            e.Property(u => u.Telefono).HasColumnName("telefono").HasMaxLength(25);
            e.Property(u => u.PasswordHash).HasColumnName("password_hash").HasMaxLength(200).IsRequired();

            // El enum se guarda como texto ("Cliente"/"Administrador"/"Dueño"),
            // igual que el CHECK ya definido en la columna rol.
            e.Property(u => u.Rol).HasColumnName("rol").HasMaxLength(20).HasConversion<string>().IsRequired();

            e.Property(u => u.CreadaPorCorreo).HasColumnName("creada_por_correo").HasMaxLength(150);
            e.Property(u => u.FechaRegistro).HasColumnName("fecha_registro");

            e.HasIndex(u => u.Correo).IsUnique();
        });

        // ================= Blog (fuera del diseño original de 18 tablas) =================

        modelBuilder.Entity<Publicacion>(e =>
        {
            e.ToTable("publicacion");
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).HasColumnName("id_publicacion");
            e.Property(p => p.Titulo).HasColumnName("titulo").HasMaxLength(200).IsRequired();
            e.Property(p => p.Categoria).HasColumnName("categoria").HasMaxLength(80);
            e.Property(p => p.Resumen).HasColumnName("resumen").HasMaxLength(300);
            e.Property(p => p.Contenido).HasColumnName("contenido").IsRequired();
            e.Property(p => p.ImagenUrl).HasColumnName("imagen_url").HasMaxLength(500);
            e.Property(p => p.Publicada).HasColumnName("publicada");
            e.Property(p => p.AutorCorreo).HasColumnName("autor_correo").HasMaxLength(150);
            e.Property(p => p.FechaPublicacion).HasColumnName("fecha_publicacion");
        });

        modelBuilder.Entity<ComentarioPublicacion>(e =>
        {
            e.ToTable("comentario_publicacion");
            e.HasKey(c => c.Id);
            e.Property(c => c.Id).HasColumnName("id_comentario");
            e.Property(c => c.PublicacionId).HasColumnName("id_publicacion");
            e.Property(c => c.UsuarioId).HasColumnName("id_usuario");
            e.Property(c => c.Contenido).HasColumnName("contenido").HasMaxLength(1000).IsRequired();
            e.Property(c => c.FechaCreacion).HasColumnName("fecha_creacion");

            e.HasOne<Publicacion>().WithMany().HasForeignKey(c => c.PublicacionId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(c => c.Usuario).WithMany().HasForeignKey(c => c.UsuarioId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
