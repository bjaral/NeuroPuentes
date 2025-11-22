namespace NeuroPuentesAPI.DTOs
{
    public class CaractsRelReadDto
    {
        public int CaracteristicaId { get; set; }
        public int ContextoId { get; set; }
    }

    public class CaractsRelCreateDto
    {
        public int CaracteristicaId { get; set; }
        public int ContextoId { get; set; }
    }
}
