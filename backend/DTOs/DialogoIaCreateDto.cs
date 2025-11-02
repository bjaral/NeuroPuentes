using Microsoft.AspNetCore.Http;

namespace NeuroPuentesAPI.DTOs
{
    public class DialogoIaCreateDto
    {
        public IFormFile Audio { get; set; }
        public string ContextTraits { get; set; }
        public int EntrevistaId { get; set; }
        public int Turno { get; set; }
        public string Sender { get; set; } = "Estudiante"; // Valor por defecto
    }
}