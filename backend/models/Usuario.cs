using System.ComponentModel.DataAnnotations;

namespace NeuroPuentesAPI.models
{
    public enum TipoUsuario
    {
        admin,
        emprendedora,
        visitante
    }
    public class Usuario
    {
        public int Id_Usuario { get; set; }
        public required string UsuarioNombre { get; set; } = String.Empty;
        public required string Contrasenna { get; set; } = String.Empty;
        public bool Vigencia { get; set; }
        public TipoUsuario Tipo_Usuario { get; set; }
        public required string Correo { get; set; } = String.Empty;
    }

    }