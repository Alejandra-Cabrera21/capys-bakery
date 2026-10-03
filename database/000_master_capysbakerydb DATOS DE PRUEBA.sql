/*
    ============================================================================
    CAPYS DIGITAL BAKERY — SCRIPT MAESTRO DE BASE DE DATOS
    ============================================================================
    Crea CapysBakeryDb completa, de cero, con todo lo que el proyecto usa hoy:

      PARTE 1 — Esquema (CREATE TABLE), en este orden:
        1.1  Las 18 tablas del diseño original v0.1
             (con las columnas de `producto` que la app YA usa en la
             práctica — precio, es_promocion, es_destacado, creado_por_correo,
             fecha_creacion — agregadas ya desde el inicio, en vez de
             como columnas agregadas después con ALTER TABLE)
        1.2  usuario (módulo de cuentas — no estaba en el diseño original)
        1.3  publicacion, comentario_publicacion (blog)
        1.4  mensaje_contacto (formulario de /Contacto)
        1.5  calificacion_pedido (calificación de 1-5 estrellas + testimonio
             destacado del cliente sobre su pedido)
        1.6  contenido_sitio (imágenes editables del Home/Nosotros/Eventos)
        1.7  paquete_evento (paquetes de /Eventos administrados desde
             /AdminEventos — antes eran 3 tarjetas fijas en la vista;
             semilla verificada contra los datos reales de producción)

      PARTE 2 — Datos semilla (INSERT), en este orden:
        2.1  Catálogos base (categorías, alérgenos, tipos y OPCIONES de
             personalización, modalidades de entrega, métodos de pago,
             estados de pedido)
        2.2  3 cuentas de prueba (Dueño, Vendedor, Cliente)
        2.3  Cuenta bancaria (datos reales) + 4 publicaciones de ejemplo del blog
        2.4  Catálogo real de 24 productos (Loafs, Pies, Mermeladas, Otros)
        2.5  Paquetes de eventos (Cumpleaños, Bodas, Corporativo)

    Cómo ejecutarlo:
      1. Abre SSMS o Azure Data Studio, conéctate a tu instancia local.
      2. Abre este archivo completo y ejecútalo (F5). No hace falta correr
         ningún otro script después — este ya incluye todo.

    Nota de diseño heredada del documento original: no se define
    ON DELETE CASCADE en las FKs del núcleo transaccional (categoria,
    producto, pedido, etc.) a propósito — los catálogos se desactivan con
    la columna `disponible` en vez de borrarse físicamente. Las tablas
    agregadas después (comentario_publicacion, calificacion_pedido) sí usan
    CASCADE donde tiene sentido (si se borra la publicación o el pedido,
    sus comentarios/calificación no tienen razón de quedar huérfanos).
    ============================================================================
*/

IF DB_ID('CapysBakeryDb') IS NULL
BEGIN
    CREATE DATABASE CapysBakeryDb;
END
GO

USE CapysBakeryDb;
GO

-- ============================================================================
-- PARTE 1.1 — LAS 18 TABLAS DEL DISEÑO ORIGINAL
-- ============================================================================

-- ---------------------------- 5.1 CATÁLOGO DE PRODUCTOS ----------------------------

CREATE TABLE categoria (
    id_categoria    INT IDENTITY(1,1) PRIMARY KEY,
    nombre          NVARCHAR(80)  NOT NULL,
    disponible      BIT           NOT NULL DEFAULT (1),
    CONSTRAINT UQ_categoria_nombre UNIQUE (nombre)
);
GO

CREATE TABLE producto (
    id_producto     INT IDENTITY(1,1) PRIMARY KEY,
    nombre          NVARCHAR(120) NOT NULL,
    descripcion     NVARCHAR(MAX) NOT NULL,
    disponible      BIT           NOT NULL DEFAULT (1),

    -- Columnas que NO estaban en el diseño original de 18 tablas, pero
    -- que AdminProductosController ya usa en la práctica: precio "desde"
    -- (denormalizado, se recalcula cada vez que se guarda el producto),
    -- si es promoción, si está destacado en el Home ("Lo más pedido"), y
    -- auditoría básica de quién y cuándo lo publicó.
    precio             DECIMAL(10,2) NULL,
    es_promocion       BIT           NOT NULL DEFAULT (0),
    es_destacado       BIT           NOT NULL DEFAULT (0),
    creado_por_correo  NVARCHAR(150) NULL,
    fecha_creacion     DATETIME2     NOT NULL DEFAULT (SYSDATETIME())
);
GO

CREATE TABLE producto_categoria (
    id_producto     INT NOT NULL,
    id_categoria    INT NOT NULL,
    CONSTRAINT PK_producto_categoria PRIMARY KEY (id_producto, id_categoria),
    CONSTRAINT FK_prodcat_producto  FOREIGN KEY (id_producto)  REFERENCES producto(id_producto),
    CONSTRAINT FK_prodcat_categoria FOREIGN KEY (id_categoria) REFERENCES categoria(id_categoria)
);
GO

CREATE TABLE producto_presentacion (
    id_presentacion INT IDENTITY(1,1) PRIMARY KEY,
    id_producto     INT NOT NULL,
    nombre          NVARCHAR(80)   NOT NULL,
    porciones       INT            NULL,
    precio          DECIMAL(10,2)  NOT NULL,
    CONSTRAINT FK_presentacion_producto FOREIGN KEY (id_producto) REFERENCES producto(id_producto),
    CONSTRAINT CK_presentacion_porciones CHECK (porciones IS NULL OR porciones > 0),
    CONSTRAINT CK_presentacion_precio    CHECK (precio >= 0)
);
GO

