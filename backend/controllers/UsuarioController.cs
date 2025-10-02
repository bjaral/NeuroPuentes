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

        public UsuariosController(IUsuarioService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetAll()
        {
            var usuarios = await _service.GetAllAsync();
            foreach (var user in usuarios)
                user.PasswordHash = "";
            return Ok(usuarios);
        }

        [HttpGet("Vigentes")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetAllVigentes()
        {
            var usuarios = await _service.GetAllVigentesAsync();
            foreach (var user in usuarios)
                user.PasswordHash = "";
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetById(int id)
        {
            var usuario = await _service.GetByIdAsync(id);
            if (usuario == null) return NotFound();
            usuario.PasswordHash = "";
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
                PasswordHash = dto.Password,
                Rol = dto.Rol,
                Vigencia = dto.Vigencia,
                FechaRegistro = DateTime.UtcNow,
                NombreUsuario = dto.NombreUsuario,
                Nombre = dto.Nombre,
                Email = dto.Email
            };

            var id = await _service.CrearAsync(usuario);
            usuario.PasswordHash = "";

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
                Id = id,
                PasswordHash = dto.Password ?? "", // Enviar contraseña plana al servicio
                Rol = dto.Rol ?? existente.Rol,
                Vigencia = dto.Vigencia ?? existente.Vigencia,
                FechaRegistro = existente.FechaRegistro,
                NombreUsuario = dto.NombreUsuario ?? existente.NombreUsuario,
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