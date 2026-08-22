// checkout.js — Formulario de datos del cliente + generación del link de
// WhatsApp + pantalla de confirmación.
//
// IMPORTANTE: el número de WhatsApp del negocio debe ser configurable por
// el Dueño (ver especificación de roles). Mientras no exista panel de
// configuración conectado a base de datos, queda como constante aquí.
// TODO (cuando exista Configuracion en la BD): reemplazar NUMERO_WHATSAPP
// por un valor que venga del servidor en vez de estar fijo en el JS.

const CapysCheckout = (() => {
    const NUMERO_WHATSAPP = "50255551234"; // TODO: mover a configuración real
    const CLAVE_PEDIDO_PENDIENTE = "capys_pedido_pendiente";

    function generarNumeroPedido() {
        const numero = Math.floor(1000 + Math.random() * 9000);
        return `CB-${numero}`;
    }

    function armarMensajeWhatsApp(datosCliente, carrito, numeroPedido) {
        const lineas = [];
        lineas.push(`Hola, quiero confirmar mi pedido *#${numeroPedido}* en Capys Bakery.`);
        lineas.push("");
        lineas.push("*Productos:*");
        carrito.forEach(item => {
            let linea = `- ${item.nombre} x${item.cantidad} (Q${(item.precio * item.cantidad).toFixed(2)})`;
            if (item.opciones?.tamano) linea += ` — ${item.opciones.tamano}`;
            if (item.opciones?.color) linea += `, decoración ${item.opciones.color}`;
            lineas.push(linea);
        });
        lineas.push("");
        lineas.push(`*Nombre:* ${datosCliente.nombre}`);
        lineas.push(`*Teléfono:* ${datosCliente.telefono}`);
        lineas.push(`*Entrega:* ${datosCliente.tipoEntrega}`);
        if (datosCliente.direccion) lineas.push(`*Dirección:* ${datosCliente.direccion}`);
        if (datosCliente.fecha) lineas.push(`*Fecha deseada:* ${datosCliente.fecha}`);
        if (datosCliente.hora) lineas.push(`*Hora aproximada:* ${datosCliente.hora}`);
        if (datosCliente.notas) lineas.push(`*Notas:* ${datosCliente.notas}`);
        lineas.push(`*Método de pago:* ${datosCliente.metodoPago}`);
        lineas.push("");
        const subtotal = CapysCarrito.calcularSubtotal(carrito);
        const esDomicilio = datosCliente.tipoEntrega === "Entrega a domicilio";
        const envio = esDomicilio ? CapysCarrito.COSTO_ENVIO : 0;
        const total = subtotal + envio;
        lineas.push(`*Total estimado:* Q${total.toFixed(2)}${esDomicilio ? ` (incluye Q${envio.toFixed(2)} de envío)` : " (recoger en tienda, sin costo de envío)"}`);

        return lineas.join("\n");
    }

    function actualizarOpcionesDePago(tipoEntrega) {
        const esRecoger = tipoEntrega === "Recoger en tienda";
        const opciones = document.querySelectorAll('[data-config="pago"] .cb-toggle');
        opciones.forEach(boton => {
            const paraQuien = boton.dataset.para.split(",");
            const disponible = esRecoger ? paraQuien.includes("recoger") : paraQuien.includes("domicilio");
            boton.hidden = !disponible;
            if (!disponible && boton.classList.contains("active")) {
                boton.classList.remove("active");
            }
        });
        // si ninguna quedó activa (porque la que estaba seleccionada se ocultó),
        // selecciona automáticamente la primera opción visible.
        const algunaActiva = Array.from(opciones).some(b => b.classList.contains("active"));
        if (!algunaActiva) {
            const primeraVisible = Array.from(opciones).find(b => !b.hidden);
            primeraVisible?.classList.add("active");
        }
    }

    function iniciarFormularioDatosCliente() {
        const carrito = CapysCarrito.obtenerCarrito();
        const contenedor = document.getElementById("cb-checkout-items");
        const totalEl = document.getElementById("cb-checkout-total");

        if (contenedor) {
            contenedor.innerHTML = carrito
                .map(item => `<div class="cb-summary-row"><span>${item.nombre} × ${item.cantidad}</span><span>${CapysCarrito.formatearMoneda(item.precio * item.cantidad)}</span></div>`)
                .join("");
        }
        if (totalEl) {
            const total = CapysCarrito.calcularSubtotal(carrito) + CapysCarrito.COSTO_ENVIO;
            totalEl.textContent = CapysCarrito.formatearMoneda(total);
        }

        // Toggle de tipo de entrega (domicilio / recoger en tienda)
        document.querySelectorAll('[data-config="entrega"] .cb-toggle').forEach(boton => {
            boton.addEventListener("click", () => {
                document.querySelectorAll('[data-config="entrega"] .cb-toggle').forEach(b => b.classList.remove("active"));
                boton.classList.add("active");
                actualizarOpcionesDePago(boton.dataset.valor);
            });
        });

        // Selección del método de pago
        document.querySelectorAll('[data-config="pago"] .cb-toggle').forEach(boton => {
            boton.addEventListener("click", () => {
                if (boton.hidden) return;
                document.querySelectorAll('[data-config="pago"] .cb-toggle').forEach(b => b.classList.remove("active"));
                boton.classList.add("active");
            });
        });

        // Estado inicial de las opciones de pago según la entrega por defecto
        const entregaActual = document.querySelector('[data-config="entrega"] .active')?.dataset.valor ?? "Entrega a domicilio";
        actualizarOpcionesDePago(entregaActual);

        // Envío del formulario → genera el pedido y redirige a WhatsApp
        const form = document.getElementById("form-datos-cliente");
        form?.addEventListener("submit", e => {
            e.preventDefault();

            if (carrito.length === 0) {
                alert("Tu carrito está vacío. Agrega productos antes de continuar.");
                window.location.href = "/Catalogo";
                return;
            }

            const tipoEntrega = document.querySelector('[data-config="entrega"] .active')?.dataset.valor ?? "Entrega a domicilio";
            const metodoPago = document.querySelector('[data-config="pago"] .active')?.dataset.valor ?? "Transferencia bancaria";

            const datosCliente = {
                nombre: document.getElementById("nombre").value,
                telefono: document.getElementById("telefono").value,
                tipoEntrega,
                direccion: document.getElementById("direccion").value,
                fecha: document.getElementById("fecha").value,
                hora: document.getElementById("hora").value,
                notas: document.getElementById("notas").value,
                metodoPago,
            };

            const numeroPedido = generarNumeroPedido();
            const mensaje = armarMensajeWhatsApp(datosCliente, carrito, numeroPedido);

            // Guarda una copia del pedido para mostrarla en la Confirmación,
            // y por ahora como único "registro" del pedido mientras no hay BD.
            localStorage.setItem(CLAVE_PEDIDO_PENDIENTE, JSON.stringify({
                numeroPedido, datosCliente, carrito, fecha: new Date().toISOString(),
            }));

            const urlWhatsApp = `https://wa.me/${NUMERO_WHATSAPP}?text=${encodeURIComponent(mensaje)}`;

            CapysCarrito.vaciarCarrito();
            window.open(urlWhatsApp, "_blank");
            window.location.href = "/Checkout/Confirmacion";
        });
    }

    function renderizarConfirmacion() {
        const datos = localStorage.getItem(CLAVE_PEDIDO_PENDIENTE);
        if (!datos) return;

        const pedido = JSON.parse(datos);

        const numeroEl = document.getElementById("cb-numero-pedido");
        if (numeroEl) numeroEl.textContent = `Pedido #${pedido.numeroPedido}`;

        const itemsEl = document.getElementById("cb-cc-items");
        if (itemsEl) {
            const subtotal = CapysCarrito.calcularSubtotal(pedido.carrito);
            const esDomicilio = pedido.datosCliente.tipoEntrega === "Entrega a domicilio";
            const envio = esDomicilio ? CapysCarrito.COSTO_ENVIO : 0;
            const total = subtotal + envio;

            let filas = pedido.carrito.map(item => `<div class="cb-cc-row"><span>${item.nombre} × ${item.cantidad}</span><span>${CapysCarrito.formatearMoneda(item.precio * item.cantidad)}</span></div>`).join("");
            filas += `<div class="cb-cc-row"><span>${esDomicilio ? "Entrega a domicilio" : "Recoger en tienda"}</span><span>${esDomicilio ? CapysCarrito.formatearMoneda(envio) : "Sin costo"}</span></div>`;
            filas += `<div class="cb-cc-row"><span>Método de pago</span><span>${pedido.datosCliente.metodoPago}</span></div>`;
            filas += `<div class="cb-cc-row total"><span>Total estimado</span><span>${CapysCarrito.formatearMoneda(total)}</span></div>`;

            itemsEl.innerHTML = filas;
        }

        document.getElementById("btn-reabrir-whatsapp")?.addEventListener("click", () => {
            const mensaje = armarMensajeWhatsApp(pedido.datosCliente, pedido.carrito, pedido.numeroPedido);
            window.open(`https://wa.me/${NUMERO_WHATSAPP}?text=${encodeURIComponent(mensaje)}`, "_blank");
        });
    }

    return { iniciarFormularioDatosCliente, renderizarConfirmacion };
})();
