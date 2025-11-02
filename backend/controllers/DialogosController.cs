using Microsoft.AspNetCore.Mvc;
using NeuroPuentesAPI.models;
using NeuroPuentesAPI.services;
using NeuroPuentesAPI.DTOs;

namespace NeuroPuentesAPI.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DialogosController : ControllerBase
    {
        private readonly IDialogoService _service;

        public DialogosController(IDialogoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DialogoReadDto>>> GetAll()
        {
            var dialogos = await _service.GetAllAsync();
            var result = dialogos.Select(d => new DialogoReadDto
            {
                Id = d.Id,
                EntrevistaId = d.EntrevistaId,
                Turno = d.Turno,
                Sender = d.Sender,
                Texto = d.Texto,
                TextoProcesado = d.TextoProcesado,
                Timestamp = d.Timestamp,
                AudioUrl = d.AudioUrl
            });
            return Ok(result);
        }

        [HttpGet("entrevista/{entrevistaId}")]
        public async Task<ActionResult<IEnumerable<DialogoReadDto>>> GetByEntrevistaId(int entrevistaId)
        {
            var dialogos = await _service.GetByEntrevistaIdAsync(entrevistaId);
            return Ok(dialogos.Select(d => new DialogoReadDto
            {
                Id = d.Id,
                EntrevistaId = d.EntrevistaId,
                Turno = d.Turno,
                Sender = d.Sender,
                Texto = d.Texto,
                TextoProcesado = d.TextoProcesado,
                Timestamp = d.Timestamp,
                AudioUrl = d.AudioUrl
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DialogoReadDto>> GetById(int id)
        {
            var d = await _service.GetByIdAsync(id);
            if (d == null) return NotFound();

            return Ok(new DialogoReadDto
            {
                Id = d.Id,
                EntrevistaId = d.EntrevistaId,
                Turno = d.Turno,
                Sender = d.Sender,
                Texto = d.Texto,
                TextoProcesado = d.TextoProcesado,
                Timestamp = d.Timestamp,
                AudioUrl = d.AudioUrl
            });
        }

        [HttpPost]
        public async Task<ActionResult<int>> Crear([FromBody] DialogoCreateDto dto)
        {
            var dialogo = new Dialogo
            {
                EntrevistaId = dto.EntrevistaId,
                Turno = dto.Turno,
                Sender = dto.Sender,
                Texto = dto.Texto,
                TextoProcesado = dto.TextoProcesado,
                Timestamp = DateTime.UtcNow,
                AudioUrl = dto.AudioUrl
            };

            var id = await _service.CrearAsync(dialogo);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] DialogoUpdateDto dto)
        {
            var existente = await _service.GetByIdAsync(id);
            if (existente == null) return NotFound();

            if (dto.Turno.HasValue) existente.Turno = dto.Turno.Value;
            if (dto.Sender.HasValue) existente.Sender = dto.Sender.Value;
            if (dto.Texto != null) existente.Texto = dto.Texto;
            if (dto.TextoProcesado != null) existente.TextoProcesado = dto.TextoProcesado;
            if (dto.AudioUrl != null) existente.AudioUrl = dto.AudioUrl;

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
    }
}
