namespace NeuroPuentesAPI.DTOs
{
    public class Feedback_EntrevistaDto
    {
        public int Id { get; set; }
        public int EntrevistaId { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public DateTime Fecha { get; set; }
    }

    public class Feedback_EntrevistaCreateDto
    {
        public int EntrevistaId { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string? Categoria { get; set; }
    }
}
