using System;

namespace NeuroPuentesAPI.models
{
    public enum ENUM_SENDER_DIALOGO
    {
        User,
        Ai
    }

    public class Dialogo
    {
        public int Id { get; set; }
        public int EntrevistaId { get; set; }

        public int Turno { get; set; }
        public ENUM_SENDER_DIALOGO Sender { get; set; }
        public string Texto { get; set; } = string.Empty;
        public string TextoProcesado { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? AudioUrl { get; set; }

        // Propiedad de navegación
        public virtual Entrevista? Entrevista { get; set; }
    }
}