CREATE TABLE imagen_producto (
    id_imagen       INT IDENTITY(1,1) PRIMARY KEY,
    id_producto     INT NOT NULL,
    url_imagen      NVARCHAR(500)  NOT NULL,
    orden           SMALLINT       NOT NULL,
    es_principal    BIT            NOT NULL DEFAULT (0),
    CONSTRAINT FK_imagen_producto FOREIGN KEY (id_producto) REFERENCES producto(id_producto),
    CONSTRAINT CK_imagen_orden CHECK (orden > 0)
);
GO

CREATE TABLE alergeno (
    id_alergeno     INT IDENTITY(1,1) PRIMARY KEY,
    nombre          NVARCHAR(80) NOT NULL,
    CONSTRAINT UQ_alergeno_nombre UNIQUE (nombre)
);
GO

CREATE TABLE producto_alergeno (
    id_producto     INT NOT NULL,
    id_alergeno     INT NOT NULL,
    CONSTRAINT PK_producto_alergeno PRIMARY KEY (id_producto, id_alergeno),
    CONSTRAINT FK_prodalerg_producto FOREIGN KEY (id_producto) REFERENCES producto(id_producto),
    CONSTRAINT FK_prodalerg_alergeno FOREIGN KEY (id_alergeno) REFERENCES alergeno(id_alergeno)
);
GO

-- ---------------------------- 5.2 PERSONALIZACIÓN ----------------------------

CREATE TABLE tipo_personalizacion (
    id_tipo_personalizacion INT IDENTITY(1,1) PRIMARY KEY,
    nombre                  NVARCHAR(80) NOT NULL,
    CONSTRAINT UQ_tipo_personalizacion_nombre UNIQUE (nombre)
);
GO

CREATE TABLE opcion_personalizacion (
    id_opcion               INT IDENTITY(1,1) PRIMARY KEY,
    id_tipo_personalizacion INT NOT NULL,
    nombre                  NVARCHAR(100) NOT NULL,
    CONSTRAINT FK_opcion_tipo FOREIGN KEY (id_tipo_personalizacion) REFERENCES tipo_personalizacion(id_tipo_personalizacion)
);
GO

CREATE TABLE producto_opcion_personalizacion (
    id_producto_opcion  INT IDENTITY(1,1) PRIMARY KEY,
    id_producto         INT NOT NULL,
    id_opcion           INT NOT NULL,
    precio_adicional    DECIMAL(10,2) NOT NULL DEFAULT (0),
    disponible          BIT           NOT NULL DEFAULT (1),
    CONSTRAINT FK_prodopc_producto FOREIGN KEY (id_producto) REFERENCES producto(id_producto),
    CONSTRAINT FK_prodopc_opcion   FOREIGN KEY (id_opcion)   REFERENCES opcion_personalizacion(id_opcion),
    CONSTRAINT UQ_producto_opcion  UNIQUE (id_producto, id_opcion),
    CONSTRAINT CK_prodopc_precio   CHECK (precio_adicional >= 0)
);
GO

-- ---------------------------- 5.3 ENTREGA Y PAGO ----------------------------

CREATE TABLE modalidad_entrega (
    id_modalidad_entrega INT IDENTITY(1,1) PRIMARY KEY,
    nombre               NVARCHAR(50) NOT NULL,
    requiere_direccion   BIT          NOT NULL,
    CONSTRAINT UQ_modalidad_entrega_nombre UNIQUE (nombre)
);
GO

CREATE TABLE metodo_pago (
    id_metodo_pago  INT IDENTITY(1,1) PRIMARY KEY,
    nombre          NVARCHAR(60) NOT NULL,
    solo_recoger    BIT          NOT NULL DEFAULT (0),
    disponible      BIT          NOT NULL DEFAULT (1),
    CONSTRAINT UQ_metodo_pago_nombre UNIQUE (nombre)
);
GO

CREATE TABLE cuenta_bancaria (
    id_cuenta_bancaria INT IDENTITY(1,1) PRIMARY KEY,
    id_metodo_pago     INT NOT NULL,
    banco              NVARCHAR(100) NOT NULL,
    tipo_cuenta        NVARCHAR(50)  NOT NULL,
    numero_cuenta      NVARCHAR(50)  NOT NULL,
    titular            NVARCHAR(150) NOT NULL,
    disponible         BIT           NOT NULL DEFAULT (1),
    CONSTRAINT FK_cuenta_metodo_pago FOREIGN KEY (id_metodo_pago) REFERENCES metodo_pago(id_metodo_pago)
);
GO

-- ---------------------------- 5.4 PEDIDOS ----------------------------

CREATE TABLE estado_pedido (
    id_estado_pedido INT IDENTITY(1,1) PRIMARY KEY,
    nombre           NVARCHAR(50) NOT NULL,
    CONSTRAINT UQ_estado_pedido_nombre UNIQUE (nombre)
);
GO

