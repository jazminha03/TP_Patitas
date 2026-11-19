using Microsoft.AspNetCore.Mvc;
using TP_Patitas.Models;

namespace TP_Patitas.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string nombreUsuario, string contrasena)
        {
            Usuario usuario = BD.ValidarUsuario(nombreUsuario, contrasena);

            if (usuario != null)
            {
                HttpContext.Session.SetInt32("IdUsuario", usuario.IdUsuario);
                HttpContext.Session.SetString("Nombre", usuario.Nombre);

                return RedirectToAction("Index", "Profesionales");
            }

            ViewBag.Error = "Usuario o contraseña incorrectos.";

            return View();
        }

        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registro(Usuario usuario, List<string> nombreMascota, List<string> especie)
        {
            Usuario existente = BD.BuscarUsuario(usuario.NombreUsuario);

            if (existente != null)
            {
                ViewBag.Error = "Ese nombre de usuario ya está registrado.";
                return View();
            }

            // Armamos la lista de mascotas, ignorando filas vacías
            List<Mascota> mascotas = new List<Mascota>();
            for (int i = 0; i < nombreMascota.Count; i++)
            {
                string nombre = nombreMascota[i]?.Trim();

                if (!string.IsNullOrEmpty(nombre) && i < especie.Count)
                {
                    if (nombre.Length > 10)
                    {
                        ViewBag.Error = "El nombre de cada mascota puede tener hasta 10 caracteres.";
                        return View();
                    }

                    Mascota m = new Mascota();
                    m.Nombre = nombre;
                    m.Especie = especie[i];
                    mascotas.Add(m);
                }
            }

            if (mascotas.Count == 0)
            {
                ViewBag.Error = "Tenés que registrar al menos una mascota.";
                return View();
            }

            int idUsuario = BD.AgregarUsuario(usuario);

            foreach (Mascota m in mascotas)
            {
                m.IdUsuario = idUsuario;
                BD.AgregarMascota(m);
            }

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}