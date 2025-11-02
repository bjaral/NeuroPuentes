namespace NeuroPuentesAPI.DTOs
{
    public class EvalCategoriaReadDto
    {
        public int Id { get; set; }
        public int EvalEntrevistaId { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public float Score { get; set; }
    }

    public class EvalCategoriaCreateDto
    {
        public int EvalEntrevistaId { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public float Score { get; set; }
    }

    public class EvalCategoriaUpdateDto
    {
        public int? EvalEntrevistaId { get; set; }
        public string? Categoria { get; set; }
        public float? Score { get; set; }
    }
}
