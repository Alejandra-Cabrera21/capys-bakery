/*
    CAPYS DIGITAL BAKERY — Script de creación de base de datos (SQL Server)
    ------------------------------------------------------------------------
    Basado 1:1 en "Documentación técnica de base de datos v0.1" y en el
    diagrama E/R ya revisado con el equipo (18 tablas).

    Cómo ejecutarlo:
      1. Abre SQL Server Management Studio (SSMS) o Azure Data Studio.
      2. Conéctate a tu instancia local (ej. localhost\SQLEXPRESS).
      3. Abre este archivo y ejecútalo completo (F5). Crea la base de datos
         CapysBakeryDb y las 18 tablas, con sus llaves primarias, foráneas
         y restricciones (CHECK, UNIQUE) documentadas.
      4. Después corre 002_seed.sql para cargar los catálogos iniciales
         (categorías, estados de pedido, modalidades de entrega, etc.).

    Nota de diseño: no se define ON DELETE CASCADE en ninguna FK a propósito
    (sección 6 del documento: "No se fuerza una estrategia específica de
    eliminación"). Los catálogos se desactivan con la columna `disponible`
    en vez de borrarse físicamente.
*/

IF DB_ID('CapysBakeryDb') IS NULL
BEGIN
    CREATE DATABASE CapysBakeryDb;
END
GO

USE CapysBakeryDb;
GO

-- =========================================================================
-- 5.1 CATÁLOGO DE PRODUCTOS
-- =========================================================================

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
    disponible      BIT           NOT NULL DEFAULT (1)
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

-- =========================================================================
-- 5.2 PERSONALIZACIÓN
-- =========================================================================

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

-- =========================================================================
-- 5.3 ENTREGA Y PAGO
-- =========================================================================

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

-- =========================================================================
-- 5.4 PEDIDOS
-- =========================================================================

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

PRINT 'CapysBakeryDb: 18 tablas creadas correctamente.';
