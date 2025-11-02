namespace NeuroPuentesAPI.DTOs
{
    public class FeedbackEntrevistaReadDto
    {
        public int Id { get; set; }
        public int EntrevistaId { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public DateTime Fecha { get; set; }
    }

    public class FeedbackEntrevistaCreateDto
    {
        public int EntrevistaId { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string? Categoria { get; set; }
    }

    public class FeedbackEntrevistaUpdateDto
    {
        public string? Tipo { get; set; }
        public string? Mensaje { get; set; }
        public string? Categoria { get; set; }
    }
}
