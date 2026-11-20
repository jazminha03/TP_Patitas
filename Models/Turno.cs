namespace TP_Patitas.Models
{
    public class Turno
    {
        public int IdTurnos { get; set; }
        public int IdProfesional { get; set; }
        public int IdMascota { get; set; }
        public DateTime FechaHora { get; set; }
        public bool Cancelado { get; set; }
        public DateTime? FechaCancelacion { get; set; }

        public string NombreProfesional { get; set; }
        public string NombreCategoria { get; set; }
        public string NombreMascota { get; set; }
    }
}