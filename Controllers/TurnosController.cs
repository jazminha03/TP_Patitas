using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using TP_Patitas.Models;

namespace TP_Patitas.Controllers
{
    public class TurnosController : Controller
    {
        // La llama el fetch
        public IActionResult HorariosLibres(int idProfesional, string fecha)
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                return Json(new { login = true });
            }

            DateTime dia;
            if (!DateTime.TryParseExact(fecha, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dia))
            {
                return Json(new { horarios = new List<string>(), mensaje = "La fecha no es válida." });
            }

            if (dia.Date < DateTime.Today)
            {
                return Json(new { horarios = new List<string>(), mensaje = "No se pueden sacar turnos en fechas pasadas." });
            }

            HorarioAtencion horario = BD.ObtenerHorario((int)dia.DayOfWeek);
            List<string> libres = new List<string>();

            if (horario != null)
            {
                List<DateTime> ocupados = BD.ObtenerTurnosOcupados(idProfesional, dia);
                TimeSpan hora = horario.HoraApertura;

                while (hora + TimeSpan.FromMinutes(30) <= horario.HoraCierre)
                {
                    DateTime inicio = dia.Date + hora;

                    if (inicio > DateTime.Now && !ocupados.Contains(inicio))
                    {
                        libres.Add(hora.ToString(@"hh\:mm"));
                    }

                    hora = hora.Add(TimeSpan.FromMinutes(30));
                }
            }

            string mensaje = libres.Count == 0 ? "No quedan horarios libres ese día." : "";
            return Json(new { horarios = libres, mensaje = mensaje });
        }

        private IActionResult VolverAlDetalle(int idProfesional, string error)
        {
            return RedirectToAction("Detalle", "Profesionales", new { id = idProfesional, error = error });
        }

        [HttpPost]
        public IActionResult Crear(int idProfesional, string fecha, string hora, int idMascota)
        {
            // 1) Usuario
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // 2) Profesional
            Profesional profesional = BD.ObtenerProfesional(idProfesional);
            if (profesional == null)
            {
                return RedirectToAction("Index", "Profesionales");
            }

            // 3) Fecha
            DateTime dia;
            if (!DateTime.TryParseExact(fecha, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dia))
            {
                return VolverAlDetalle(idProfesional, "Elegí una fecha válida.");
            }

            // 4) Hora
            TimeSpan h;
            if (!TimeSpan.TryParseExact(hora, @"hh\:mm", CultureInfo.InvariantCulture, out h))
            {
                return VolverAlDetalle(idProfesional, "Elegí un horario.");
            }

            // 5) Mascota
            Mascota mascota = BD.ObtenerMascota(idMascota, idUsuario.Value);
            if (mascota == null)
            {
                return VolverAlDetalle(idProfesional, "Elegí una de tus mascotas.");
            }

            // 6) Horario de atención
            HorarioAtencion horario = BD.ObtenerHorario((int)dia.DayOfWeek);
            if (horario == null)
            {
                return VolverAlDetalle(idProfesional, "Ese día no se atiende.");
            }

            bool dentroDelHorario = h >= horario.HoraApertura && h + TimeSpan.FromMinutes(30) <= horario.HoraCierre;
            bool cadaMediaHora = (h - horario.HoraApertura).TotalMinutes % 30 == 0;

            if (!dentroDelHorario || !cadaMediaHora)
            {
                return VolverAlDetalle(idProfesional, "El horario " + hora + " no es válido ese día: se atiende de "
                    + horario.HoraApertura.ToString(@"hh\:mm") + " a " + horario.HoraCierre.ToString(@"hh\:mm")
                    + ", con turnos cada 30 minutos.");
            }

            // 7) Turnos de ese dia en adelante
            DateTime fechaHora = dia.Date + h;
            if (fechaHora <= DateTime.Now)
            {
                return VolverAlDetalle(idProfesional, "No se pueden sacar turnos en el pasado.");
            }

            // 8) cuando el turno esta ocupado
            List<DateTime> ocupados = BD.ObtenerTurnosOcupados(idProfesional, dia);
            if (ocupados.Contains(fechaHora))
            {
                return VolverAlDetalle(idProfesional, profesional.Nombre + " ya tiene un turno a las " + hora + ".");
            }

            // 9) La mascota ya tiene otro turno a esa hora
            if (BD.MascotaTieneTurno(idMascota, fechaHora))
            {
                return VolverAlDetalle(idProfesional, mascota.Nombre + " ya tiene un turno a las " + hora + " ese día.");
            }

            // se guarda
            BD.AgregarTurno(idProfesional, idMascota, fechaHora);
            return RedirectToAction("MisTurnos", "Turnos");
        }

        public IActionResult MisTurnos()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Account");
            }

            List<Turno> todos = BD.ObtenerTurnosUsuario(idUsuario.Value);
            List<Turno> proximos = new List<Turno>();
            List<Turno> pasados = new List<Turno>();

            foreach (Turno t in todos)
            {
                if (!t.Cancelado && t.FechaHora >= DateTime.Now)
                {
                    proximos.Add(t);
                }
                else
                {
                    pasados.Add(t);
                }
            }

            pasados.Reverse(); // el más reciente primero

            ViewBag.Proximos = proximos;
            ViewBag.Pasados = pasados;
            return View();
        }

        // La llama el fetch
        [HttpPost]
        public IActionResult Cancelar(int idTurno)
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null)
            {
                return Json(new { login = true });
            }

            Turno turno = BD.ObtenerTurnoUsuario(idTurno, idUsuario.Value);
            if (turno == null)
            {
                return Json(new { ok = false, mensaje = "El turno no existe." });
            }

            if (turno.Cancelado)
            {
                return Json(new { ok = false, mensaje = "El turno ya estaba cancelado." });
            }

            TimeSpan falta = turno.FechaHora - DateTime.Now;

            if (falta <= TimeSpan.Zero)
            {
                return Json(new { ok = false, mensaje = "El turno ya pasó, no se puede cancelar." });
            }
            //cancelar 24 horas antes
            if (falta < TimeSpan.FromHours(24))
            {
                return Json(new { ok = false, mensaje = "Solo se puede cancelar hasta 24 horas antes del turno." });
            }

            BD.CancelarTurno(idTurno);
            return Json(new { ok = true, mensaje = "Turno cancelado." });
        }
    }
}