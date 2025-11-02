namespace NeuroPuentesAPI.DTOs
{
    public class EvalEntrevistaReadDto
    {
        public int Id { get; set; }
        public int EntrevistaId { get; set; }
        public float ScoreFinal { get; set; }
    }

    public class EvalEntrevistaCreateDto
    {
        public int EntrevistaId { get; set; }
        public float ScoreFinal { get; set; }
    }

    public class EvalEntrevistaUpdateDto
    {
        public int? EntrevistaId { get; set; }
        public float? ScoreFinal { get; set; }
    }
}
