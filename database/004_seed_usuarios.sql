/*
    CAPYS DIGITAL BAKERY — Cuentas de prueba (mismas que MockUsuarioRepository)
    ------------------------------------------------------------------------
    Los hashes de abajo son SHA-256 en Base64 de las MISMAS contraseñas que
    ya usas para probar la app (dueno@capysbakery.com / Dueño123!, etc.),
    calculados con el mismo algoritmo que usa hoy MockUsuarioRepository.cs.
    Así, cuando conectemos Entity Framework, el login sigue funcionando con
    las credenciales que ya conoces — no cambia nada para ti como usuario.

    Corre esto DESPUÉS de 003_usuario.sql (o de la migración de Entity
    Framework, si ya la generaste — ver Fase 5).

    NOTA: se incluye fecha_registro explícitamente (GETDATE()) porque la
    tabla creada por la migración de EF Core no tiene un valor por defecto
    para esa columna (a diferencia del script SQL manual original, que sí
    lo tenía con DEFAULT SYSDATETIME()).
*/

USE CapysBakeryDb;
GO

INSERT INTO usuario (nombre, correo, telefono, password_hash, rol, creada_por_correo, fecha_registro) VALUES
    (N'Capy (Dueño)',       N'dueno@capysbakery.com',     N'+502 5555 1234', N'fDQ0ZqnQ5I/+Tvw6zBuS4++5TRGBLvW+NOv6Lw74GIs=', N'Dueño',         NULL, GETDATE()),
    (N'Vendedor de prueba', N'vendedor@capysbakery.com',  N'+502 5555 5678', N'/UJQh5bsR2C8SPitPgwJ9166Dex/pjKzz2Onf+ruIr8=', N'Administrador', N'dueno@capysbakery.com', GETDATE()),
    (N'Cliente de prueba',  N'cliente@capysbakery.com',   N'+502 5555 9012', N'UZ8kqCOxBhJRbDP0OPEpZ6F5q3fW59KvaPO58mMTRac=', N'Cliente',       NULL, GETDATE());
GO

PRINT 'CapysBakeryDb: 3 cuentas de prueba cargadas (mismas credenciales de siempre).';
