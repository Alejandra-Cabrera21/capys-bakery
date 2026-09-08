// mis-favoritos.js — Página "Mis favoritos" (Cuenta/MisFavoritos).
// Los favoritos son solo una lista de ids de producto guardada en
// localStorage bajo "capys_favoritos" (ver configurador.js, botón ♥ en el
// detalle de producto). Como esa lista no guarda nombre/precio/imagen, aquí
// se cruza contra window.CAPYS_PRODUCTOS (el catálogo completo, servido por
// la vista) para poder armar las tarjetas.
//
// TODO (BD real): cuando los favoritos se guarden por id_usuario en vez de
// en el navegador, esta página deja de depender de localStorage y consulta
// directamente al servidor.

const CapysMisFavoritos = (() => {
    const CLAVE_FAVORITOS = "capys_favoritos";

    function obtenerIdsFavoritos() {
        return JSON.parse(localStorage.getItem(CLAVE_FAVORITOS) || "[]");
    }

    function quitarDeFavoritos(id) {
        const favoritos = obtenerIdsFavoritos().filter(f => f !== String(id));
        localStorage.setItem(CLAVE_FAVORITOS, JSON.stringify(favoritos));
    }

    function agregarAlCarritoRapido(producto) {
        // No hay selector de tamaño aquí, así que se usa el precio "desde"
        // (la presentación más económica) como en la tarjeta del catálogo.
        CapysCarrito.agregarProducto({
            id: String(producto.id),
            presentacionId: producto.presentacionId ?? null,
            nombre: producto.nombre,
            precio: producto.precio,
            cantidad: 1,
            opciones: { tamano: null, color: null, toppings: [] },
        });
    }

    function renderizar() {
        const idsFavoritos = obtenerIdsFavoritos();
        const catalogo = window.CAPYS_PRODUCTOS || [];
        const favoritos = catalogo.filter(p => idsFavoritos.includes(String(p.id)));

        const contenedor = document.getElementById("cb-favoritos-lista");
        const vacio = document.getElementById("cb-favoritos-vacio");
        if (!contenedor) return;

        if (favoritos.length === 0) {
            contenedor.innerHTML = "";
            if (vacio) vacio.style.display = "block";
            return;
        }
        if (vacio) vacio.style.display = "none";

        contenedor.innerHTML = favoritos.map(producto => {
            const fondoImagen = producto.imagenUrl
                ? `background-image:url('${producto.imagenUrl}'); background-size:cover; background-position:center;`
                : `background:linear-gradient(135deg,#F2D6C9,#E8B876);`;

            return `
                <div class="cb-product-card" data-favorito-id="${producto.id}">
                    <a href="/Catalogo/Detalle/${producto.id}" class="cb-product-img" style="${fondoImagen}; display:block;"></a>
                    <div class="cb-product-body">
                        <div class="cb-product-tag">${producto.categoria}</div>
                        <a href="/Catalogo/Detalle/${producto.id}" class="cb-product-name" style="text-decoration:none; color:inherit;">${producto.nombre}</a>
                        <div class="cb-product-foot">
                            <span class="cb-product-price">Desde Q${Number(producto.precio).toFixed(2)}</span>
                        </div>
                        <div class="cb-favoritos-acciones">
                            <button type="button" class="btn cb-btn-outline cb-quitar-favorito">Quitar</button>
                            <button type="button" class="btn cb-btn-plum cb-agregar-carrito-rapido">Agregar al carrito</button>
                        </div>
                    </div>
                </div>
            `;
        }).join("");

        contenedor.querySelectorAll(".cb-quitar-favorito").forEach(boton => {
            boton.addEventListener("click", () => {
                const id = boton.closest("[data-favorito-id]").dataset.favoritoId;
                quitarDeFavoritos(id);
                renderizar();
            });
        });

        contenedor.querySelectorAll(".cb-agregar-carrito-rapido").forEach(boton => {
            boton.addEventListener("click", () => {
                const id = boton.closest("[data-favorito-id]").dataset.favoritoId;
                const producto = favoritos.find(p => String(p.id) === id);
                if (!producto) return;
                agregarAlCarritoRapido(producto);
                const textoOriginal = boton.textContent;
                boton.textContent = "✓ Agregado";
                setTimeout(() => { boton.textContent = textoOriginal; }, 1400);
            });
        });
    }

    return { renderizar };
})();
