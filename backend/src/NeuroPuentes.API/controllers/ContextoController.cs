using Microsoft.AspNetCore.Mvc;
using NeuroPuentesAPI.models;
using NeuroPuentesAPI.services;
using NeuroPuentesAPI.DTOs;

namespace NeuroPuentesAPI.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContextosController : ControllerBase
    {
        private readonly IContextoService _service;
        // private readonly ICaracteristicaService _caracteristicaService;

        public ContextosController(IContextoService service)
        {
            _service = service;
            // _caracteristicaService = caracteristicaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContextoReadDto>>> GetAll()
        {
            var contextos = await _service.GetAllAsync();
            var result = contextos.Select(c => new ContextoReadDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                Scope = c.Scope,
                CreadoPor = c.CreadoPor,
                Origen = c.Origen,
                PromptSeed = c.PromptSeed,
                Vigencia = c.Vigencia,
                FechaCreacion = c.FechaCreacion
            });
            return Ok(result);
        }

        [HttpGet("Vigentes")]
        public async Task<ActionResult<IEnumerable<ContextoReadDto>>> GetAllVigentes()
        {
            var contextos = await _service.GetAllVigentesAsync();
            var result = contextos.Select(c => new ContextoReadDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                Scope = c.Scope,
                CreadoPor = c.CreadoPor,
                Origen = c.Origen,
                PromptSeed = c.PromptSeed,
                Vigencia = c.Vigencia,
                FechaCreacion = c.FechaCreacion
            });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContextoReadDto>> GetById(int id)
        {
            var contexto = await _service.GetByIdAsync(id);
            if (contexto == null) return NotFound();

            return Ok(new ContextoReadDto
            {
                Id = contexto.Id,
                Nombre = contexto.Nombre,
                Descripcion = contexto.Descripcion,
                Scope = contexto.Scope,
                CreadoPor = contexto.CreadoPor,
                Origen = contexto.Origen,
                PromptSeed = contexto.PromptSeed,
                Vigencia = contexto.Vigencia,
                FechaCreacion = contexto.FechaCreacion
            });
        }

        [HttpPost]
        public async Task<ActionResult<int>> Crear([FromBody] ContextoCreateDto dto)
        {
            var contexto = new Contexto
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Scope = dto.Scope,
                CreadoPor = dto.CreadoPor,
                Origen = dto.Origen,
                PromptSeed = dto.PromptSeed,
                Vigencia = dto.Vigencia,
                FechaCreacion = DateTime.UtcNow
            };

            var id = await _service.CrearAsync(contexto);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ContextoUpdateDto dto)
        {
            var existente = await _service.GetByIdAsync(id);
            if (existente == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.Nombre)) existente.Nombre = dto.Nombre;
            if (!string.IsNullOrWhiteSpace(dto.Descripcion)) existente.Descripcion = dto.Descripcion;
            if (dto.Scope.HasValue) existente.Scope = dto.Scope.Value;
            if (dto.CreadoPor.HasValue) existente.CreadoPor = dto.CreadoPor;
            if (dto.Origen.HasValue) existente.Origen = dto.Origen.Value;
            if (dto.PromptSeed != null) existente.PromptSeed = dto.PromptSeed;
            if (dto.Vigencia.HasValue) existente.Vigencia = dto.Vigencia.Value;

            await _service.ActualizarAsync(id, existente);
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var contexto = await _service.GetByIdAsync(id);
            if (contexto == null) return NotFound();

            await _service.EliminarAsync(id);
            return NoContent();
        }
        
        // Obtener contexto dada entrevista Id
        [HttpGet("entrevista/{entrevistaId}")]
        public async Task<ActionResult<ContextoReadDto>> GetByEntrevistaId(int entrevistaId)
        {
            var contexto = await _service.GetByEntrevistaIdAsync(entrevistaId);
            if (contexto == null) return NotFound();

            return Ok(new ContextoReadDto
            {
                Id = contexto.Id,
                Nombre = contexto.Nombre,
                Descripcion = contexto.Descripcion,
                Scope = contexto.Scope,
                CreadoPor = contexto.CreadoPor,
                Origen = contexto.Origen,
                PromptSeed = contexto.PromptSeed,
                Vigencia = contexto.Vigencia,
                FechaCreacion = contexto.FechaCreacion
            });
        }
    }
}
