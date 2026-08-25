// mis-pedidos.js — Historial de pedidos del comprador (modo de prueba sin
// BD). Lee la misma lista que usa el panel de Administrador
// ("capys_pedidos_admin", ver checkout.js) pero la filtra por el correo de
// la cuenta con sesión iniciada, y no permite cambiar el estado — eso es
// exclusivo del Administrador/Dueño. Cuando exista base de datos, esto se
// reemplaza por una consulta real filtrada por id_usuario.

const CapysMisPedidos = (() => {
    const CLAVE_PEDIDOS_ADMIN = "capys_pedidos_admin";

    function obtenerMisPedidos(correo) {
        const todos = JSON.parse(localStorage.getItem(CLAVE_PEDIDOS_ADMIN) || "[]");
        if (!correo) return [];
        return todos.filter(p => (p.correoCliente || "").toLowerCase() === correo.toLowerCase());
    }

    // "Pedir de nuevo": toma los mismos productos (con la misma cantidad y
    // precio que tenían en ese pedido) y los agrega al carrito ACTUAL, sin
    // vaciar lo que ya hubiera en él. El precio se conserva tal como se
    // guardó en el pedido (no se vuelve a consultar el catálogo), igual que
    // ya se documentó para pedido_detalle en el diseño de base de datos:
    // el precio histórico de la compra no debe cambiar aunque el catálogo
    // cambie después.
    function pedirDeNuevo(pedido) {
        pedido.productos.forEach(item => CapysCarrito.agregarProducto({ ...item }));
        window.location.href = "/Carrito";
    }

    function renderizar(correo) {
        const pedidos = obtenerMisPedidos(correo);
        const contenedor = document.getElementById("cb-mis-pedidos-lista");
        const vacio = document.getElementById("cb-mis-pedidos-vacio");
        if (!contenedor) return;

        if (pedidos.length === 0) {
            contenedor.style.display = "none";
            if (vacio) vacio.style.display = "block";
            return;
        }
        if (vacio) vacio.style.display = "none";
        contenedor.style.display = "block";

        contenedor.innerHTML = pedidos.map((pedido, indice) => {
            const productos = pedido.productos.map(p => `${p.cantidad}× ${p.nombre}`).join(", ");
            return `
                <div class="cb-pedido-card">
                    <div class="cb-pedido-card-top">
                        <span class="cb-admin-pedido-id">#${pedido.identificador}</span>
                        <span class="cb-estado-pill cb-estado-${(pedido.estado || "").replace(/\s+/g, "-")}">${pedido.estado}</span>
                    </div>
                    <p class="cb-admin-sub">${productos}</p>
                    <div class="cb-summary-row"><span>${pedido.formaEntrega}</span><span>${pedido.modalidadPago}</span></div>
                    <div class="cb-summary-row total"><span>Total</span><span>Q${pedido.total.toFixed(2)}</span></div>
                    <button type="button" class="btn cb-btn-outline cb-pedir-de-nuevo" data-indice="${indice}" style="width:100%; margin-top:12px;">↻ Pedir de nuevo</button>
                </div>
            `;
        }).join("");

        contenedor.querySelectorAll(".cb-pedir-de-nuevo").forEach(boton => {
            boton.addEventListener("click", () => pedirDeNuevo(pedidos[Number(boton.dataset.indice)]));
        });
    }

    return { renderizar };
})();