CREATE TABLE pedido (
    id_pedido                  BIGINT IDENTITY(1,1) PRIMARY KEY,
    codigo_pedido              NVARCHAR(30)  NOT NULL,
    nombre_cliente             NVARCHAR(150) NOT NULL,
    telefono_cliente           NVARCHAR(25)  NOT NULL,
    fecha_entrega_solicitada   DATE          NOT NULL,
    id_modalidad_entrega       INT           NOT NULL,
    direccion_o_punto_entrega  NVARCHAR(300) NULL,
    id_metodo_pago             INT           NOT NULL,
    id_estado_pedido           INT           NOT NULL,
    comentarios                NVARCHAR(MAX) NULL,
    fecha_registro             DATETIME2     NOT NULL DEFAULT (SYSDATETIME()),

    -- Agregada junto con el módulo de cuentas: permite asociar el pedido a
    -- una cuenta real sin perder nombre_cliente/telefono_cliente para
    -- pedidos históricos o de invitado.
    id_usuario                 INT NULL,

    CONSTRAINT UQ_pedido_codigo UNIQUE (codigo_pedido),
    CONSTRAINT FK_pedido_modalidad_entrega FOREIGN KEY (id_modalidad_entrega) REFERENCES modalidad_entrega(id_modalidad_entrega),
    CONSTRAINT FK_pedido_metodo_pago FOREIGN KEY (id_metodo_pago) REFERENCES metodo_pago(id_metodo_pago),
    CONSTRAINT FK_pedido_estado FOREIGN KEY (id_estado_pedido) REFERENCES estado_pedido(id_estado_pedido)
);
GO

CREATE TABLE pedido_detalle (
    id_detalle_pedido  BIGINT IDENTITY(1,1) PRIMARY KEY,
    id_pedido          BIGINT NOT NULL,
    id_presentacion    INT    NOT NULL,
    cantidad           INT    NOT NULL,
    precio_unitario    DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_detalle_pedido FOREIGN KEY (id_pedido) REFERENCES pedido(id_pedido),
    CONSTRAINT FK_detalle_presentacion FOREIGN KEY (id_presentacion) REFERENCES producto_presentacion(id_presentacion),
    CONSTRAINT CK_detalle_cantidad CHECK (cantidad > 0),
    CONSTRAINT CK_detalle_precio   CHECK (precio_unitario >= 0)
);
GO

CREATE TABLE pedido_detalle_personalizacion (
    id_detalle_personalizacion BIGINT IDENTITY(1,1) PRIMARY KEY,
    id_detalle_pedido          BIGINT NOT NULL,
    id_producto_opcion         INT    NOT NULL,
    precio_adicional_unitario  DECIMAL(10,2) NOT NULL DEFAULT (0),
    CONSTRAINT FK_detper_detalle FOREIGN KEY (id_detalle_pedido) REFERENCES pedido_detalle(id_detalle_pedido),
    CONSTRAINT FK_detper_opcion  FOREIGN KEY (id_producto_opcion) REFERENCES producto_opcion_personalizacion(id_producto_opcion),
    CONSTRAINT CK_detper_precio  CHECK (precio_adicional_unitario >= 0)
);
GO

CREATE TABLE historial_estado_pedido (
    id_historial_pedido INT IDENTITY(1,1) PRIMARY KEY,
    id_pedido           BIGINT NOT NULL,
    id_estado_pedido    INT    NOT NULL,
    fecha_cambio        DATETIME2 NOT NULL DEFAULT (SYSDATETIME()),
    CONSTRAINT FK_historial_pedido FOREIGN KEY (id_pedido) REFERENCES pedido(id_pedido),
    CONSTRAINT FK_historial_estado FOREIGN KEY (id_estado_pedido) REFERENCES estado_pedido(id_estado_pedido)
);
GO

PRINT 'CapysBakeryDb: 18 tablas del diseño original creadas correctamente.';
GO

-- ============================================================================
-- PARTE 1.2 — USUARIO (módulo de cuentas, fuera del diseño original)
-- ============================================================================

CREATE TABLE usuario (
    id_usuario          INT IDENTITY(1,1) PRIMARY KEY,
    nombre              NVARCHAR(150) NOT NULL,
    correo              NVARCHAR(150) NOT NULL,
    telefono            NVARCHAR(25)  NULL,
    password_hash       NVARCHAR(200) NOT NULL,
    rol                 NVARCHAR(20)  NOT NULL,
    creada_por_correo   NVARCHAR(150) NULL,
    fecha_registro      DATETIME2     NOT NULL DEFAULT (SYSDATETIME()),
    CONSTRAINT UQ_usuario_correo UNIQUE (correo),
    CONSTRAINT CK_usuario_rol CHECK (rol IN (N'Cliente', N'Administrador', N'Dueño'))
);
GO

-- El FK de pedido.id_usuario a usuario se agrega hasta ahora (después de
-- crear la tabla usuario) para no romper el orden de creación.
ALTER TABLE pedido
    ADD CONSTRAINT FK_pedido_usuario FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario);
GO

PRINT 'CapysBakeryDb: tabla usuario creada correctamente.';
GO

-- ============================================================================
-- PARTE 1.3 — BLOG (fuera del diseño original)
-- ============================================================================

CREATE TABLE publicacion (
    id_publicacion      INT IDENTITY(1,1) PRIMARY KEY,
    titulo              NVARCHAR(200)  NOT NULL,
    categoria           NVARCHAR(80)   NULL,
    resumen             NVARCHAR(300)  NULL,
    contenido           NVARCHAR(MAX)  NOT NULL,
    imagen_url          NVARCHAR(500)  NULL,
    publicada           BIT            NOT NULL DEFAULT (1),
    autor_correo        NVARCHAR(150)  NULL,
    fecha_publicacion   DATETIME2      NOT NULL DEFAULT (SYSDATETIME())
);
GO

CREATE TABLE comentario_publicacion (
    id_comentario       INT IDENTITY(1,1) PRIMARY KEY,
    id_publicacion      INT NOT NULL,
    id_usuario          INT NOT NULL,
    contenido           NVARCHAR(1000) NOT NULL,
    fecha_creacion      DATETIME2 NOT NULL DEFAULT (SYSDATETIME()),
    CONSTRAINT FK_comentario_publicacion FOREIGN KEY (id_publicacion) REFERENCES publicacion(id_publicacion) ON DELETE CASCADE,
    CONSTRAINT FK_comentario_usuario FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario)
);
GO

