namespace NeuroPuentesAPI.DTOs
{
    public class CaracteristicaReadDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Grupo { get; set; } = string.Empty;
        public bool Vigencia { get; set; }
    }

    public class CaracteristicaCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Grupo { get; set; } = string.Empty;
        public bool Vigencia { get; set; } = true;
    }

    public class CaracteristicaUpdateDto
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? Grupo { get; set; }
        public bool? Vigencia { get; set; }
    }
}
