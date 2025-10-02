using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeuroPuentesAPI.models
{
    public class Caracts_Rel
    {
        public required int CaracteristicaId { get; set; }
        public required int ContextoId { get; set; }
    }
}
