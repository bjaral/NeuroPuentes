using NeuroPuentesAPI.models;

namespace NeuroPuentesAPI.DTOs
{
    public class UsuarioCreateDto
    { 
        //public int _id { get; set; }  // EL ID se asigna solo!

        public required string Password_hash { get; set; } = String.Empty;
        public ENUM_TIPO_USUARIO Rol { get; set; }
        public bool Vigencia { get; set; }
        // public DateTime Fecha_registro {get;set;} // esto tambien se deberia asignar solo

        public required string Nombre_usuario { get; set; } = String.Empty;
        public required string Nombre{ get; set; } = String.Empty;
        public required string Email { get; set; } = String.Empty;
    }
}