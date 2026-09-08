// carrito.js — Lógica del carrito de compras.
// El carrito vive por completo en localStorage del navegador (NO en el
// servidor), para que un visitante sin cuenta pueda agregar productos
// libremente, tal como se definió en la especificación de roles.
//
// Estructura guardada en localStorage bajo la llave "capys_carrito":
// [
//   { id, nombre, precio, cantidad, opciones: { tamano, color, toppings } }
// ]

const CapysCarrito = (() => {
    const CLAVE_STORAGE = "capys_carrito";
    const COSTO_ENVIO = 35.0;

    function obtenerCarrito() {
        const datos = localStorage.getItem(CLAVE_STORAGE);
        return datos ? JSON.parse(datos) : [];
    }

    function guardarCarrito(carrito) {
        localStorage.setItem(CLAVE_STORAGE, JSON.stringify(carrito));
        actualizarBadgeCarrito();
    }

    function agregarProducto(item) {
        const carrito = obtenerCarrito();
        carrito.push(item);
        guardarCarrito(carrito);
    }

    function actualizarCantidad(indice, nuevaCantidad) {
        const carrito = obtenerCarrito();
        if (nuevaCantidad <= 0) {
            carrito.splice(indice, 1);
        } else {
            carrito[indice].cantidad = nuevaCantidad;
        }
        guardarCarrito(carrito);
    }

    function eliminarProducto(indice) {
        const carrito = obtenerCarrito();
        carrito.splice(indice, 1);
        guardarCarrito(carrito);
    }

    function vaciarCarrito() {
        localStorage.removeItem(CLAVE_STORAGE);
        actualizarBadgeCarrito();
    }

    function calcularSubtotal(carrito) {
        return carrito.reduce((total, item) => total + item.precio * item.cantidad, 0);
    }

    function formatearMoneda(valor) {
        return "Q" + valor.toFixed(2);
    }

    // Actualiza el numerito del carrito en el header, en TODAS las páginas.
    function actualizarBadgeCarrito() {
        const carrito = obtenerCarrito();
        const totalItems = carrito.reduce((total, item) => total + item.cantidad, 0);
        document.querySelectorAll("[data-cart-badge]").forEach(el => {
            el.textContent = totalItems;
        });
    }

    // Pinta la lista completa de productos en la página /Carrito
    function renderizarPaginaCarrito() {
        const carrito = obtenerCarrito();
        const contenedor = document.getElementById("cb-cart-items");
        const vacio = document.getElementById("cb-cart-empty");
        const template = document.getElementById("cb-cart-item-template");
        const textoConteo = document.getElementById("cb-cart-count-text");

        if (!contenedor) return;

        contenedor.innerHTML = "";

        if (carrito.length === 0) {
            if (vacio) vacio.style.display = "block";
            if (textoConteo) textoConteo.textContent = "Tu carrito está vacío.";
            actualizarResumen([]);
            return;
        }

        if (vacio) vacio.style.display = "none";
        if (textoConteo) textoConteo.textContent = `${carrito.length} producto(s) seleccionados para tu pedido`;

        carrito.forEach((item, indice) => {
            const nodo = template.content.cloneNode(true);
            nodo.querySelector(".cb-cart-item-name").textContent = item.nombre;

            const opciones = [];
            if (item.opciones?.tamano) opciones.push(item.opciones.tamano);
            if (item.opciones?.color) opciones.push(`Decoración ${item.opciones.color}`);
            if (item.opciones?.toppings?.length) opciones.push(item.opciones.toppings.join(", "));
            nodo.querySelector(".cb-cart-item-opt").textContent = opciones.join(" · ");

            nodo.querySelector(".cb-qty-value").textContent = item.cantidad;
            nodo.querySelector(".cb-cart-item-price").textContent = formatearMoneda(item.precio * item.cantidad);

            nodo.querySelector(".cb-qty-minus").addEventListener("click", () => {
                actualizarCantidad(indice, item.cantidad - 1);
                renderizarPaginaCarrito();
            });
            nodo.querySelector(".cb-qty-plus").addEventListener("click", () => {
                actualizarCantidad(indice, item.cantidad + 1);
                renderizarPaginaCarrito();
            });
            nodo.querySelector(".cb-cart-item-remove").addEventListener("click", () => {
                eliminarProducto(indice);
                renderizarPaginaCarrito();
            });

            contenedor.appendChild(nodo);
        });

        actualizarResumen(carrito);
    }

    function actualizarResumen(carrito) {
        const subtotal = calcularSubtotal(carrito);
        const envio = carrito.length > 0 ? COSTO_ENVIO : 0;
        const total = subtotal + envio;

        const elSubtotal = document.getElementById("cb-subtotal");
        const elTotal = document.getElementById("cb-total");
        if (elSubtotal) elSubtotal.textContent = formatearMoneda(subtotal);
        if (elTotal) elTotal.textContent = formatearMoneda(total);
    }

    return {
        obtenerCarrito,
        agregarProducto,
        actualizarCantidad,
        eliminarProducto,
        vaciarCarrito,
        calcularSubtotal,
        formatearMoneda,
        actualizarBadgeCarrito,
        renderizarPaginaCarrito,
        COSTO_ENVIO,
    };
})();

// El badge del carrito se actualiza en CUALQUIER página que tenga el layout,
// no solo en /Carrito, para que el número se vea siempre en el header.
document.addEventListener("DOMContentLoaded", () => CapysCarrito.actualizarBadgeCarrito());
