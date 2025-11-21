using System.ComponentModel.DataAnnotations;

namespace NeuroPuentesAPI.DTOs
{
    public class CalificacionUsuarioReadDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int Calificacion { get; set; }
        public string? Mensaje { get; set; }
        public DateTime Fecha { get; set; }
    }

    public class CalificacionUsuarioCreateDto
    {
        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [Range(1, 10)]
        public int Calificacion { get; set; }

        public string? Mensaje { get; set; }
    }

    public class CalificacionUsuarioUpdateDto
    {
        // public int? UsuarioId { get; set; }
        public int? Calificacion { get; set; }
        public string? Mensaje { get; set; }
    }
}
