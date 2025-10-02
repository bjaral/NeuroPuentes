using System.ComponentModel.DataAnnotations;

namespace NeuroPuentesAPI.models
{
    public enum ENUM_ORIGEN_CONTEXTO
    {
        Preset,
        Ia,
        Manual
    }

    public enum ENUM_SCOPE_CONTEXTO
    {
        Global,
        Usuario
    }

    public class Contexto
    {
        public int Id { get; set; }
        
        public required string Nombre { get; set; } = String.Empty;
        public required string Descripcion { get; set; } = String.Empty;

        public required ENUM_SCOPE_CONTEXTO Scope { get; set; }
        public int? CreadoPor {get; set;} 
        public required ENUM_ORIGEN_CONTEXTO Origen { get; set; }

        public string? PromptSeed {get; set; }
        public required bool Vigencia { get; set; }

        public required DateTime FechaCreacion {get;set;}

    }

}