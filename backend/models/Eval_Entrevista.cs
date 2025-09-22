using System.ComponentModel.DataAnnotations;

namespace NeuroPuentesAPI.models
{
    public class Eval_Entrevista
    {
        public int _id { get; set; }
        
        public required int Entrevista_id { get; set; }
        public required float Score_final { get; set; }
    }

}