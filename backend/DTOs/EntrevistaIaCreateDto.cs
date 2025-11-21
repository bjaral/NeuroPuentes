using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace NeuroPuentesAPI.DTOs
{
    public class EntrevistaIaCreateDto
    {
        [Required]
        public IFormFile Audio { get; set; }

        [Required]
        public string ContextTraits { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int ContextoId { get; set; }

        [Required]
        public string Titulo { get; set; }

        [Required]
        public float DuracionMin { get; set; } 
    }
}