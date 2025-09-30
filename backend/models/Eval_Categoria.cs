namespace NeuroPuentesAPI.models
{
    public class Eval_Categoria
    {
        public int Id { get; set; }
        public int EvalEntrevistaId { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public float Score { get; set; }
    }
}
