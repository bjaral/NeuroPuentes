using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using NeuroPuentesAPI.services;
using NeuroPuentesAPI.models;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IConfiguration _configuration;

        public AuthController(IUsuarioService usuarioService, IConfiguration configuration)
        {
            _usuarioService = usuarioService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            Usuario? user = null;

            // Permitir login por correo O nombre de usuario
            if (!string.IsNullOrEmpty(request.Email))
            {
                user = await _usuarioService.AuthenticateByCorreoAsync(request.Email, request.Password);
            }
            else if (!string.IsNullOrEmpty(request.NombreUsuario))
            {
                user = await _usuarioService.AuthenticateAsync(request.NombreUsuario, request.Password);
            }

            if (user == null || !user.Vigencia)
                return Unauthorized("Credenciales inválidas o usuario inactivo.");

            var token = GenerateJwtToken(user);
            return Ok(new
            {
                token,
                usuario = new {
                    user.Id,
                    user.NombreUsuario,
                    user.Email,
                    user.Rol
                }
            });
        }

        private string GenerateJwtToken(Usuario user)
        {
            var keyString = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(keyString))
                throw new InvalidOperationException("Jwt:Key no está configurado.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.NombreUsuario),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Rol.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string? NombreUsuario { get; set; }
        public string? Email { get; set; }
        public required string Password { get; set; }
    }
}