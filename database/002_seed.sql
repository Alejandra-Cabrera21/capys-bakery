/*
    CAPYS DIGITAL BAKERY — Datos semilla (catálogos iniciales)
    ------------------------------------------------------------------------
    Corre esto DESPUÉS de 001_schema.sql. Carga únicamente los catálogos
    que ya fueron confirmados por el cliente en la documentación funcional
    (categorías, alérgenos conocidos, modalidades de entrega, métodos de
    pago y los 6 estados de pedido). No inventa productos reales ni
    opciones de personalización específicas — esas las carga el equipo
    desde el panel de administración (/AdminProductos) una vez que Capys
    Bakery las confirme.
*/

USE CapysBakeryDb;
GO

-- 5.1.1 categoria — categorías iniciales confirmadas por el cliente
INSERT INTO categoria (nombre, disponible) VALUES
    (N'Pies', 1),
    (N'Loafs', 1),
    (N'Mermeladas', 1),
    (N'Personalizados', 0);
GO

-- 5.1.6 alergeno — ejemplos mencionados por el cliente
INSERT INTO alergeno (nombre) VALUES
    (N'Gluten'),
    (N'Lácteos');
GO

-- 5.2.1 tipo_personalizacion — grupos base; las opciones concretas
-- (5.2.2) se agregan cuando Capys Bakery las apruebe.
INSERT INTO tipo_personalizacion (nombre) VALUES
    (N'Decoración'),
    (N'Topping');
GO

-- 5.3.1 modalidad_entrega
INSERT INTO modalidad_entrega (nombre, requiere_direccion) VALUES
    (N'Recoger', 0),
    (N'Envío', 1);
GO

-- 5.3.2 metodo_pago
INSERT INTO metodo_pago (nombre, solo_recoger, disponible) VALUES
    (N'Transferencia bancaria', 0, 1),
    (N'Pago al recoger', 1, 1);
GO

-- 5.4.1 estado_pedido — los 6 estados confirmados por el cliente
INSERT INTO estado_pedido (nombre) VALUES
    (N'Pendiente'),
    (N'Confirmado'),
    (N'En preparación'),
    (N'Listo'),
    (N'Entregado'),
    (N'Cancelado');
GO

PRINT 'CapysBakeryDb: catálogos iniciales cargados.';
