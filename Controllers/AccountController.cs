
using Microsoft.AspNetCore.Mvc;
using TP_Patitas.Models;

namespace TP_Patitas.Controllers
{
    public class AccountController : Controller
    {
        // Mostrar Login
        public IActionResult Login()
        {
            return View();
        }

        // Procesar Login
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

        // Mostrar Registro
        public IActionResult Registro()
        {
            return View();
        }

        // Procesar Registro
        [HttpPost]
        public IActionResult Registro(Usuario usuario)
        {
            Usuario existente = BD.BuscarUsuario(usuario.NombreUsuario);

            if (existente != null)
            {
                ViewBag.Error = "Ese nombre de usuario ya está registrado.";
                return View();
            }

            BD.AgregarUsuario(usuario);

            return RedirectToAction("Login");
        }

        // Cerrar sesión
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}
