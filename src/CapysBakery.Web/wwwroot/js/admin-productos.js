// admin-productos.js — Permite agregar/quitar filas de "presentación y
// precio" en el formulario de publicar/editar producto (Views/AdminProductos
// /Formulario.cshtml), reindexando los atributos name="Presentaciones[i].*"
// para que el model binding de ASP.NET Core arme correctamente la lista.

document.addEventListener("DOMContentLoaded", () => {
    const lista = document.getElementById("cb-presentaciones-lista");
    const botonAgregar = document.getElementById("cb-agregar-presentacion");
    if (!lista || !botonAgregar) return;

    function reindexar() {
        lista.querySelectorAll(".cb-presentacion-row").forEach((fila, indice) => {
            fila.querySelectorAll("input").forEach(input => {
                input.name = input.name.replace(/Presentaciones\[\d+\]/, `Presentaciones[${indice}]`);
            });
        });
    }

    function crearFila() {
        const fila = document.createElement("div");
        fila.className = "cb-presentacion-row";
        fila.innerHTML = `
            <input type="text" name="Presentaciones[0].Nombre" class="cb-form-input" placeholder="Ej. Grande (12 porciones)" required />
            <input type="number" name="Presentaciones[0].Porciones" class="cb-form-input" placeholder="Porciones" min="1" />
            <input type="number" name="Presentaciones[0].Precio" step="0.01" class="cb-form-input" placeholder="Precio Q" min="0" required />
            <button type="button" class="cb-admin-link-btn cb-quitar-presentacion" aria-label="Quitar presentación">✕</button>
        `;
        return fila;
    }

    botonAgregar.addEventListener("click", () => {
        lista.appendChild(crearFila());
        reindexar();
    });

    lista.addEventListener("click", evento => {
        if (!evento.target.classList.contains("cb-quitar-presentacion")) return;
        const filas = lista.querySelectorAll(".cb-presentacion-row");
        if (filas.length <= 1) return; // siempre debe quedar al menos una presentación
        evento.target.closest(".cb-presentacion-row").remove();
        reindexar();
    });
});
