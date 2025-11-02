using Microsoft.AspNetCore.Mvc;
using NeuroPuentesAPI.models;
using NeuroPuentesAPI.services;
using NeuroPuentesAPI.DTOs;

namespace NeuroPuentesAPI.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackEntrevistaController : ControllerBase
    {
        private readonly IFeedbackEntrevistaService _service;

        public FeedbackEntrevistaController(IFeedbackEntrevistaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FeedbackEntrevistaReadDto>>> GetAll()
        {
            var feedbacks = await _service.GetAllAsync();
            return Ok(feedbacks.Select(f => new FeedbackEntrevistaReadDto
            {
                Id = f.Id,
                EntrevistaId = f.EntrevistaId,
                Tipo = f.Tipo,
                Mensaje = f.Mensaje,
                Categoria = f.Categoria,
                Fecha = f.Fecha
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FeedbackEntrevistaReadDto>> GetById(int id)
        {
            var feedback = await _service.GetByIdAsync(id);
            if (feedback == null) return NotFound();

            return Ok(new FeedbackEntrevistaReadDto
            {
                Id = feedback.Id,
                EntrevistaId = feedback.EntrevistaId,
                Tipo = feedback.Tipo,
                Mensaje = feedback.Mensaje,
                Categoria = feedback.Categoria,
                Fecha = feedback.Fecha
            });
        }


        [HttpPost]
        public async Task<ActionResult<int>> Crear([FromBody] FeedbackEntrevistaCreateDto dto)
        {
            var feedback = new Feedback_Entrevista
            {
                EntrevistaId = dto.EntrevistaId,
                Tipo = dto.Tipo,
                Mensaje = dto.Mensaje,
                Categoria = dto.Categoria,
                Fecha = DateTime.UtcNow
            };

            var id = await _service.CrearAsync(feedback);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] FeedbackEntrevistaUpdateDto dto)
        {
            var existente = await _service.GetByIdAsync(id);
            if (existente == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.Tipo)) existente.Tipo = dto.Tipo;
            if (!string.IsNullOrWhiteSpace(dto.Mensaje)) existente.Mensaje = dto.Mensaje;
            if (dto.Categoria != null) existente.Categoria = dto.Categoria;

            await _service.ActualizarAsync(id, existente);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var existente = await _service.GetByIdAsync(id);
            if (existente == null) return NotFound();

            await _service.EliminarAsync(id);
            return NoContent();
        }



        // busqueda por entrevista
        [HttpGet("entrevista/{entrevistaId}")]
        public async Task<ActionResult<IEnumerable<FeedbackEntrevistaReadDto>>> GetByEntrevistaId(int entrevistaId)
        {
            var feedbacks = await _service.GetByEntrevistaIdAsync(entrevistaId);
            return Ok(feedbacks.Select(f => new FeedbackEntrevistaReadDto
            {
                Id = f.Id,
                EntrevistaId = f.EntrevistaId,
                Tipo = f.Tipo,
                Mensaje = f.Mensaje,
                Categoria = f.Categoria,
                Fecha = f.Fecha
            }));
        }


    }
}
