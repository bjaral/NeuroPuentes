using System.ComponentModel.DataAnnotations;

namespace NeuroPuentesAPI.models
{
    public enum ENUM_TIPO_FEEDBACK
    {
        Fortaleza, 
        Debilidad, 
        Sugerencia, 
        General
    }

    public class Feedback_Usuario
    {
        public int Id { get; set; }
        
        public required int UsuarioId { get; set; }
        public required ENUM_TIPO_FEEDBACK Tipo { get; set;}
        public required string Mensaje { get; set; } = String.Empty;
        public required string Categoria { get; set; } = String.Empty;

        public required DateTime Fecha { get; set; }
    }

}