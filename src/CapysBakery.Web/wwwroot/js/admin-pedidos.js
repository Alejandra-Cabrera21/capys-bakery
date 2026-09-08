// admin-pedidos.js — Panel de gestión de pedidos (modo de prueba sin BD).
// Lee y actualiza la lista de pedidos guardados en localStorage bajo la
// llave "capys_pedidos_admin" (ver checkout.js). Cuando exista la base de
// datos, esto se reemplaza por llamadas reales al servidor.

const CapysAdminPedidos = (() => {
    const CLAVE_PEDIDOS_ADMIN = "capys_pedidos_admin";
    const ESTADOS = ["Pendiente", "Confirmado", "En preparación", "Listo", "Entregado", "Cancelado"];

    function obtenerPedidos() {
        return JSON.parse(localStorage.getItem(CLAVE_PEDIDOS_ADMIN) || "[]");
    }

    function guardarPedidos(pedidos) {
        localStorage.setItem(CLAVE_PEDIDOS_ADMIN, JSON.stringify(pedidos));
    }

    function renderizar() {
        const pedidos = obtenerPedidos();
        const tbody = document.getElementById("cb-pedidos-tbody");
        const vacio = document.getElementById("cb-sin-pedidos");
        const tabla = document.getElementById("cb-tabla-pedidos");
        if (!tbody) return;

        if (pedidos.length === 0) {
            if (vacio) vacio.style.display = "block";
            if (tabla) tabla.style.display = "none";
            return;
        }
        if (vacio) vacio.style.display = "none";
        if (tabla) tabla.style.display = "table";

        tbody.innerHTML = pedidos.map((pedido, indice) => {
            const productos = pedido.productos.map(p => `${p.cantidad}× ${p.nombre}`).join(", ");
            const opciones = ESTADOS.map(e => `<option value="${e}" ${pedido.estado === e ? "selected" : ""}>${e}</option>`).join("");
            return `
                <tr>
                    <td><span class="cb-admin-pedido-id">#${pedido.identificador}</span></td>
                    <td>${pedido.nombreCliente}<br><span class="cb-admin-sub">${pedido.telefono}</span></td>
                    <td>${productos}</td>
                    <td>${pedido.formaEntrega}${pedido.direccion ? `<br><span class="cb-admin-sub">${pedido.direccion}</span>` : ""}</td>
                    <td>${pedido.modalidadPago}</td>
                    <td>Q${pedido.total.toFixed(2)}</td>
                    <td>
                        <select class="cb-admin-estado-select" data-indice="${indice}">
                            ${opciones}
                        </select>
                    </td>
                </tr>
            `;
        }).join("");

        tbody.querySelectorAll(".cb-admin-estado-select").forEach(select => {
            select.addEventListener("change", () => {
                const pedidos = obtenerPedidos();
                const indice = parseInt(select.dataset.indice, 10);
                pedidos[indice].estado = select.value;
                guardarPedidos(pedidos);
            });
        });
    }

    return { renderizar };
})();
