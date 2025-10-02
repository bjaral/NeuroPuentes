using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeuroPuentesAPI.models
{
    public class Caracteristica
    {

        public int Id { get; set; }

        public required string Nombre { get; set; } = string.Empty;
        public required string Descripcion { get; set; } = string.Empty;
        public required string Grupo { get; set; } = string.Empty;
        public required bool Vigencia { get; set; }
    }
}
