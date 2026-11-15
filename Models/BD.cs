using Microsoft.Data.SqlClient;
using Dapper;

namespace TP_Patitas.Models
{
    public class BD
    {
        private static string _connectionString =
        @"Server= SQLEXPRESS;DataBase=TP_Patitas;Integrated Security=True;TrustServerCertificate=True;";

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

        public static void AgregarUsuario(Usuario usuario)
        {
            string query = @"INSERT INTO Usuarios
                            (NombreUsuario, Contrasena, Nombre, Apellido)
                            VALUES
                            (@NombreUsuario, @Contrasena, @Nombre, @Apellido)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Execute(query, usuario);
            }
        }
    }
}