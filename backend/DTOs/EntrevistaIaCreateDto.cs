using Microsoft.AspNetCore.Http;

namespace NeuroPuentesAPI.DTOs
{

    public class EntrevistaIaCreateDto
    {
        public IFormFile Audio { get; set; }
        public string ContextTraits { get; set; }
        public int UsuarioId { get; set; }
        public int ContextoId { get; set; }
        public string Titulo { get; set; }
    }
}