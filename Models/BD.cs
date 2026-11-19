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

    }
}