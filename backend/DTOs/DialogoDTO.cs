using System;

namespace NeuroPuentesAPI.DTOs
{
    public class DialogoDto
    {
        public int Id { get; set; }
        public int EntrevistaId { get; set; }
        public int Turno { get; set; }
        public string Sender { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;
        public string TextoProcesado { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? AudioUrl { get; set; }
    }

    public class DialogoCreateDto
    {
        public int EntrevistaId { get; set; }
        public int Turno { get; set; }
        public string Sender { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;
        public string TextoProcesado { get; set; } = string.Empty;
        public string? AudioUrl { get; set; }
    }
}