PRINT 'CapysBakeryDb: tablas del blog (publicacion, comentario_publicacion) creadas correctamente.';
GO

-- ============================================================================
-- PARTE 1.4 — MENSAJES DE CONTACTO (fuera del diseño original)
-- ============================================================================

CREATE TABLE mensaje_contacto (
    id_mensaje      INT IDENTITY(1,1) PRIMARY KEY,
    nombre          NVARCHAR(150)  NOT NULL,
    correo          NVARCHAR(150)  NOT NULL,
    telefono        NVARCHAR(25)   NULL,
    tipo_consulta   NVARCHAR(50)   NULL,
    mensaje         NVARCHAR(MAX)  NOT NULL,
    fecha_envio     DATETIME2      NOT NULL DEFAULT (SYSDATETIME()),
    leido           BIT            NOT NULL DEFAULT (0)
);
GO

PRINT 'CapysBakeryDb: tabla mensaje_contacto creada correctamente.';
GO

-- ============================================================================
-- PARTE 1.5 — CALIFICACIÓN DE PEDIDOS (fuera del diseño original)
-- ============================================================================
-- Ya incluye desde el inicio los 2 ajustes que hicimos después de crearla
-- la primera vez: 'estrellas' como INT (no TINYINT, para que EF Core la
-- lea sin tronar) y la columna 'destacado' (para elegir testimonios reales
-- que se muestran en el Home).

CREATE TABLE calificacion_pedido (
    id_calificacion    INT IDENTITY(1,1) PRIMARY KEY,
    id_pedido          BIGINT NOT NULL UNIQUE, -- debe ser BIGINT: pedido.id_pedido también lo es (un INT aquí hace fallar la FK)
    estrellas          INT NOT NULL,
    comentario         NVARCHAR(500) NULL,
    fecha_calificacion DATETIME2 NOT NULL DEFAULT (SYSDATETIME()),
    destacado          BIT NOT NULL DEFAULT (0),
    CONSTRAINT CK_calificacion_pedido_estrellas CHECK (estrellas BETWEEN 1 AND 5),
    CONSTRAINT FK_calificacion_pedido FOREIGN KEY (id_pedido) REFERENCES pedido(id_pedido) ON DELETE CASCADE
);
GO

PRINT 'CapysBakeryDb: tabla calificacion_pedido creada correctamente.';
GO

-- ============================================================================
-- PARTE 1.6 — CONTENIDO DEL SITIO (fuera del diseño original)
-- ============================================================================

CREATE TABLE contenido_sitio (
    id_contenido         INT IDENTITY(1,1) PRIMARY KEY,
    clave                NVARCHAR(60) NOT NULL UNIQUE,
    url_imagen           NVARCHAR(500) NULL,
    fecha_actualizacion  DATETIME2 NOT NULL DEFAULT (SYSDATETIME())
);
GO

PRINT 'CapysBakeryDb: tabla contenido_sitio creada correctamente.';
GO

-- ============================================================================
-- PARTE 1.7 — PAQUETES DE EVENTOS (fuera del diseño original)
-- ============================================================================
-- Las tarjetas de /Eventos (Cumpleaños, Bodas, Corporativo) antes estaban
-- fijas en la vista; ahora el Dueño/Administrador las administra desde
-- /AdminEventos (agregar, editar, ocultar, eliminar).

CREATE TABLE paquete_evento (
    id_paquete_evento   INT IDENTITY(1,1) PRIMARY KEY,
    nombre              NVARCHAR(150)  NOT NULL,
    subtitulo           NVARCHAR(150)  NULL,
    descripcion         NVARCHAR(500)  NOT NULL,
    precio              DECIMAL(10,2)  NOT NULL,
    unidad_precio       NVARCHAR(50)   NULL,
    incluye             NVARCHAR(MAX)  NOT NULL,
    etiqueta            NVARCHAR(50)   NULL,
    orden               INT            NOT NULL DEFAULT (0),
    activo              BIT            NOT NULL DEFAULT (1)
);
GO

PRINT 'CapysBakeryDb: tabla paquete_evento creada correctamente.';
GO


-- ############################################################################
-- PARTE 2 — DATOS SEMILLA
-- ############################################################################

-- ============================================================================
-- PARTE 2.1 — CATÁLOGOS BASE
-- ============================================================================

INSERT INTO categoria (nombre, disponible) VALUES
    (N'Pies', 1),
    (N'Loafs', 1),
    (N'Mermeladas', 1),
    (N'Personalizados', 0);
GO

INSERT INTO alergeno (nombre) VALUES
    (N'Gluten'),
    (N'Lácteos');
GO

INSERT INTO tipo_personalizacion (nombre) VALUES
    (N'Decoración'),
    (N'Topping');
GO

-- Opciones de personalización (color/decoración y topping) que el
-- Administrador puede habilitar por producto desde Formulario.cshtml. Sin
-- esta semilla, la sección "Personalizaciones disponibles" del formulario
-- de producto aparece vacía porque no hay ninguna opción cargada en el
-- catálogo global.
INSERT INTO opcion_personalizacion (id_tipo_personalizacion, nombre)
SELECT id_tipo_personalizacion, nombre FROM (
    SELECT N'Decoración' AS tipo, N'Ciruela' AS nombre
    UNION ALL SELECT N'Decoración', N'Dorado'
    UNION ALL SELECT N'Decoración', N'Blush'
    UNION ALL SELECT N'Decoración', N'Chocolate'
    UNION ALL SELECT N'Topping', N'Flores comestibles'
    UNION ALL SELECT N'Topping', N'Chocolate rallado'
    UNION ALL SELECT N'Topping', N'Frutos rojos'
    UNION ALL SELECT N'Topping', N'Hojas de menta'
) AS datos
JOIN tipo_personalizacion t ON t.nombre = datos.tipo;
GO

