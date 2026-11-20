using Microsoft.Data.SqlClient;
using Dapper;

namespace TP_Patitas.Models
{
    public class BD
    {
        private static string _connectionString =
        @"Server=JazminHan\SQLEXPRESS;DataBase=TP_Patitas;Integrated Security=True;TrustServerCertificate=True;";

        public static Usuario ValidarUsuario(string nombreUsuario, string contrasena)
        {
            Usuario usuario = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM Usuarios
                                WHERE NombreUsuario = @nombreUsuario
                                AND Contrasena = @contrasena";

                usuario = connection.QueryFirstOrDefault<Usuario>(
                    query,
                    new { nombreUsuario, contrasena }
                );
            }

            return usuario;
        }

         public static Usuario BuscarUsuario(string nombreUsuario)
        {
            Usuario usuario = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Usuarios WHERE NombreUsuario = @nombreUsuario";

                usuario = connection.QueryFirstOrDefault<Usuario>(
                    query,
                    new { nombreUsuario }
                );
            }

            return usuario;
        }

        public static int AgregarUsuario(Usuario usuario)
        {
            int idUsuario = 0;

            string query = @"INSERT INTO Usuarios (NombreUsuario, Contrasena, Nombre, Apellido)
                            VALUES (@NombreUsuario, @Contrasena, @Nombre, @Apellido);
                            SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                idUsuario = connection.QueryFirstOrDefault<int>(query, usuario);
            }

            return idUsuario;
        }

        public static void AgregarMascota(Mascota mascota)
        {
            string query = @"INSERT INTO Mascotas (IdUsuario, Nombre, Especie)
                            VALUES (@IdUsuario, @Nombre, @Especie)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Execute(query, mascota);
            }
        }

        public static List<Profesional> ObtenerProfesionales(int desde, int cantidad)
        {
            List<Profesional> profesionales = new List<Profesional>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT p.IdProfesional, p.Nombre, p.Descripcion, p.Imagen,
                                        p.IdCategoria, p.Activo, c.Nombre AS NombreCategoria
                                FROM Profesionales p
                                INNER JOIN Categorias c ON c.IdCategoria = p.IdCategoria
                                WHERE p.Activo = 1
                                ORDER BY p.IdProfesional
                                OFFSET @desde ROWS FETCH NEXT @cantidad ROWS ONLY";

                profesionales = connection.Query<Profesional>(query, new { desde, cantidad }).ToList();
            }

            return profesionales;
        }

        public static int ContarProfesionales()
        {
            int cantidad = 0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(*) FROM Profesionales WHERE Activo = 1";
                cantidad = connection.QueryFirstOrDefault<int>(query);
            }

            return cantidad;
        }

        public static Profesional ObtenerProfesional(int idProfesional)
        {
            Profesional profesional = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT p.IdProfesional, p.Nombre, p.Descripcion, p.Imagen,
                                        p.IdCategoria, p.Activo, c.Nombre AS NombreCategoria
                                FROM Profesionales p
                                INNER JOIN Categorias c ON c.IdCategoria = p.IdCategoria
                                WHERE p.IdProfesional = @idProfesional AND p.Activo = 1";

                profesional = connection.QueryFirstOrDefault<Profesional>(query, new { idProfesional });
            }

            return profesional;
        }

        public static HorarioAtencion ObtenerHorario(int diaSemana)
        {
            HorarioAtencion horario = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM HorariosAtencion WHERE DiaSemana = @diaSemana";
                horario = connection.QueryFirstOrDefault<HorarioAtencion>(query, new { diaSemana });
            }

            return horario;
        }

        public static List<DateTime> ObtenerTurnosOcupados(int idProfesional, DateTime dia)
        {
            List<DateTime> ocupados = new List<DateTime>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT FechaHora FROM Turnos
                                WHERE IdProfesional = @idProfesional
                                AND Cancelado = 0
                                AND FechaHora >= @desde
                                AND FechaHora < @hasta";
                ocupados = connection.Query<DateTime>(query, new
                {
                    idProfesional,
                    desde = dia.Date,
                    hasta = dia.Date.AddDays(1)
                }).ToList();
            }

            return ocupados;
        }

        public static List<Mascota> ObtenerMascotasUsuario(int idUsuario)
        {
            List<Mascota> mascotas = new List<Mascota>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT IdMascotas, IdUsuario, RTRIM(Nombre) AS Nombre, Especie
                                FROM Mascotas WHERE IdUsuario = @idUsuario";

                mascotas = connection.Query<Mascota>(query, new { idUsuario }).ToList();
            }

            return mascotas;
        }

        public static Mascota ObtenerMascota(int idMascota, int idUsuario)
        {
            Mascota mascota = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT IdMascotas, IdUsuario, RTRIM(Nombre) AS Nombre, Especie
                                FROM Mascotas
                                WHERE IdMascotas = @idMascota AND IdUsuario = @idUsuario";

                mascota = connection.QueryFirstOrDefault<Mascota>(query, new { idMascota, idUsuario });
            }

            return mascota;
        }

        public static bool MascotaTieneTurno(int idMascota, DateTime fechaHora)
        {
            int cantidad = 0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT COUNT(*) FROM Turnos
                                WHERE IdMascota = @idMascota
                                AND FechaHora = @fechaHora
                                AND Cancelado = 0";

                cantidad = connection.QueryFirstOrDefault<int>(query, new { idMascota, fechaHora });
            }

            return cantidad > 0;
        }

        public static void AgregarTurno(int idProfesional, int idMascota, DateTime fechaHora)
        {
            string query = @"INSERT INTO Turnos (IdProfesional, IdMascota, FechaHora, Cancelado)
                            VALUES (@idProfesional, @idMascota, @fechaHora, 0)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Execute(query, new { idProfesional, idMascota, fechaHora });
            }
        }

        public static List<Turno> ObtenerTurnosUsuario(int idUsuario)
        {
            List<Turno> turnos = new List<Turno>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT t.IdTurnos, t.IdProfesional, t.IdMascota, t.FechaHora,
                                        t.Cancelado, t.FechaCancelacion,
                                        p.Nombre AS NombreProfesional,
                                        c.Nombre AS NombreCategoria,
                                        RTRIM(m.Nombre) AS NombreMascota
                                FROM Turnos t
                                INNER JOIN Profesionales p ON p.IdProfesional = t.IdProfesional
                                INNER JOIN Categorias c ON c.IdCategoria = p.IdCategoria
                                INNER JOIN Mascotas m ON m.IdMascotas = t.IdMascota
                                WHERE m.IdUsuario = @idUsuario
                                ORDER BY t.FechaHora";

                turnos = connection.Query<Turno>(query, new { idUsuario }).ToList();
            }

            return turnos;
        }

        public static Turno ObtenerTurnoUsuario(int idTurno, int idUsuario)
        {
            Turno turno = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT t.IdTurnos, t.IdProfesional, t.IdMascota, t.FechaHora,
                                        t.Cancelado, t.FechaCancelacion
                                FROM Turnos t
                                INNER JOIN Mascotas m ON m.IdMascotas = t.IdMascota
                                WHERE t.IdTurnos = @idTurno AND m.IdUsuario = @idUsuario";

                turno = connection.QueryFirstOrDefault<Turno>(query, new { idTurno, idUsuario });
            }

            return turno;
        }

        public static void CancelarTurno(int idTurno)
        {
            string query = @"UPDATE Turnos
                            SET Cancelado = 1, FechaCancelacion = @ahora
                            WHERE IdTurnos = @idTurno AND Cancelado = 0";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Execute(query, new { idTurno, ahora = DateTime.Now });
            }
        }

    }
}