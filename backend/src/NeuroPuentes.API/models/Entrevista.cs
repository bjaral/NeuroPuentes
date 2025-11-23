using System.ComponentModel.DataAnnotations;

namespace NeuroPuentesAPI.models
{
    public class Entrevista
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int ContextoId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public float DuracionMin { get; set; }

        [Required]
        public int NumeroTurnos { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaCierre { get; set; }

        [Required]
        public string ContextoSnapshot { get; set; } = string.Empty;

        // Colección de diálogos
        public virtual ICollection<Dialogo> Dialogos { get; set; } = new List<Dialogo>();

        // Propiedad de navegación
        public virtual Contexto? Contexto { get; set; }

        public virtual Usuario? Usuario { get; set; }
    }
}