INSERT INTO modalidad_entrega (nombre, requiere_direccion) VALUES
    (N'Recoger', 0),
    (N'Envío', 1);
GO

INSERT INTO metodo_pago (nombre, solo_recoger, disponible) VALUES
    (N'Transferencia bancaria', 0, 1),
    (N'Pago al recoger', 1, 1);
GO

INSERT INTO estado_pedido (nombre) VALUES
    (N'Pendiente'),
    (N'Confirmado'),
    (N'En preparación'),
    (N'Listo'),
    (N'Entregado'),
    (N'Cancelado');
GO

PRINT 'CapysBakeryDb: catálogos iniciales cargados (incluye 8 opciones de personalización).';
GO

-- ============================================================================
-- PARTE 2.2 — CUENTAS DE PRUEBA
-- ============================================================================
-- Mismas credenciales que ya conoces (hash SHA-256 en Base64, igual que
-- MockUsuarioRepository.cs / EfUsuarioRepository.cs):
--   dueno@capysbakery.com     / Dueño123!
--   vendedor@capysbakery.com  / Vendedor123!
--   cliente@capysbakery.com   / Cliente123!

INSERT INTO usuario (nombre, correo, telefono, password_hash, rol, creada_por_correo, fecha_registro) VALUES
    (N'Capy (Dueño)',       N'dueno@capysbakery.com',     N'+502 5555 1234', N'fDQ0ZqnQ5I/+Tvw6zBuS4++5TRGBLvW+NOv6Lw74GIs=', N'Dueño',         NULL, GETDATE()),
    (N'Vendedor de prueba', N'vendedor@capysbakery.com',  N'+502 5555 5678', N'/UJQh5bsR2C8SPitPgwJ9166Dex/pjKzz2Onf+ruIr8=', N'Administrador', N'dueno@capysbakery.com', GETDATE()),
    (N'Cliente de prueba',  N'cliente@capysbakery.com',   N'+502 5555 9012', N'UZ8kqCOxBhJRbDP0OPEpZ6F5q3fW59KvaPO58mMTRac=', N'Cliente',       NULL, GETDATE());
GO

PRINT 'CapysBakeryDb: 3 cuentas de prueba cargadas (mismas credenciales de siempre).';
GO

-- ============================================================================
-- PARTE 2.3 — CUENTA BANCARIA Y BLOG DE EJEMPLO
-- ============================================================================
-- Datos reales de la cuenta a la que el Dueño pidió recibir transferencias.

INSERT INTO cuenta_bancaria (id_metodo_pago, banco, tipo_cuenta, numero_cuenta, titular, disponible)
SELECT id_metodo_pago, N'Banco Industrial', N'Cuenta de ahorro', N'1593082', N'Huberto Pernilla', 1
FROM metodo_pago
WHERE nombre = N'Transferencia bancaria';
GO

INSERT INTO publicacion (titulo, categoria, resumen, contenido, imagen_url, publicada, autor_correo, fecha_publicacion) VALUES
(N'Cómo hacemos nuestro merengue perfecto para el pie de limón', N'Recetas',
 N'Después de docenas de intentos fallidos, así es como logramos un merengue firme, brillante y sin que se baje.',
 N'Después de docenas de intentos fallidos, así es como logramos un merengue firme, brillante y sin que se baje.' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) +
 N'El truco principal está en la temperatura del jarabe de azúcar y en batir las claras justo hasta el punto de picos firmes, ni un segundo más. Usamos claras a temperatura ambiente y un tazón completamente libre de grasa — cualquier resto de yema puede arruinar el batido.' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) +
 N'Una vez armado, lo doramos con soplete en vez de horno, para controlar mejor el color sin cocinar de más el relleno de abajo.',
 NULL, 1, NULL, '2026-08-03'),

(N'3 formas de decorar con flores comestibles', N'Recetas',
 N'Guía rápida para principiantes.',
 N'Guía rápida para principiantes.' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) +
 N'Las flores comestibles son una forma fácil de darle un toque especial a cualquier pastel sin necesitar mangas ni boquillas. Aquí van tres formas sencillas: en cascada sobre un lateral, formando una corona en el centro, o esparcidas junto con hojas de menta alrededor del borde.',
 NULL, 1, NULL, '2026-07-28'),

(N'Un día en la cocina de Capys', N'Detrás de cámaras',
 N'Una jornada completa de horneado.',
 N'Una jornada completa de horneado.' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) +
 N'Empezamos antes de las 6am pesando harina y horneando los primeros loafs del día. A media mañana se arman los pies, y por la tarde se preparan los pedidos para entrega o recogida del día siguiente.',
 NULL, 1, NULL, '2026-07-20'),

(N'Cómo conservar tu pastel fresco por más días', N'Tips',
 N'Errores comunes al guardar postres.',
 N'Errores comunes al guardar postres.' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) +
 N'Guardar un pastel recién horneado todavía tibio hace que se condense humedad dentro del empaque, arruinando la textura. Deja enfriar por completo, guarda en un recipiente hermético, y si lleva relleno de crema, refrigéralo — pero sácalo con tiempo antes de servir para que recupere su textura.',
 NULL, 1, NULL, '2026-07-12');
GO

PRINT 'CapysBakeryDb: cuenta bancaria y publicaciones de ejemplo cargadas.';
GO

