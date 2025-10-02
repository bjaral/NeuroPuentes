using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeuroPuentesAPI.models
{
    public class Tip
    {

        public int Id { get; set; }

        public required int UsuarioId { get; set; }
        public required int EntrevistaId { get; set; }

        public string? Titulo { get; set; }  
        public required string Contenido { get; set; } = string.Empty; 
        public required string Categoria { get; set; } = string.Empty; 

        public required DateTime Fecha { get; set; }
        public required bool Usado { get; set; }
        public required bool Vigencia { get; set; }
    }
}