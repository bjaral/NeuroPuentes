namespace NeuroPuentesAPI.DTOs
{
    public class Eval_CategoriaDto
    {
        public int Id { get; set; }
        public int EvalEntrevistaId { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public float Score { get; set; }
    }

    public class Eval_CategoriaCreateDto
    {
        public int EvalEntrevistaId { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public float Score { get; set; }
    }
}
