using System.ComponentModel.DataAnnotations;

namespace NeuroPuentesAPI.models
{
    public class Calificacion_Usuario
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }


        // Cada punto de la calificacion es media estrella.
        [Required]
        [Range(1, 10, ErrorMessage = "La calificación debe estar entre 1 y 10.")]
        public int Calificacion { get; set; }

        public string? Mensaje { get; set; }

        public required DateTime Fecha { get; set; }
    }
}
