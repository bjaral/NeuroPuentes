using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using NeuroPuentesAPI.models; // <-- Importante para el ENUM

namespace NeuroPuentesAPI.DTOs
{
    public class DialogoIaCreateDto
    {
        [Required]
        public IFormFile Audio { get; set; }

        [Required]
        public string ContextTraits { get; set; }

        [Required]
        public int EntrevistaId { get; set; }

        [Required]
        public int Turno { get; set; }
        
        // Usa el ENUM correcto
        public ENUM_SENDER_DIALOGO Sender { get; set; } = ENUM_SENDER_DIALOGO.User; 
    }
}