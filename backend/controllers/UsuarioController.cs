using Microsoft.AspNetCore.Mvc;
using NeuroPuentesAPI.DTOs;
using NeuroPuentesAPI.models;
using NeuroPuentesAPI.services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace NeuroPuentesAPI.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _service;
        private readonly IConfiguration _config;

        public UsuariosController(IUsuarioService service, IConfiguration config)
        {
            _service = service;
            _config = config;
        }

        // === LOGIN ENDPOINT ===
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto dto)
        {
            var usuario = await _service.AuthenticateAsync(dto.Nombre_usuario, dto.Password);
            if (usuario == null)
                return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos" });

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Nombre_usuario),
                new Claim(ClaimTypes.Role, usuario.Rol.ToString())
            };

            var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetAll()
        {
            var usuarios = await _service.GetAllAsync();
            foreach (var user in usuarios)
                user.Password_hash = "";
            return Ok(usuarios);
        }

        [HttpGet("Vigentes")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetAllVigentes()
        {
            var usuarios = await _service.GetAllVigentesAsync();
            foreach (var user in usuarios)
                user.Password_hash = "";
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetById(int id)
        {
            var usuario = await _service.GetByIdAsync(id);
            if (usuario == null) return NotFound();
            usuario.Password_hash = "";
            return Ok(usuario);
        }

        [HttpGet("tipos")]
        public ActionResult<IEnumerable<string>> GetTiposUsuario()
        {
            var tipos = Enum.GetNames(typeof(ENUM_TIPO_USUARIO));
            return Ok(tipos);
        }

        [HttpPost]
        public async Task<ActionResult> Crear([FromBody] UsuarioCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuario = new Usuario
            {
                Password_hash = dto.Password_hash,
                Rol = dto.Rol,
                Vigencia = dto.Vigencia,
                Fecha_registro = DateTime.UtcNow,
                Nombre_usuario = dto.Nombre_usuario,
                Nombre = dto.Nombre,
                Email = dto.Email
            };

            var id = await _service.CrearAsync(usuario);
            usuario.Password_hash = "";

            return Ok(new { id });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Actualizar(int id, [FromBody] UsuarioUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existente = await _service.GetByIdAsync(id);
            if (existente == null)
                return NotFound();

            var usuario = new Usuario
            {
                _id = id,
                Password_hash = string.IsNullOrWhiteSpace(dto.Password_hash)
                    ? existente.Password_hash
                    : BCrypt.Net.BCrypt.HashPassword(dto.Password_hash),
                Rol = dto.Rol ?? existente.Rol,
                Vigencia = dto.Vigencia ?? existente.Vigencia,
                Fecha_registro = existente.Fecha_registro,
                Nombre_usuario = dto.Nombre_usuario ?? existente.Nombre_usuario,
                Nombre = dto.Nombre ?? existente.Nombre,
                Email = dto.Email ?? existente.Email
            };

            await _service.ActualizarAsync(id, usuario);
            return Ok(new { mensaje = "Usuario actualizado correctamente" });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            await _service.EliminarAsync(id);
            return Ok(new { mensaje = "Usuario eliminado correctamente" });
        }
    }
}
