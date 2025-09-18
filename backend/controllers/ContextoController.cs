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

        public ContextosController(IContextoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContextoReadDto>>> GetAll()
        {
            var contextos = await _service.GetAllAsync();
            var result = contextos.Select(c => new ContextoReadDto
            {
                _id = c._id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                Scope = c.Scope,
                Creado_por = c.Creado_por,
                Origen = c.Origen,
                Prompt_seed = c.Prompt_seed,
                Vigencia = c.Vigencia,
                Fecha_creacion = c.Fecha_creacion
            });
            return Ok(result);
        }

        [HttpGet("Vigentes")]
        public async Task<ActionResult<IEnumerable<ContextoReadDto>>> GetAllVigentes()
        {
            var contextos = await _service.GetAllVigentesAsync();
            var result = contextos.Select(c => new ContextoReadDto
            {
                _id = c._id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                Scope = c.Scope,
                Creado_por = c.Creado_por,
                Origen = c.Origen,
                Prompt_seed = c.Prompt_seed,
                Vigencia = c.Vigencia,
                Fecha_creacion = c.Fecha_creacion
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
                _id = contexto._id,
                Nombre = contexto.Nombre,
                Descripcion = contexto.Descripcion,
                Scope = contexto.Scope,
                Creado_por = contexto.Creado_por,
                Origen = contexto.Origen,
                Prompt_seed = contexto.Prompt_seed,
                Vigencia = contexto.Vigencia,
                Fecha_creacion = contexto.Fecha_creacion
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
                Creado_por = dto.Creado_por,
                Origen = dto.Origen,
                Prompt_seed = dto.Prompt_seed,
                Vigencia = dto.Vigencia,
                Fecha_creacion = DateTime.UtcNow
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
            if (dto.Creado_por.HasValue) existente.Creado_por = dto.Creado_por;
            if (dto.Origen.HasValue) existente.Origen = dto.Origen.Value;
            if (dto.Prompt_seed != null) existente.Prompt_seed = dto.Prompt_seed;
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
    }
}
