using System;

namespace NeuroPuentesAPI.models
{
    public class Dialogo
    {
        public int Id { get; set; }
        public int EntrevistaId { get; set; }
        public int Turno { get; set; }
        public string Sender { get; set; } = string.Empty; // "user" o "ai"
        public string Texto { get; set; } = string.Empty;
        public string TextoProcesado { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? AudioUrl { get; set; }
    }
}
