/*
    CAPYS DIGITAL BAKERY — Tabla de usuarios y roles
    ------------------------------------------------------------------------
    El diseño original (v0.1) dejó este módulo fuera a propósito: "El
    presente documento se concentra en el núcleo transaccional... El modelo
    de cuentas, autenticación y permisos debe documentarse como módulo
    adicional". Esta es esa ampliación, ya que el proyecto de C# ya tiene
    tres roles funcionando (Cliente, Administrador/vendedor, Dueño) y
    necesitan guardarse en la base de datos real en vez de en memoria.

    Corre esto DESPUÉS de 001_schema.sql y 002_seed.sql.
*/

USE CapysBakeryDb;
GO

CREATE TABLE usuario (
    id_usuario          INT IDENTITY(1,1) PRIMARY KEY,
    nombre              NVARCHAR(150) NOT NULL,
    correo              NVARCHAR(150) NOT NULL,
    telefono            NVARCHAR(25)  NULL,

    -- Hash de la contraseña (SHA-256 en Base64, igual que hace hoy
    -- MockUsuarioRepository.cs). TODO (Sprint futuro): migrar a
    -- ASP.NET Core Identity con un hashing más robusto (PBKDF2/Argon2)
    -- cuando el proyecto lo requiera; esta columna seguiría funcionando
    -- igual, solo cambiaría cómo se genera el valor.
    password_hash       NVARCHAR(200) NOT NULL,

    -- Coincide exactamente con los valores del enum RolUsuario en C#
    -- (Cliente, Administrador, Dueño) para poder mapearlo 1:1 con
    -- Entity Framework usando HasConversion<string>().
    rol                 NVARCHAR(20)  NOT NULL,

    -- Auditoría: qué cuenta (Dueño) creó esta cuenta de staff. Null para
    -- cuentas de Cliente (autorregistro).
    creada_por_correo   NVARCHAR(150) NULL,
    fecha_registro      DATETIME2     NOT NULL DEFAULT (SYSDATETIME()),

    CONSTRAINT UQ_usuario_correo UNIQUE (correo),
    CONSTRAINT CK_usuario_rol CHECK (rol IN (N'Cliente', N'Administrador', N'Dueño'))
);
GO

PRINT 'CapysBakeryDb: tabla usuario creada correctamente.';
