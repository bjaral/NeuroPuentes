namespace NeuroPuentesAPI.DTOs
{
    public class EntrevistaDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int ContextoId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public float DuracionMin { get; set; }
        public int NumeroTurnos { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string ContextoSnapshot { get; set; } = string.Empty;
    }

    public class EntrevistaCreateDto
    {
        public int UsuarioId { get; set; }
        public int ContextoId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public float DuracionMin { get; set; }
        public int NumeroTurnos { get; set; }
        public string ContextoSnapshot { get; set; } = string.Empty;
    }
}
