using Microsoft.AspNetCore.Mvc;
using NeuroPuentesAPI.models;
using NeuroPuentesAPI.services;
using NeuroPuentesAPI.DTOs;

namespace NeuroPuentesAPI.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Feedback_UsuarioController : ControllerBase
    {
        private readonly IFeedbackUsuarioService _service;

        public Feedback_UsuarioController(IFeedbackUsuarioService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FeedbackUsuarioReadDto>>> GetAll()
        {
            var feedbacks = await _service.GetAllAsync();
            var result = feedbacks.Select(f => new FeedbackUsuarioReadDto
            {
                Id = f.Id,
                UsuarioId = f.UsuarioId,
                Tipo = f.Tipo,
                Mensaje = f.Mensaje,
                Categoria = f.Categoria,
                Fecha = f.Fecha
            });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FeedbackUsuarioReadDto>> GetById(int id)
        {
            var feedback = await _service.GetByIdAsync(id);
            if (feedback == null) return NotFound();

            return Ok(new FeedbackUsuarioReadDto
            {
                Id = feedback.Id,
                UsuarioId = feedback.UsuarioId,
                Tipo = feedback.Tipo,
                Mensaje = feedback.Mensaje,
                Categoria = feedback.Categoria,
                Fecha = feedback.Fecha
            });
        }

        [HttpPost]
        public async Task<ActionResult<int>> Crear([FromBody] FeedbackUsuarioCreateDto dto)
        {
            var feedback = new Feedback_Usuario
            {
                UsuarioId = dto.UsuarioId,
                Tipo = dto.Tipo,
                Mensaje = dto.Mensaje,
                Categoria = dto.Categoria,
                Fecha = DateTime.UtcNow
            };

            var id = await _service.CrearAsync(feedback);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] FeedbackUsuarioUpdateDto dto)
        {
            var existente = await _service.GetByIdAsync(id);
            if (existente == null) return NotFound();

            if (dto.Tipo.HasValue) existente.Tipo = dto.Tipo.Value;
            if (!string.IsNullOrWhiteSpace(dto.Mensaje)) existente.Mensaje = dto.Mensaje;
            if (!string.IsNullOrWhiteSpace(dto.Categoria)) existente.Categoria = dto.Categoria;

            await _service.ActualizarAsync(id, existente);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var feedback = await _service.GetByIdAsync(id);
            if (feedback == null) return NotFound();

            await _service.EliminarAsync(id);
            return NoContent();
        }
    }
}
