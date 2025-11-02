using Microsoft.AspNetCore.Mvc;
using NeuroPuentesAPI.models;
using NeuroPuentesAPI.services;
using NeuroPuentesAPI.DTOs;

namespace NeuroPuentesAPI.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EvalCategoriaController : ControllerBase
    {
        private readonly IEvalCategoriaService _service;

        public EvalCategoriaController(IEvalCategoriaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EvalCategoriaReadDto>>> GetAll()
        {
            var evals = await _service.GetAllAsync();
            var result = evals.Select(e => new EvalCategoriaReadDto
            {
                Id = e.Id,
                EvalEntrevistaId = e.EvalEntrevistaId,
                Categoria = e.Categoria,
                Score = e.Score
            });
            return Ok(result);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<EvalCategoriaReadDto>> GetById(int id)
        {
            var eval = await _service.GetByIdAsync(id);
            if (eval == null) return NotFound();

            return Ok(new EvalCategoriaReadDto
            {
                Id = eval.Id,
                EvalEntrevistaId = eval.EvalEntrevistaId,
                Categoria = eval.Categoria,
                Score = eval.Score
            });
        }

        [HttpPost]
        public async Task<ActionResult<int>> Crear([FromBody] EvalCategoriaCreateDto dto)
        {
            var eval = new Eval_Categoria
            {
                EvalEntrevistaId = dto.EvalEntrevistaId,
                Categoria = dto.Categoria,
                Score = dto.Score
            };

            var id = await _service.CrearAsync(eval);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] EvalCategoriaUpdateDto dto)
        {
            var existente = await _service.GetByIdAsync(id);
            if (existente == null) return NotFound();

            if (dto.EvalEntrevistaId.HasValue) existente.EvalEntrevistaId = dto.EvalEntrevistaId.Value;
            if (!string.IsNullOrWhiteSpace(dto.Categoria)) existente.Categoria = dto.Categoria;
            if (dto.Score.HasValue) existente.Score = dto.Score.Value;

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



        // busqueda por entrevistaId
        [HttpGet("porEntrevista/{entrevistaId}")]
        public async Task<ActionResult<IEnumerable<EvalCategoriaReadDto>>> GetByEntrevistaId(int entrevistaId)
        {
            var evals = await _service.GetByEntrevistaIdAsync(entrevistaId);
            var result = evals.Select(e => new EvalCategoriaReadDto
            {
                Id = e.Id,
                EvalEntrevistaId = e.EvalEntrevistaId,
                Categoria = e.Categoria,
                Score = e.Score
            });
            return Ok(result);
        }

    }
}
