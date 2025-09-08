using System.Text.Json.Serialization;

using NeuroPuentesAPI.models;

namespace NeuroPuentesAPI.DTOs

{
    public class UsuarioDto
    {
        public int _id { get; set; }
        
        [JsonIgnore]
        public required string Password_hash { get; set; } = String.Empty;
        
        public ENUM_TIPO_USUARIO Rol { get; set; }
        public bool Vigencia { get; set; }
        public DateTime Fecha_registro {get;set;}

        public required string Nombre_usuario { get; set; } = String.Empty;
        public required string Nombre{ get; set; } = String.Empty;
        public required string Email { get; set; } = String.Empty;
    }
}