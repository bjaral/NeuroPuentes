using NeuroPuentesAPI.models;

namespace NeuroPuentesAPI.DTOs
{
    public class TipReadDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int EntrevistaId { get; set; }
        public string? Titulo { get; set; }
        public string Contenido { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public bool Usado { get; set; }
        public bool Vigencia { get; set; }
    }

    public class TipCreateDto
    {
        public int UsuarioId { get; set; }
        public int EntrevistaId { get; set; }
        public string? Titulo { get; set; }
        public string Contenido { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public bool Usado { get; set; } = true;
        public bool Vigencia { get; set; } = true;
    }

    public class TipUpdateDto
    {
        public int? UsuarioId { get; set; }
        public int? EntrevistaId { get; set; }
        public string? Titulo { get; set; }
        public string? Contenido { get; set; }
        public string? Categoria { get; set; }
        public bool? Usado { get; set; }
        public bool? Vigencia { get; set; }
    }
}
