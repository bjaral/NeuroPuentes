using System.ComponentModel.DataAnnotations;

namespace NeuroPuentesAPI.models
{
    public enum ENUM_TIPO_USUARIO
    {
        Estudiante,
        Supervisor,
        Admin
    }
    public class Usuario
    {
        public int _id { get; set; }
        
        public required string Password_hash { get; set; } = String.Empty;
        public required ENUM_TIPO_USUARIO Rol { get; set; }
        public required bool Vigencia { get; set; }
        public required DateTime Fecha_registro {get;set;}

        public required string Nombre_usuario { get; set; } = String.Empty;
        public required string Nombre{ get; set; } = String.Empty;
        public required string Email { get; set; } = String.Empty;
    }

}