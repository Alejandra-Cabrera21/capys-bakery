/*
    CAPYS DIGITAL BAKERY — Datos adicionales (Fase 6)
    ------------------------------------------------------------------------
    Dos cosas que solo existían como datos fijos en los repositorios "Mock"
    y que ahora sí necesitan estar en la base de datos real, porque la app
    ya no las trae de memoria:

    1. cuenta_bancaria: los datos que se mostraban en el checkout al elegir
       "Transferencia bancaria" (antes vivían en MockEntregaPagoRepository).
    2. publicacion: las 4 entradas de ejemplo del blog (antes vivían en
       MockPublicacionRepository), para no perder ese contenido de
       demostración.

    Corre esto DESPUÉS de 002_seed.sql (necesita que metodo_pago ya exista).
*/

USE CapysBakeryDb;
GO

-- cuenta_bancaria — ligada a "Transferencia bancaria" (id_metodo_pago = 1,
-- según el orden en que 002_seed.sql insertó metodo_pago).
INSERT INTO cuenta_bancaria (id_metodo_pago, banco, tipo_cuenta, numero_cuenta, titular, disponible)
SELECT id_metodo_pago, N'Banco Industrial', N'Monetaria', N'0000-0000-00', N'Capys Bakery', 1
FROM metodo_pago
WHERE nombre = N'Transferencia bancaria';
GO

-- publicacion — las mismas 4 entradas de ejemplo que antes estaban
-- escritas directamente en el código (MockPublicacionRepository).
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
