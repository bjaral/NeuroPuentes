

using NeuroPuentesAPI.models;

namespace NeuroPuentesAPI.DTOs
{
    public class UsuarioUpdateDto
    { 
        public string? Password_hash { get; set; } = String.Empty;
        public ENUM_TIPO_USUARIO? Rol { get; set; }
        public bool? Vigencia { get; set; }
        // public DateTime Fecha_registro {get;set;} // la fecha de creacion no se modifica!

        public string? Nombre_usuario { get; set; } = String.Empty;
        public string? Nombre{ get; set; } = String.Empty;
        public string? Email { get; set; } = String.Empty;
    }
}