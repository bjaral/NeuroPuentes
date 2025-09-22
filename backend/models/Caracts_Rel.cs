using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeuroPuentesAPI.models
{
    public class Caracts_Rel
    {
        public required int Caracteristica_id { get; set; }
        public required int Contexto_id { get; set; }
    }
}
