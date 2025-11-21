using NeuroPuentesAPI.models;

namespace NeuroPuentesAPI.DTOs
{
    public class DialogoReadDto
    {
        public int Id { get; set; }
        public int EntrevistaId { get; set; }
        public int Turno { get; set; }
        public ENUM_SENDER_DIALOGO Sender { get; set; }
        public string Texto { get; set; } = string.Empty;
        public string TextoProcesado { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? AudioUrl { get; set; }
    }

    public class DialogoCreateDto
    {
        public int EntrevistaId { get; set; }
        public int Turno { get; set; }
        public ENUM_SENDER_DIALOGO Sender { get; set; }
        public string Texto { get; set; } = string.Empty;
        public string TextoProcesado { get; set; } = string.Empty;
        public string? AudioUrl { get; set; }
    }

    public class DialogoUpdateDto
    {
        public int? Turno { get; set; }
        public ENUM_SENDER_DIALOGO? Sender { get; set; }
        public string? Texto { get; set; }
        public string? TextoProcesado { get; set; }
        public string? AudioUrl { get; set; }
    }
}
