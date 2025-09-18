using System.ComponentModel.DataAnnotations;

namespace NeuroPuentesAPI.models
{
    public class Eval_Categoria
    {
        public int _id { get; set; }
        
        public required int Eval_entrevista_id { get; set; }
        public required string Categoria { get; set; } = String.Empty;
        public required float Score { get; set; }
    }

}