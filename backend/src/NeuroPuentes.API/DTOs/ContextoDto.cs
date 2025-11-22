using NeuroPuentesAPI.models;

namespace NeuroPuentesAPI.DTOs
{
    public class ContextoReadDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public ENUM_SCOPE_CONTEXTO Scope { get; set; }
        public int? CreadoPor { get; set; }
        public ENUM_ORIGEN_CONTEXTO Origen { get; set; }
        public string? PromptSeed { get; set; }
        public bool Vigencia { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class ContextoCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public ENUM_SCOPE_CONTEXTO Scope { get; set; }
        public int? CreadoPor { get; set; }
        public ENUM_ORIGEN_CONTEXTO Origen { get; set; }
        public string? PromptSeed { get; set; }
        public bool Vigencia { get; set; } = true;
    }

    public class ContextoUpdateDto
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public ENUM_SCOPE_CONTEXTO? Scope { get; set; }
        public int? CreadoPor { get; set; }
        public ENUM_ORIGEN_CONTEXTO? Origen { get; set; }
        public string? PromptSeed { get; set; }
        public bool? Vigencia { get; set; }
    }
}
