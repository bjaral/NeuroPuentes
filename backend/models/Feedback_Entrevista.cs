using System;

namespace NeuroPuentesAPI.models
{
    public class Feedback_Entrevista
    {
        public int Id { get; set; }
        public int EntrevistaId { get; set; }
        public string Tipo { get; set; } = string.Empty; // fortaleza, debilidad, sugerencia, general
        public string Mensaje { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }
}
