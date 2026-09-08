/*
    CAPYS DIGITAL BAKERY — Catálogo real del cliente
    ------------------------------------------------------------------------
    Carga los productos REALES documentados en "Descripciones Productos.pdf"
    y "Análisis funcional de los productos.pdf" — no son productos de
    prueba. Precios y presentaciones exactos según esos documentos.

    NOTA sobre las descripciones: el resumen de los PDFs solo trae el texto
    comercial completo del Loaf de Limón como ejemplo; para el resto de
    productos se escribió una descripción breve equivalente. El equipo debe
    reemplazarlas por el texto exacto y aprobado por el cliente cuando esté
    disponible (usando el panel /AdminProductos → Editar, no hace falta
    tocar este script de nuevo).

    NOTA sobre alérgenos: el cliente confirmó que SÍ quiere esta función,
    pero los documentos no especifican qué alérgeno corresponde a cada
    producto real — por eso este script no asigna ninguno. Se agregan
    después desde el panel de administración cuando Capys Bakery los
    confirme (mismo criterio que ya seguía el diseño original de BD).

    Corre esto DESPUÉS de 002_seed.sql (necesita las categorías ya creadas).
*/

USE CapysBakeryDb;
GO

-- "Otros" no estaba en las categorías iniciales obligatorias (Pies, Loafs,
-- Mermeladas) pero sí existe en el negocio real (ej. Brownies) — se crea
-- aquí si todavía no existe.
IF NOT EXISTS (SELECT 1 FROM categoria WHERE nombre = N'Otros')
    INSERT INTO categoria (nombre, disponible) VALUES (N'Otros', 1);
GO

DECLARE @idPies INT = (SELECT id_categoria FROM categoria WHERE nombre = N'Pies');
DECLARE @idLoafs INT = (SELECT id_categoria FROM categoria WHERE nombre = N'Loafs');
DECLARE @idMermeladas INT = (SELECT id_categoria FROM categoria WHERE nombre = N'Mermeladas');
DECLARE @idOtros INT = (SELECT id_categoria FROM categoria WHERE nombre = N'Otros');
DECLARE @id INT;

-- ============================= LOAFS =============================
-- Presentaciones: Normal (~7 porciones), Mediano (~15), Grande (~25).

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
-- Presentación única: Normal (~7 porciones).

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
-- Presentación única: Frasco 16 oz.

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

-- --- Mermeladas sin azúcar (misma categoría, línea aparte) ---

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
