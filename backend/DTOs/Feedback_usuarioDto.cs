using NeuroPuentesAPI.models;

namespace NeuroPuentesAPI.DTOs
{
    public class FeedbackUsuarioReadDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
    }

    public class FeedbackUsuarioCreateDto
    {
        public int UsuarioId { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
    }

    public class FeedbackUsuarioUpdateDto
    {
        public string? Tipo { get; set; }
        public string? Mensaje { get; set; }
        public string? Categoria { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
