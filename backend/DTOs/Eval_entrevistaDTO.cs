namespace NeuroPuentesAPI.DTOs
{
    public class Eval_EntrevistaDto
    {
        public int Id { get; set; }
        public int EntrevistaId { get; set; }
        public float ScoreFinal { get; set; }
    }

    public class Eval_EntrevistaCreateDto
    {
        public int EntrevistaId { get; set; }
        public float ScoreFinal { get; set; }
    }
}
