using System.ComponentModel.DataAnnotations;

namespace NeuroPuentesAPI.models
{


    public class Feedback_Usuario
    {
        public int _id { get; set; }
        
        public required int Usuario_id { get; set; }
        public required ENUM_TIPO_FEEDBACK Tipo { get; set;}
        public required string Mensaje { get; set; } = String.Empty;
        public required string Categoria { get; set; } = String.Empty;

        public required DateTime Fecha { get; set; }
    }

}