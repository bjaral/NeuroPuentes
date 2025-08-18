namespace NeuroPuentesAPI.DTOs
{
    public class UsuarioDto
    {
        public int Id_Usuario { get; set; }
        public required string Usuario { get; set; }
        public required string Contrasenna { get; set; }
        public bool Vigencia { get; set; }
        public required string Tipo_Usuario { get; set; }
        public required string Correo { get; set; }
    }
}