-- ============================================================================
-- PARTE 2.4 — CATÁLOGO REAL (24 productos)
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM categoria WHERE nombre = N'Otros')
    INSERT INTO categoria (nombre, disponible) VALUES (N'Otros', 1);
GO

DECLARE @idPies INT = (SELECT id_categoria FROM categoria WHERE nombre = N'Pies');
DECLARE @idLoafs INT = (SELECT id_categoria FROM categoria WHERE nombre = N'Loafs');
DECLARE @idMermeladas INT = (SELECT id_categoria FROM categoria WHERE nombre = N'Mermeladas');
DECLARE @idOtros INT = (SELECT id_categoria FROM categoria WHERE nombre = N'Otros');
DECLARE @id INT;

-- ============================= LOAFS =============================
INSERT INTO producto (nombre, descripcion, precio, es_promocion, disponible, fecha_creacion)
VALUES (N'Loaf de Limón', N'Refrescante y aromático, un equilibrio perfecto entre dulzura y acidez.', 60, 0, 1, GETDATE());
SET @id = SCOPE_IDENTITY();
INSERT INTO producto_categoria (id_producto, id_categoria) VALUES (@id, @idLoafs);
INSERT INTO producto_presentacion (id_producto, nombre, porciones, precio) VALUES
    (@id, N'Normal', 7, 60), (@id, N'Mediano', 15, 130), (@id, N'Grande', 25, 185);

INSERT INTO producto (nombre, descripcion, precio, es_promocion, disponible, fecha_creacion)
VALUES (N'Loaf de Banano', N'Clásico y húmedo, hecho con banano bien maduro para un sabor intenso.', 65, 0, 1, GETDATE());
SET @id = SCOPE_IDENTITY();
INSERT INTO producto_categoria (id_producto, id_categoria) VALUES (@id, @idLoafs);
INSERT INTO producto_presentacion (id_producto, nombre, porciones, precio) VALUES
    (@id, N'Normal', 7, 65), (@id, N'Mediano', 15, 135), (@id, N'Grande', 25, 200);

INSERT INTO producto (nombre, descripcion, precio, es_promocion, disponible, fecha_creacion)
VALUES (N'Loaf de Zanahoria', N'Especiado y jugoso, con un toque de canela que lo hace irresistible.', 85, 0, 1, GETDATE());
SET @id = SCOPE_IDENTITY();
INSERT INTO producto_categoria (id_producto, id_categoria) VALUES (@id, @idLoafs);
INSERT INTO producto_presentacion (id_producto, nombre, porciones, precio) VALUES
    (@id, N'Normal', 7, 85), (@id, N'Mediano', 15, 160), (@id, N'Grande', 25, 240);

INSERT INTO producto (nombre, descripcion, precio, es_promocion, disponible, fecha_creacion)
VALUES (N'Loaf de Manzana y Canela', N'Trozos de manzana fresca y un toque cálido de canela en cada bocado.', 70, 0, 1, GETDATE());
SET @id = SCOPE_IDENTITY();
INSERT INTO producto_categoria (id_producto, id_categoria) VALUES (@id, @idLoafs);
INSERT INTO producto_presentacion (id_producto, nombre, porciones, precio) VALUES
    (@id, N'Normal', 7, 70), (@id, N'Mediano', 15, 140), (@id, N'Grande', 25, 210);

INSERT INTO producto (nombre, descripcion, precio, es_promocion, disponible, fecha_creacion)
VALUES (N'Loaf de Especias', N'Una mezcla cálida de especias que lo hace ideal para cualquier época del año.', 75, 0, 1, GETDATE());
SET @id = SCOPE_IDENTITY();
INSERT INTO producto_categoria (id_producto, id_categoria) VALUES (@id, @idLoafs);
INSERT INTO producto_presentacion (id_producto, nombre, porciones, precio) VALUES
    (@id, N'Normal', 7, 75), (@id, N'Mediano', 15, 150), (@id, N'Grande', 25, 220);

INSERT INTO producto (nombre, descripcion, precio, es_promocion, disponible, fecha_creacion)
VALUES (N'Loaf de Melocotón', N'Suave y frutal, con trozos de melocotón en cada rebanada.', 70, 0, 1, GETDATE());
SET @id = SCOPE_IDENTITY();
INSERT INTO producto_categoria (id_producto, id_categoria) VALUES (@id, @idLoafs);
INSERT INTO producto_presentacion (id_producto, nombre, porciones, precio) VALUES
    (@id, N'Normal', 7, 70), (@id, N'Mediano', 15, 155), (@id, N'Grande', 25, 230);

INSERT INTO producto (nombre, descripcion, precio, es_promocion, disponible, fecha_creacion)
VALUES (N'Loaf de Elote', N'Dulce y suave, un sabor tradicional guatemalteco en formato loaf.', 80, 0, 1, GETDATE());
SET @id = SCOPE_IDENTITY();
INSERT INTO producto_categoria (id_producto, id_categoria) VALUES (@id, @idLoafs);
INSERT INTO producto_presentacion (id_producto, nombre, porciones, precio) VALUES
    (@id, N'Normal', 7, 80), (@id, N'Mediano', 15, 155), (@id, N'Grande', 25, 230);

-- ============================= PIES =============================
INSERT INTO producto (nombre, descripcion, precio, es_promocion, disponible, fecha_creacion)
VALUES (N'Lemon Pie', N'Base crujiente, relleno cítrico y merengue dorado por encima.', 100, 0, 1, GETDATE());
SET @id = SCOPE_IDENTITY();
INSERT INTO producto_categoria (id_producto, id_categoria) VALUES (@id, @idPies);
INSERT INTO producto_presentacion (id_producto, nombre, porciones, precio) VALUES (@id, N'Normal', 7, 100);

