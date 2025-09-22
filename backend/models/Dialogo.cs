using System.ComponentModel.DataAnnotations;

namespace NeuroPuentesAPI.models
{

    public enum ENUM_SENDER_DIALOGO
    {
        User,
        Ai
    }

    public class Dialogo
    {
        public int _id { get; set; }
        
        public required int Entrevista_id { get; set; }
        public required int Turno { get; set; }

        public required ENUM_SENDER_DIALOGO Sender { get; set; }
        public required string Texto {get; set; } = String.Empty;
        public required string Texto_procesado {get; set; } = String.Empty;
        public required DateTime Timestamp { get; set; }

        public string? Audio_url { get; set; }
    }

}