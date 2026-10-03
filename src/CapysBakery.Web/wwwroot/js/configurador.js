// configurador.js — Lógica de la página de detalle de producto:
// selección de tamaño/presentación (cada una con su propio precio),
// personalizaciones (color, topping, etc. — ahora con precio adicional
// real, cargado desde producto_opcion_personalizacion), cantidad, y
// agregar al carrito.

document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("form-agregar-carrito");
    if (!form) return; // esta página no es la de detalle de producto

    const precioMostrado = document.getElementById("cb-pd-precio-mostrado");
    const botonAgregar = document.getElementById("btn-agregar-carrito");
    const botonFavoritos = document.getElementById("btn-agregar-favoritos");

    // --- favoritos ---
    // Igual que el carrito, se guarda en localStorage mientras no exista una
    // cuenta/BD real que persista favoritos entre dispositivos (ver TODOs de
    // checkout.js). Es solo una lista de ids de producto.
    const CLAVE_FAVORITOS = "capys_favoritos";

    function obtenerFavoritos() {
        return JSON.parse(localStorage.getItem(CLAVE_FAVORITOS) || "[]");
    }

    function actualizarBotonFavoritos() {
        if (!botonFavoritos) return;
        const esFavorito = obtenerFavoritos().includes(botonFavoritos.dataset.productoId);
        botonFavoritos.textContent = esFavorito ? "♥ En favoritos" : "♡ Agregar a favoritos";
        botonFavoritos.classList.toggle("cb-btn-favorito-activo", esFavorito);
    }

    botonFavoritos?.addEventListener("click", () => {
        const id = botonFavoritos.dataset.productoId;
        let favoritos = obtenerFavoritos();
        favoritos = favoritos.includes(id) ? favoritos.filter(f => f !== id) : [...favoritos, id];
        localStorage.setItem(CLAVE_FAVORITOS, JSON.stringify(favoritos));
        actualizarBotonFavoritos();
    });

    actualizarBotonFavoritos();

    // --- personalizaciones (color, topping, etc.) ---
    // Cada grupo viene marcado con data-config="personalizacion-N" (ver
    // Catalogo/Detalle.cshtml) y data-multiple="true"/"false" según si el
    // cliente puede elegir una sola opción (ej. Decoración) o varias
    // (ej. Topping). Cada botón trae su propio recargo en data-precio.
    function obtenerPersonalizacionesSeleccionadas() {
        const seleccionadas = [];
        form.querySelectorAll('[data-config^="personalizacion-"]').forEach(grupo => {
            grupo.querySelectorAll(".cb-chip.active").forEach(chip => {
                seleccionadas.push({
                    opcionId: parseInt(chip.dataset.opcionId, 10),
                    nombre: chip.dataset.valor,
                    precioAdicional: parseFloat(chip.dataset.precio) || 0,
                });
            });
        });
        return seleccionadas;
    }

    function precioSeleccionado() {
        const activo = form.querySelector('[data-config="tamano"] .active');
        const precioBase = activo ? parseFloat(activo.dataset.precio) : 0;
        const extra = obtenerPersonalizacionesSeleccionadas()
            .reduce((suma, p) => suma + p.precioAdicional, 0);
        return precioBase + extra;
    }

    function actualizarPrecioMostrado() {
        const precio = precioSeleccionado();
        if (precioMostrado) precioMostrado.textContent = `Q${precio.toFixed(2)}`;
        if (botonAgregar) botonAgregar.textContent = `Agregar al carrito · Q${precio.toFixed(2)}`;
    }

    // --- selección de tamaño/presentación (única) ---
    form.querySelectorAll(".cb-size-option").forEach(boton => {
        boton.addEventListener("click", () => {
            form.querySelectorAll(".cb-size-option.active").forEach(b => b.classList.remove("active"));
            boton.classList.add("active");
            actualizarPrecioMostrado();
        });
    });

    // --- chips de selección única o múltiple (personalizaciones) ---
    form.querySelectorAll("[data-config]").forEach(grupo => {
        if (grupo.classList.contains("cb-size-options")) return; // ya manejado arriba
        const multiple = grupo.dataset.multiple === "true";
        grupo.querySelectorAll(".cb-chip").forEach(boton => {
            boton.addEventListener("click", () => {
                if (!multiple) {
                    grupo.querySelectorAll(".active").forEach(b => b.classList.remove("active"));
                }
                boton.classList.toggle("active");
                actualizarPrecioMostrado();
            });
        });
    });

    // --- cantidad ---
    const qtyValue = form.querySelector(".cb-qty-value");
    form.querySelector(".cb-qty-minus")?.addEventListener("click", () => {
        const actual = parseInt(qtyValue.textContent, 10);
        if (actual > 1) qtyValue.textContent = actual - 1;
    });
    form.querySelector(".cb-qty-plus")?.addEventListener("click", () => {
        qtyValue.textContent = parseInt(qtyValue.textContent, 10) + 1;
    });

    actualizarPrecioMostrado();

    // --- agregar al carrito ---
    form.addEventListener("submit", e => {
        e.preventDefault();

        const tamanoBtn = form.querySelector('[data-config="tamano"] .active');
        const tamano = tamanoBtn?.dataset.valor ?? null;
        const personalizaciones = obtenerPersonalizacionesSeleccionadas();
        const precio = precioSeleccionado(); // precio base + recargo de personalizaciones ya sumado

        const item = {
            id: form.dataset.productoId,
            presentacionId: tamanoBtn?.dataset.presentacionId ? parseInt(tamanoBtn.dataset.presentacionId, 10) : null,
            nombre: form.dataset.productoNombre,
            imagenUrl: form.dataset.productoImagen || null,
            precio: precio,
            cantidad: parseInt(qtyValue.textContent, 10),
            opciones: { tamano },
            personalizaciones, // [{ opcionId, nombre, precioAdicional }, ...]
        };

        CapysCarrito.agregarProducto(item);

        const textoOriginal = botonAgregar.textContent;
        botonAgregar.textContent = "✓ Agregado al carrito";
        setTimeout(() => { botonAgregar.textContent = textoOriginal; }, 1600);
    });
});
