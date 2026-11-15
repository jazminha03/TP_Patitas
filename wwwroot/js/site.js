// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


const btnVerMas = document.getElementById("btnVerMas");

if (btnVerMas) {
    btnVerMas.addEventListener("click", async function () {
        const lista = document.getElementById("listaProfesionales");

        // cuántos hay ya en pantalla = desde dónde pedir
        const desde = lista.querySelectorAll(".tarjeta").length;

        const respuesta = await fetch(btnVerMas.dataset.url + "?desde=" + desde);
        const datos = await respuesta.json();

        if (datos.login) {
            window.location.href = "/Account/Login";
            return;
        }

        datos.profesionales.forEach(function (p) {
            const a = document.createElement("a");
            a.className = "tarjeta";
            a.href = p.url;

            const img = document.createElement("img");
            img.src = p.imagen;
            img.alt = p.nombre;

            const info = document.createElement("div");
            info.className = "tarjeta-info";

            const h3 = document.createElement("h3");
            h3.textContent = p.nombre;

            const span = document.createElement("span");
            span.textContent = p.categoria;

            info.appendChild(h3);
            info.appendChild(span);
            a.appendChild(img);
            a.appendChild(info);
            lista.appendChild(a);
        });

        // si no quedan más, el botón desaparece
        if (!datos.hayMas) {
            btnVerMas.remove();
        }
    });
}