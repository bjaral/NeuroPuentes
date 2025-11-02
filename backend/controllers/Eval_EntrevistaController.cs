using Microsoft.AspNetCore.Mvc;
using NeuroPuentesAPI.models;
using NeuroPuentesAPI.services;
using NeuroPuentesAPI.DTOs;

namespace NeuroPuentesAPI.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EvalEntrevistasController : ControllerBase
    {
        private readonly IEvalEntrevistaService _service;

        public EvalEntrevistasController(IEvalEntrevistaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EvalEntrevistaReadDto>>> GetAll()
        {
            var evals = await _service.GetAllAsync();
            var result = evals.Select(e => new EvalEntrevistaReadDto
            {
                Id = e.Id,
                EntrevistaId = e.EntrevistaId,
                ScoreFinal = e.ScoreFinal
            });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EvalEntrevistaReadDto>> GetById(int id)
        {
            var eval = await _service.GetByIdAsync(id);
            if (eval == null) return NotFound();

            return Ok(new EvalEntrevistaReadDto
            {
                Id = eval.Id,
                EntrevistaId = eval.EntrevistaId,
                ScoreFinal = eval.ScoreFinal
            });
        }

        [HttpPost]
        public async Task<ActionResult<int>> Crear([FromBody] EvalEntrevistaCreateDto dto)
        {
            var eval = new Eval_Entrevista
            {
                EntrevistaId = dto.EntrevistaId,
                ScoreFinal = dto.ScoreFinal
            };

            var id = await _service.CrearAsync(eval);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] EvalEntrevistaUpdateDto dto)
        {
            var existente = await _service.GetByIdAsync(id);
            if (existente == null) return NotFound();

            if (dto.EntrevistaId.HasValue) existente.EntrevistaId = dto.EntrevistaId.Value;
            if (dto.ScoreFinal.HasValue) existente.ScoreFinal = dto.ScoreFinal.Value;

            await _service.ActualizarAsync(id, existente);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eval = await _service.GetByIdAsync(id);
            if (eval == null) return NotFound();

            await _service.EliminarAsync(id);
            return NoContent();
        }


        // busqueda por entrevista Id
        [HttpGet("ByEntrevista/{entrevistaId}")]
        public async Task<ActionResult<EvalEntrevistaReadDto>> GetByEntrevistaId(int entrevistaId)
        {
            var eval = await _service.GetByEntrevistaIdAsync(entrevistaId);
            if (eval == null) return NotFound();

            return Ok(new EvalEntrevistaReadDto
            {
                Id = eval.Id,
                EntrevistaId = eval.EntrevistaId,
                ScoreFinal = eval.ScoreFinal
            });
        }
    }
}
