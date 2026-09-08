// mis-pedidos.js — Fase 6: la página ya se renderiza completa en el
// servidor (Views/Cuenta/MisPedidos.cshtml) con datos reales de la base de
// datos. Lo único que hace este archivo es la parte que sí debe vivir en
// el navegador: "Pedir de nuevo" agrega esos mismos productos al carrito
// actual (que sigue viviendo en localStorage, ver carrito.js).

document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll(".cb-pedir-de-nuevo").forEach(boton => {
        boton.addEventListener("click", () => {
            const items = JSON.parse(boton.dataset.items || "[]");
            items.forEach(item => CapysCarrito.agregarProducto({
                id: String(item.id),
                presentacionId: item.presentacionId,
                nombre: item.nombre,
                precio: item.precio,
                cantidad: item.cantidad,
                opciones: { tamano: null, color: null, toppings: [] },
            }));
            window.location.href = "/Carrito";
        });
    });
});
