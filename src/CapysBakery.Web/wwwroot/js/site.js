// JavaScript general del sitio.
// Sprint 1+: lógica del menú hamburguesa en mobile, contador del carrito, etc.

// Menú hamburguesa (mobile): abre/cierra el panel de navegación.
document.addEventListener("DOMContentLoaded", () => {
    const boton = document.getElementById("cb-hamburger-btn");
    const menu = document.getElementById("cb-mobile-menu");
    if (!boton || !menu) return;

    boton.addEventListener("click", () => {
        const abierto = menu.classList.toggle("cb-open");
        boton.setAttribute("aria-expanded", abierto ? "true" : "false");
    });

    // Cierra el menú al elegir una opción, para no tener que cerrarlo a mano.
    menu.querySelectorAll("a").forEach(link => {
        link.addEventListener("click", () => {
            menu.classList.remove("cb-open");
            boton.setAttribute("aria-expanded", "false");
        });
    });

    // Cierra el menú si tocan fuera de él.
    document.addEventListener("click", (e) => {
        if (!menu.contains(e.target) && !boton.contains(e.target)) {
            menu.classList.remove("cb-open");
            boton.setAttribute("aria-expanded", "false");
        }
    });
});

// Dropdowns del nav ("Panel", "Mi cuenta"): clic para abrir/cerrar. En
// mobile no hacen nada (ver CSS: ahí quedan siempre expandidos como lista
// plana dentro del menú hamburguesa).
document.addEventListener("DOMContentLoaded", () => {
    const dropdowns = document.querySelectorAll(".cb-nav-dropdown");
    if (!dropdowns.length) return;

    dropdowns.forEach(dropdown => {
        const trigger = dropdown.querySelector(".cb-nav-dropdown-trigger");
        trigger?.addEventListener("click", () => {
            const yaAbierto = dropdown.classList.contains("cb-open");
            dropdowns.forEach(d => d.classList.remove("cb-open"));
            if (!yaAbierto) dropdown.classList.add("cb-open");
        });
    });

    document.addEventListener("click", (e) => {
        dropdowns.forEach(dropdown => {
            if (!dropdown.contains(e.target)) dropdown.classList.remove("cb-open");
        });
    });
});