INSERT INTO producto (nombre, descripcion, precio, es_promocion, disponible, fecha_creacion)
VALUES (N'Apple Pie', N'Relleno de manzana especiada dentro de una masa hojaldrada.', 85, 0, 1, GETDATE());
SET @id = SCOPE_IDENTITY();
INSERT INTO producto_categoria (id_producto, id_categoria) VALUES (@id, @idPies);
INSERT INTO producto_presentacion (id_producto, nombre, porciones, precio) VALUES (@id, N'Normal', 7, 85);

INSERT INTO producto (nombre, descripcion, precio, es_promocion, disponible, fecha_creacion)
VALUES (N'Corn Pie', N'Receta guatemalteca clásica, dulce y cremosa.', 175, 0, 1, GETDATE());
SET @id = SCOPE_IDENTITY();
INSERT INTO producto_categoria (id_producto, id_categoria) VALUES (@id, @idPies);
INSERT INTO producto_presentacion (id_producto, nombre, porciones, precio) VALUES (@id, N'Normal', 7, 175);

INSERT INTO producto (nombre, descripcion, precio, es_promocion, disponible, fecha_creacion)
VALUES (N'Pineapple Pie', N'Relleno tropical de piña sobre una base crujiente.', 85, 0, 1, GETDATE());
SET @id = SCOPE_IDENTITY();
INSERT INTO producto_categoria (id_producto, id_categoria) VALUES (@id, @idPies);
INSERT INTO producto_presentacion (id_producto, nombre, porciones, precio) VALUES (@id, N'Normal', 7, 85);

-- ============================= MERMELADAS =============================
INSERT INTO producto (nombre, descripcion, precio, es_promocion, disponible, fecha_creacion)
VALUES (N'Mermelada de Fresa', N'Hecha con fresas frescas de temporada, sin conservantes.', 50, 0, 1, GETDATE());
SET @id = SCOPE_IDENTITY();
INSERT INTO producto_categoria (id_producto, id_categoria) VALUES (@id, @idMermeladas);
INSERT INTO producto_presentacion (id_producto, nombre, porciones, precio) VALUES (@id, N'Frasco 16 oz', NULL, 50);

INSERT INTO producto (nombre, descripcion, precio, es_promocion, disponible, fecha_creacion)
VALUES (N'Mermelada de Mango', N'Dulce y tropical, hecha con mango maduro.', 50, 0, 1, GETDATE());
SET @id = SCOPE_IDENTITY();
INSERT INTO producto_categoria (id_producto, id_categoria) VALUES (@id, @idMermeladas);
INSERT INTO producto_presentacion (id_producto, nombre, porciones, precio) VALUES (@id, N'Frasco 16 oz', NULL, 50);

INSERT INTO producto (nombre, descripcion, precio, es_promocion, disponible, fecha_creacion)
VALUES (N'Mermelada de Mora', N'Sabor intenso y ligeramente ácido, ideal para el desayuno.', 40, 0, 1, GETDATE());
SET @id = SCOPE_IDENTITY();
INSERT INTO producto_categoria (id_producto, id_categoria) VALUES (@id, @idMermeladas);
INSERT INTO producto_presentacion (id_producto, nombre, porciones, precio) VALUES (@id, N'Frasco 16 oz', NULL, 40);

INSERT INTO producto (nombre, descripcion, precio, es_promocion, disponible, fecha_creacion)
VALUES (N'Mermelada de Piña', N'Fresca y dulce, con trozos de piña natural.', 35, 0, 1, GETDATE());
SET @id = SCOPE_IDENTITY();
INSERT INTO producto_categoria (id_producto, id_categoria) VALUES (@id, @idMermeladas);
INSERT INTO producto_presentacion (id_producto, nombre, porciones, precio) VALUES (@id, N'Frasco 16 oz', NULL, 35);

INSERT INTO producto (nombre, descripcion, precio, es_promocion, disponible, fecha_creacion)
VALUES (N'Mermelada de Coco', N'Cremosa y aromática, hecha con coco natural.', 50, 0, 1, GETDATE());
SET @id = SCOPE_IDENTITY();
INSERT INTO producto_categoria (id_producto, id_categoria) VALUES (@id, @idMermeladas);
INSERT INTO producto_presentacion (id_producto, nombre, porciones, precio) VALUES (@id, N'Frasco 16 oz', NULL, 50);

INSERT INTO producto (nombre, descripcion, precio, es_promocion, disponible, fecha_creacion)
VALUES (N'Mermelada de Frutos Rojos', N'Mezcla de frutos rojos de temporada, dulce y con un toque ácido.', 80, 0, 1, GETDATE());
SET @id = SCOPE_IDENTITY();
INSERT INTO producto_categoria (id_producto, id_categoria) VALUES (@id, @idMermeladas);
INSERT INTO producto_presentacion (id_producto, nombre, porciones, precio) VALUES (@id, N'Frasco 16 oz', NULL, 80);

-- --- Mermeladas sin azúcar ---
INSERT INTO producto (nombre, descripcion, precio, es_promocion, disponible, fecha_creacion)
VALUES (N'Mermelada de Fresa sin azúcar', N'Todo el sabor de la fresa, sin azúcar añadida.', 55, 0, 1, GETDATE());
SET @id = SCOPE_IDENTITY();
INSERT INTO producto_categoria (id_producto, id_categoria) VALUES (@id, @idMermeladas);
INSERT INTO producto_presentacion (id_producto, nombre, porciones, precio) VALUES (@id, N'Frasco 16 oz', NULL, 55);

