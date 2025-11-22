using NeuroPuentesAPI.models;

namespace NeuroPuentesAPI.DTOs
{
    public class StatsUsuarioReadDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaCorte { get; set; }
        public int TotalEntrevistas { get; set; }
        public float TiempoTotalMin { get; set; }
        public float ScorePromedio { get; set; }
    }

    public class StatsUsuarioCreateDto
    {
        public int UsuarioId { get; set; }
        public DateTime FechaCorte { get; set; } = DateTime.UtcNow;
        public int TotalEntrevistas { get; set; }
        public float TiempoTotalMin { get; set; }
        public float ScorePromedio { get; set; }
    }

    public class StatsUsuarioUpdateDto
    {
        public int? UsuarioId { get; set; }
        public DateTime? FechaCorte { get; set; }
        public int? TotalEntrevistas { get; set; }
        public float? TiempoTotalMin { get; set; }
        public float? ScorePromedio { get; set; }
    }


    // utilizado para extraer stats de usuario mas completo, extrae datos de otros modelos a traves del respositorio
    public class StatsUsuarioResumenDto
    {
        public int UsuarioId { get; set; }
        public int TotalEntrevistas { get; set; }
        public double TiempoTotalMin { get; set; }
        public double ScorePromedio { get; set; }
        public double FluidezPromedio { get; set; }
        public double EmpatiaPromedio { get; set; }
        public int FeedbackFortalezas { get; set; }
        public int FeedbackDebilidades { get; set; }
        public int ContextosDificiles { get; set; }
        public int ContextosFaciles { get; set; }
        public double NumTurnosPromedio { get; set; }
    }


}
