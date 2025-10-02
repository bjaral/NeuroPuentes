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
        public int Id { get; set; }
        
        public required string PasswordHash { get; set; } = String.Empty;
        public required ENUM_TIPO_USUARIO Rol { get; set; }
        public required bool Vigencia { get; set; }
        public required DateTime FechaRegistro {get;set;}

        public required string NombreUsuario { get; set; } = String.Empty;
        public required string Nombre{ get; set; } = String.Empty;
        public required string Email { get; set; } = String.Empty;
    }

}