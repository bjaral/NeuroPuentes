using System.Text.Json.Serialization;
using NeuroPuentesAPI.models;

namespace NeuroPuentesAPI.DTOs
{
    public class UsuarioReadDto
    {
        public int Id { get; set; }
        
        [JsonIgnore]
        public required string PasswordHash { get; set; } = String.Empty;
        
        public ENUM_TIPO_USUARIO Rol { get; set; }
        public bool Vigencia { get; set; }
        public DateTime FechaRegistro {get;set;}

        public required string NombreUsuario { get; set; } = String.Empty;
        public required string Nombre{ get; set; } = String.Empty;
        public required string Email { get; set; } = String.Empty;
    }

    public class UsuarioCreateDto
    { 
        public required string Password { get; set; } = String.Empty;
        public required ENUM_TIPO_USUARIO Rol { get; set; }
        public required bool Vigencia { get; set; }
       
        public required string NombreUsuario { get; set; } = String.Empty;
        public required string Nombre { get; set; } = String.Empty;
        public required string Email { get; set; } = String.Empty;
    }

    public class UsuarioUpdateDto
    { 
        public string? Password { get; set; } = String.Empty;
        public ENUM_TIPO_USUARIO? Rol { get; set; }
        public bool? Vigencia { get; set; }

        public string? NombreUsuario { get; set; } = String.Empty;
        public string? Nombre { get; set; } = String.Empty;
        public string? Email { get; set; } = String.Empty;
    }
}