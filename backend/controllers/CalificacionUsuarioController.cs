using Microsoft.AspNetCore.Mvc;
using NeuroPuentesAPI.models;
using NeuroPuentesAPI.services;
using NeuroPuentesAPI.DTOs;

namespace NeuroPuentesAPI.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalificacionUsuarioController : ControllerBase
    {
        private readonly ICalificacionUsuarioService _service;

        public CalificacionUsuarioController(ICalificacionUsuarioService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CalificacionUsuarioReadDto>>> GetAll()
        {
            var calificaciones = await _service.GetAllAsync();
            var result = calificaciones.Select(c => new CalificacionUsuarioReadDto
            {
                Id = c.Id,
                UsuarioId = c.UsuarioId,
                Calificacion = c.Calificacion,
                Mensaje = c.Mensaje,
                Fecha = c.Fecha
            });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CalificacionUsuarioReadDto>> GetById(int id)
        {
            var calificacion = await _service.GetByIdAsync(id);
            if (calificacion == null) return NotFound();

            return Ok(new CalificacionUsuarioReadDto
            {
                Id = calificacion.Id,
                UsuarioId = calificacion.UsuarioId,
                Calificacion = calificacion.Calificacion,
                Mensaje = calificacion.Mensaje,
                Fecha = calificacion.Fecha
            });
        }

        [HttpPost]
        public async Task<ActionResult<int>> Crear([FromBody] CalificacionUsuarioCreateDto dto)
        {
            var calificacion = new Calificacion_Usuario
            {
                UsuarioId = dto.UsuarioId,
                Calificacion = dto.Calificacion,
                Mensaje = dto.Mensaje,
                Fecha = DateTime.UtcNow
            };

            var id = await _service.CrearAsync(calificacion);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] CalificacionUsuarioUpdateDto dto)
        {
            var existente = await _service.GetByIdAsync(id);
            if (existente == null) return NotFound();

            // if (dto.UsuarioId.HasValue) existente.UsuarioId = dto.UsuarioId.Value;
            if (dto.Calificacion.HasValue) existente.Calificacion = dto.Calificacion.Value;
            if (dto.Mensaje != null) existente.Mensaje = dto.Mensaje;

            await _service.ActualizarAsync(id, existente);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var calificacion = await _service.GetByIdAsync(id);
            if (calificacion == null) return NotFound();

            await _service.EliminarAsync(id);
            return NoContent();
        }

        // obtener todas las calificaciones por usuario
        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<CalificacionUsuarioReadDto>>> GetByUsuarioId(int usuarioId)
        {
            var calificaciones = await _service.GetByUsuarioIdAsync(usuarioId);
            var result = calificaciones.Select(c => new CalificacionUsuarioReadDto
            {
                Id = c.Id,
                UsuarioId = c.UsuarioId,
                Calificacion = c.Calificacion,
                Mensaje = c.Mensaje,
                Fecha = c.Fecha
            });
            return Ok(result);
        }
    }
}
