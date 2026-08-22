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

        contenedor.innerHTML = pedidos.map(pedido => {
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
                </div>
            `;
        }).join("");
    }

    return { renderizar };
})();
