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
}
