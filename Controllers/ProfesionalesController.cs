using Microsoft.AspNetCore.Mvc;
using TP_Patitas.Models;

namespace TP_Patitas.Controllers
{
    public class ProfesionalesController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Profesionales = BD.ObtenerProfesionales(0, 8);
            ViewBag.HayMas = BD.ContarProfesionales() > 8;
            return View();
        }

        public IActionResult CargarMas(int desde)
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                return Json(new { login = true });
            }

            List<Profesional> profesionales = BD.ObtenerProfesionales(desde, 8);
            bool hayMas = BD.ContarProfesionales() > desde + profesionales.Count;

            var lista = profesionales.Select(p => new
            {
                id = p.IdProfesional,
                nombre = p.Nombre,
                imagen = p.Imagen,
                categoria = p.NombreCategoria,
                url = Url.Action("Detalle", "Profesionales", new { id = p.IdProfesional })
            });

            return Json(new { profesionales = lista, hayMas = hayMas });
        }

        public IActionResult Detalle(int id, string error)
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Profesional profesional = BD.ObtenerProfesional(id);

            if (profesional == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Profesional = profesional;
            ViewBag.Mascotas = BD.ObtenerMascotasUsuario(idUsuario.Value);
            ViewBag.Error = error;
            return View();
        }
}
}