INSERT INTO producto (nombre, descripcion, precio, es_promocion, disponible, fecha_creacion)
VALUES (N'Mermelada de Piña sin azúcar', N'Dulzura natural de la piña, sin azúcar añadida.', 40, 0, 1, GETDATE());
SET @id = SCOPE_IDENTITY();
INSERT INTO producto_categoria (id_producto, id_categoria) VALUES (@id, @idMermeladas);
INSERT INTO producto_presentacion (id_producto, nombre, porciones, precio) VALUES (@id, N'Frasco 16 oz', NULL, 40);

INSERT INTO producto (nombre, descripcion, precio, es_promocion, disponible, fecha_creacion)
VALUES (N'Mermelada de Mango sin azúcar', N'Sabor tropical intenso, sin azúcar añadida.', 55, 0, 1, GETDATE());
SET @id = SCOPE_IDENTITY();
INSERT INTO producto_categoria (id_producto, id_categoria) VALUES (@id, @idMermeladas);
INSERT INTO producto_presentacion (id_producto, nombre, porciones, precio) VALUES (@id, N'Frasco 16 oz', NULL, 55);

INSERT INTO producto (nombre, descripcion, precio, es_promocion, disponible, fecha_creacion)
VALUES (N'Mermelada de Coco sin azúcar', N'Cremosa y aromática, sin azúcar añadida.', 55, 0, 1, GETDATE());
SET @id = SCOPE_IDENTITY();
INSERT INTO producto_categoria (id_producto, id_categoria) VALUES (@id, @idMermeladas);
INSERT INTO producto_presentacion (id_producto, nombre, porciones, precio) VALUES (@id, N'Frasco 16 oz', NULL, 55);

INSERT INTO producto (nombre, descripcion, precio, es_promocion, disponible, fecha_creacion)
VALUES (N'Mermelada de Frutos Rojos sin azúcar', N'Mezcla de frutos rojos, sin azúcar añadida.', 85, 0, 1, GETDATE());
SET @id = SCOPE_IDENTITY();
INSERT INTO producto_categoria (id_producto, id_categoria) VALUES (@id, @idMermeladas);
INSERT INTO producto_presentacion (id_producto, nombre, porciones, precio) VALUES (@id, N'Frasco 16 oz', NULL, 85);

INSERT INTO producto (nombre, descripcion, precio, es_promocion, disponible, fecha_creacion)
VALUES (N'Mermelada de Mora sin azúcar', N'Sabor intenso y ácido, sin azúcar añadida.', 45, 0, 1, GETDATE());
SET @id = SCOPE_IDENTITY();
INSERT INTO producto_categoria (id_producto, id_categoria) VALUES (@id, @idMermeladas);
INSERT INTO producto_presentacion (id_producto, nombre, porciones, precio) VALUES (@id, N'Frasco 16 oz', NULL, 45);

-- ============================= OTROS =============================
INSERT INTO producto (nombre, descripcion, precio, es_promocion, disponible, fecha_creacion)
VALUES (N'Brownies', N'Caja de 4 brownies de chocolate, densos y con el centro suave.', 55, 0, 1, GETDATE());
SET @id = SCOPE_IDENTITY();
INSERT INTO producto_categoria (id_producto, id_categoria) VALUES (@id, @idOtros);
INSERT INTO producto_presentacion (id_producto, nombre, porciones, precio) VALUES (@id, N'Caja de 4', 4, 55);

PRINT 'CapysBakeryDb: catálogo real cargado (24 productos: 7 Loafs, 4 Pies, 12 Mermeladas, 1 Otros).';
GO

-- ============================================================================
-- PARTE 2.5 — PAQUETES DE EVENTOS
-- ============================================================================
-- Datos idénticos a los que ya están cargados en producción (verificados
-- por consulta directa a la tabla paquete_evento).

INSERT INTO paquete_evento (nombre, subtitulo, descripcion, precio, unidad_precio, incluye, etiqueta, orden, activo) VALUES
(N'Cumpleaños', N'Para reuniones íntimas',
 N'Ideal para reuniones pequeñas y cumpleaños íntimos.',
 450, N'hasta 15 personas',
 N'Pastel de 1 piso (hasta 20 porciones)' + CHAR(13) + CHAR(10) +
 N'Mesa de dulces sencilla' + CHAR(13) + CHAR(10) +
 N'Entrega incluida en el área metropolitana',
 NULL, 1, 1),

(N'Bodas', N'Mesa dulce completa',
 N'Nuestro paquete más pedido para bodas y eventos grandes.',
 1200, N'hasta 80 personas',
 N'Pastel de 2 pisos (hasta 60 porciones)' + CHAR(13) + CHAR(10) +
 N'Mesa de dulces decorada a juego con el evento' + CHAR(13) + CHAR(10) +
 N'Servicio de meseros para postres' + CHAR(13) + CHAR(10) +
 N'Entrega y montaje incluidos',
 N'Más popular', 2, 1),

(N'Corporativo', N'Para tu empresa',
 N'Pensado para eventos corporativos y lanzamientos.',
 900, N'persona',
 N'Bocadillos dulces surtidos para hasta 50 personas' + CHAR(13) + CHAR(10) +
 N'Presentación individual por invitado' + CHAR(13) + CHAR(10) +
 N'Entrega puntual con empaque de marca',
 NULL, 3, 1);
GO

PRINT 'CapysBakeryDb: 3 paquetes de eventos cargados (Cumpleaños, Bodas, Corporativo).';
GO

PRINT '============================================================';
PRINT 'CapysBakeryDb: base de datos creada y sembrada por completo.';
PRINT '============================================================';
GO
