namespace TP_Patitas.Models
{
    public class HorarioAtencion
    {
        public int IdHxA { get; set; }
        public int DiaSemana { get; set; }
        public TimeSpan HoraApertura { get; set; }
        public TimeSpan HoraCierre { get; set; }
    }
}