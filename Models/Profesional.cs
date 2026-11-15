namespace TP_Patitas.Models
{
    public class Profesional
    {
        public int IdProfesional { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Imagen { get; set; }
        public int IdCategoria { get; set; }
        public bool Activo { get; set; }

        // Viene del JOIN con Categorias (el alias de la query tiene que llamarse igual)
        public string NombreCategoria { get; set; }
    }
}