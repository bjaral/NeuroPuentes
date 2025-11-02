using System.ComponentModel.DataAnnotations;

namespace NeuroPuentesAPI.DTOs
{
    public class EntrevistaReadDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int ContextoId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public float DuracionMin { get; set; }
        public int NumeroTurnos { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string ContextoSnapshot { get; set; } = string.Empty;
    }

    public class EntrevistaCreateDto
    {
        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int ContextoId { get; set; }

        [Required]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public float DuracionMin { get; set; }

        [Required]
        public int NumeroTurnos { get; set; }

        [Required]
        public string ContextoSnapshot { get; set; } = string.Empty;
    }

    public class EntrevistaUpdateDto
    {
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public float? DuracionMin { get; set; }
        public int? NumeroTurnos { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string? ContextoSnapshot { get; set; }
    }
}
