// VER MAS PROFESIONALES

let botonVerMas = document.getElementById("btnVerMas");

if (botonVerMas != null)
{
    botonVerMas.addEventListener("click", function () {

        let lista = document.getElementById("listaProfesionales");
        let desde = lista.getElementsByClassName("tarjeta").length;

        fetch(botonVerMas.getAttribute("data-url") + "?desde=" + desde)
            .then(response => response.json())
            .then(datos => {

                if (datos.login == true)
                {
                    window.location.href = "/Account/Login";
                    return;
                }

                let contenido = "";

                for (let i = 0; i < datos.profesionales.length; i++)
                {
                    let p = datos.profesionales[i];

                    contenido += "<a class='tarjeta' href='" + p.url + "'>" +
                        "<img src='" + p.imagen + "' alt='" + p.nombre + "' />" +
                        "<div class='tarjeta-info'>" +
                        "<h3>" + p.nombre + "</h3>" +
                        "<span class='categoria'>" + p.categoria + "</span>" +
                        "</div></a>";
                }

                lista.innerHTML += contenido;

                if (datos.hayMas == false)
                {
                    botonVerMas.style.display = "none";
                }

            });

    });
}


// AGREGAR Y QUITAR MASCOTAS (REGISTRO)

let botonAgregarMascota = document.getElementById("btnAgregarMascota");

if (botonAgregarMascota != null)
{
    let contenedorMascotas = document.getElementById("mascotas");

    botonAgregarMascota.addEventListener("click", function () {

        let nuevaFila = contenedorMascotas.getElementsByClassName("fila-mascota")[0].cloneNode(true);

        nuevaFila.getElementsByTagName("input")[0].value = "";
        nuevaFila.getElementsByTagName("select")[0].selectedIndex = 0;

        contenedorMascotas.appendChild(nuevaFila);

    });

    // Quitar una fila (siempre queda al menos una)
    contenedorMascotas.addEventListener("click", function (e) {

        if (e.target.className == "boton-quitar")
        {
            let fila = e.target;

            while (fila.className != "fila-mascota")
            {
                fila = fila.parentElement;
            }

            if (contenedorMascotas.getElementsByClassName("fila-mascota").length > 1)
            {
                fila.remove();
            }
        }

    });
}


// HORARIOS LIBRES (DETALLE DEL PROFESIONAL)

let inputFecha = document.getElementById("fecha");

if (inputFecha != null)
{
    inputFecha.addEventListener("change", function () {

        let selectHora = document.getElementById("hora");
        let mensaje = document.getElementById("mensajeHorarios");

        mensaje.textContent = "";

        if (inputFecha.value == "")
        {
            selectHora.disabled = true;
            selectHora.innerHTML = "<option value=''>Primero elegí una fecha</option>";
            return;
        }

        let url = inputFecha.getAttribute("data-url") +
            "?idProfesional=" + inputFecha.getAttribute("data-profesional") +
            "&fecha=" + inputFecha.value;

        fetch(url)
            .then(response => response.json())
            .then(datos => {

                if (datos.login == true)
                {
                    window.location.href = "/Account/Login";
                    return;
                }

                if (datos.horarios.length == 0)
                {
                    selectHora.disabled = true;
                    selectHora.innerHTML = "<option value=''>Sin horarios libres</option>";
                    mensaje.textContent = datos.mensaje;
                }
                else
                {
                    let opciones = "<option value=''>Elegí un horario</option>";

                    for (let i = 0; i < datos.horarios.length; i++)
                    {
                        opciones += "<option value='" + datos.horarios[i] + "'>" + datos.horarios[i] + "</option>";
                    }

                    selectHora.disabled = false;
                    selectHora.innerHTML = opciones;
                }

            });

    });
}


// VALIDAR REGISTRO

function ValidarRegistro()
{
    let contrasena = document.getElementById("contrasena").value;

    if (contrasena.length < 6)
    {
        alert("La contraseña debe tener al menos 6 caracteres.");
        return false;
    }

    return true;
}

// CANCELAR TURNOS (MIS TURNOS)

let botonesCancelar = document.getElementsByClassName("boton-cancelar");

for (let i = 0; i < botonesCancelar.length; i++)
{
    botonesCancelar[i].addEventListener("click", function () {

        if (confirm("¿Seguro que querés cancelar este turno?") == false)
        {
            return;
        }

        let boton = this;
        let idTurno = boton.getAttribute("data-id");
        let listaProximos = document.getElementById("listaProximos");
        let mensajeTurnos = document.getElementById("mensajeTurnos");

        mensajeTurnos.textContent = "";

        fetch(listaProximos.getAttribute("data-url"), {
            method: "POST",
            headers: {
                "Content-Type": "application/x-www-form-urlencoded"
            },
            body: "idTurno=" + idTurno
        })
        .then(response => response.json())
        .then(datos => {

            if (datos.login == true)
            {
                window.location.href = "/Account/Login";
                return;
            }

            if (datos.ok == true)
            {
                let turno = boton;

                while (turno.className != "turno")
                {
                    turno = turno.parentElement;
                }

                turno.remove();

                if (listaProximos.getElementsByClassName("turno").length == 0)
                {
                    document.getElementById("sinProximos").hidden = false;
                }
            }
            else
            {
                mensajeTurnos.textContent = datos.mensaje;
            }

        });

    });
}