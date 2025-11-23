using System.ComponentModel.DataAnnotations;

namespace NeuroPuentesAPI.models
{
    public class Eval_Categoria
    {
        public int Id { get; set; }

        public required int EvalEntrevistaId { get; set; }

        public required string Categoria { get; set; } = string.Empty;

        public required float Score { get; set; }
    }
}
