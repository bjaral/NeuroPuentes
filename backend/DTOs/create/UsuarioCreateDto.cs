namespace NeuroPuentesAPI.DTOs
{
    public class UsuarioCreateDto
    {
        public required string Usuario { get; set; }
        public required string Contrasenna { get; set; }
        public bool Vigencia { get; set; }
        public required string Tipo_Usuario { get; set; }
        public required string Correo { get; set; }
    }
}