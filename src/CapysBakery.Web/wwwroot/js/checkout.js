// checkout.js — Formulario de datos del cliente + generación del link de
// WhatsApp + pantalla de confirmación.
//
// Basado en el "Análisis funcional de la confirmación mediante WhatsApp"
// y el "Análisis funcional de las modalidades de pago" proporcionados por
// el cliente.

const CapysCheckout = (() => {
    // Número autorizado por el cliente (personal, mientras no exista uno
    // dedicado al negocio). TODO: mover a configuración del Dueño cuando
    // exista panel conectado a base de datos.
    const NUMERO_WHATSAPP = "50248036717";
    const CLAVE_PEDIDO_PENDIENTE = "capys_pedido_pendiente";
    const CLAVE_PEDIDOS_ADMIN = "capys_pedidos_admin"; // lista completa, para el panel de Administrador

    // Datos bancarios que el cliente indicó que deben mostrarse al elegir
    // Transferencia bancaria. TODO: mover a configuración real del Dueño.
    const DATOS_BANCARIOS = {
        banco: "Banco Industrial",
        tipoCuenta: "Monetaria",
        numeroCuenta: "0000-0000-00",
        titular: "Capys Bakery",
    };

    function generarNumeroPedido() {
        const numero = Math.floor(1000 + Math.random() * 9000);
        return `CB-${numero}`;
    }

    function armarMensajeWhatsApp(datosCliente, carrito, numeroPedido, total) {
        const lineas = [];
        lineas.push("Hola, realicé un pedido desde la página de Capys Bakery.");
        lineas.push("");
        lineas.push(`Pedido: #${numeroPedido}`);
        lineas.push(`Cliente: ${datosCliente.nombre}`);
        lineas.push("");
        lineas.push("Productos:");
        carrito.forEach(item => {
            let linea = `- ${item.cantidad} ${item.nombre}`;
            if (item.opciones?.tamano) linea += ` — ${item.opciones.tamano}`;
            linea += ` — Q${(item.precio * item.cantidad).toFixed(2)}`;
            lineas.push(linea);
        });
        lineas.push("");
        lineas.push(`Total: Q${total.toFixed(2)}`);
        lineas.push(`Fecha de entrega: ${datosCliente.fecha || "por confirmar"}`);
        lineas.push(`Forma de entrega: ${datosCliente.formaEntrega}`);
        lineas.push(`Modalidad de pago: ${datosCliente.metodoPago}`);
        if (datosCliente.direccion) lineas.push(`Dirección: ${datosCliente.direccion}`);
        if (datosCliente.comentarios) lineas.push(`Comentarios: ${datosCliente.comentarios}`);
        lineas.push("");
        lineas.push("Quisiera continuar con la confirmación de mi pedido.");

        return lineas.join("\n");
    }

    function actualizarOpcionesDePago(formaEntrega) {
        // Regla del cliente: "Pago al recoger" SOLO existe si la forma de
        // entrega es Recoger. Para Envío, únicamente Transferencia bancaria.
        const esRecoger = formaEntrega === "Recoger";
        const opciones = document.querySelectorAll('[data-config="pago"] .cb-toggle');
        opciones.forEach(boton => {
            const paraQuien = boton.dataset.para.split(",");
            const disponible = esRecoger ? paraQuien.includes("recoger") : paraQuien.includes("envio");
            boton.hidden = !disponible;
            if (!disponible && boton.classList.contains("active")) {
                boton.classList.remove("active");
            }
        });
        const algunaActiva = Array.from(opciones).some(b => b.classList.contains("active"));
        if (!algunaActiva) {
            const primeraVisible = Array.from(opciones).find(b => !b.hidden);
            primeraVisible?.classList.add("active");
        }
        actualizarBloqueTransferencia();
    }

    // Muestra/oculta los datos bancarios según el método de pago elegido.
    function actualizarBloqueTransferencia() {
        const metodoActivo = document.querySelector('[data-config="pago"] .active')?.dataset.valor;
        const bloque = document.getElementById("cb-datos-bancarios");
        if (bloque) bloque.style.display = metodoActivo === "Transferencia bancaria" ? "block" : "none";
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

        // Datos bancarios en el HTML (rellenados aquí para que el equipo
        // los pueda cambiar fácilmente desde una sola constante)
        const elBanco = document.getElementById("cb-dato-banco");
        if (elBanco) {
            elBanco.innerHTML = `
                <div class="cb-summary-row"><span>Banco</span><span>${DATOS_BANCARIOS.banco}</span></div>
                <div class="cb-summary-row"><span>Tipo de cuenta</span><span>${DATOS_BANCARIOS.tipoCuenta}</span></div>
                <div class="cb-summary-row"><span>Número de cuenta</span><span>${DATOS_BANCARIOS.numeroCuenta}</span></div>
                <div class="cb-summary-row"><span>Titular</span><span>${DATOS_BANCARIOS.titular}</span></div>
            `;
        }

        // Toggle de forma de entrega (Envío / Recoger)
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
                actualizarBloqueTransferencia();
            });
        });

        const entregaActual = document.querySelector('[data-config="entrega"] .active')?.dataset.valor ?? "Envío";
        actualizarOpcionesDePago(entregaActual);

        const form = document.getElementById("form-datos-cliente");
        form?.addEventListener("submit", e => {
            e.preventDefault();

            if (carrito.length === 0) {
                alert("Tu carrito está vacío. Agrega productos antes de continuar.");
                window.location.href = "/Catalogo";
                return;
            }

            const formaEntrega = document.querySelector('[data-config="entrega"] .active')?.dataset.valor ?? "Envío";
            const metodoPago = document.querySelector('[data-config="pago"] .active')?.dataset.valor ?? "Transferencia bancaria";
            const subtotal = CapysCarrito.calcularSubtotal(carrito);
            const esEnvio = formaEntrega === "Envío";
            const envio = esEnvio ? CapysCarrito.COSTO_ENVIO : 0;
            const total = subtotal + envio;

            const datosCliente = {
                nombre: document.getElementById("nombre").value,
                telefono: document.getElementById("telefono").value,
                formaEntrega,
                direccion: document.getElementById("direccion").value,
                fecha: document.getElementById("fecha").value,
                comentarios: document.getElementById("notas").value,
                metodoPago,
            };

            const numeroPedido = generarNumeroPedido();
            const mensaje = armarMensajeWhatsApp(datosCliente, carrito, numeroPedido, total);

            const pedido = {
                identificador: numeroPedido,
                nombreCliente: datosCliente.nombre,
                telefono: datosCliente.telefono,
                fechaEntrega: datosCliente.fecha,
                formaEntrega: datosCliente.formaEntrega,
                direccion: datosCliente.direccion,
                modalidadPago: datosCliente.metodoPago,
                comentarios: datosCliente.comentarios,
                productos: carrito,
                total,
                estado: "Pendiente", // según el análisis funcional: siempre inicia Pendiente
                fechaCreacion: new Date().toISOString(),
            };

            // Copia para mostrar en la pantalla de Confirmación
            localStorage.setItem(CLAVE_PEDIDO_PENDIENTE, JSON.stringify(pedido));

            // Lista acumulada para que el panel de Administrador pueda
            // listarlos. TODO: reemplazar por guardado real en base de
            // datos — esto solo funciona dentro del mismo navegador.
            const pedidosGuardados = JSON.parse(localStorage.getItem(CLAVE_PEDIDOS_ADMIN) || "[]");
            pedidosGuardados.unshift(pedido);
            localStorage.setItem(CLAVE_PEDIDOS_ADMIN, JSON.stringify(pedidosGuardados));

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
        if (numeroEl) numeroEl.textContent = `Pedido #${pedido.identificador}`;

        const itemsEl = document.getElementById("cb-cc-items");
        if (itemsEl) {
            const subtotal = CapysCarrito.calcularSubtotal(pedido.productos);
            const esEnvio = pedido.formaEntrega === "Envío";
            const envio = esEnvio ? CapysCarrito.COSTO_ENVIO : 0;

            let filas = pedido.productos.map(item => `<div class="cb-cc-row"><span>${item.nombre} × ${item.cantidad}</span><span>${CapysCarrito.formatearMoneda(item.precio * item.cantidad)}</span></div>`).join("");
            filas += `<div class="cb-cc-row"><span>${pedido.formaEntrega}</span><span>${esEnvio ? CapysCarrito.formatearMoneda(envio) : "Sin costo"}</span></div>`;
            filas += `<div class="cb-cc-row"><span>Método de pago</span><span>${pedido.modalidadPago}</span></div>`;
            filas += `<div class="cb-cc-row total"><span>Total estimado</span><span>${CapysCarrito.formatearMoneda(pedido.total)}</span></div>`;

            itemsEl.innerHTML = filas;
        }

        document.getElementById("btn-reabrir-whatsapp")?.addEventListener("click", () => {
            const datosCliente = {
                nombre: pedido.nombreCliente, formaEntrega: pedido.formaEntrega,
                direccion: pedido.direccion, fecha: pedido.fechaEntrega,
                comentarios: pedido.comentarios, metodoPago: pedido.modalidadPago,
            };
            const mensaje = armarMensajeWhatsApp(datosCliente, pedido.productos, pedido.identificador, pedido.total);
            window.open(`https://wa.me/${NUMERO_WHATSAPP}?text=${encodeURIComponent(mensaje)}`, "_blank");
        });
    }

    return { iniciarFormularioDatosCliente, renderizarConfirmacion };
})();
