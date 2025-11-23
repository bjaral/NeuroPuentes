using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace NeuroPuentesAPI.DTOs
{
    public class EntrevistaIaCreateDto
    {
        [Required]
        public required IFormFile Audio { get; set; }

        [Required]
        public required string ContextTraits { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int ContextoId { get; set; }

        [Required]
        public required string Titulo { get; set; }

        [Required]
        public float DuracionMin { get; set; }
    }

}