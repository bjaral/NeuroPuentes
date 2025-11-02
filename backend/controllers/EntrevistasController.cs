using Microsoft.AspNetCore.Mvc;
using NeuroPuentesAPI.models;
using NeuroPuentesAPI.services;
using NeuroPuentesAPI.DTOs;

namespace NeuroPuentesAPI.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntrevistasController : ControllerBase
    {
        private readonly IEntrevistaService _service;

        public EntrevistasController(IEntrevistaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntrevistaReadDto>>> GetAll()
        {
            var entrevistas = await _service.GetAllAsync();
            var result = entrevistas.Select(e => new EntrevistaReadDto
            {
                Id = e.Id,
                UsuarioId = e.UsuarioId,
                ContextoId = e.ContextoId,
                Titulo = e.Titulo,
                Descripcion = e.Descripcion,
                DuracionMin = e.DuracionMin,
                NumeroTurnos = e.NumeroTurnos,
                FechaCreacion = e.FechaCreacion,
                FechaCierre = e.FechaCierre,
                ContextoSnapshot = e.ContextoSnapshot
            });
            return Ok(result);
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<EntrevistaReadDto>>> GetByUsuarioId(int usuarioId)
        {
            var entrevistas = await _service.GetByUsuarioIdAsync(usuarioId);
            var result = entrevistas.Select(e => new EntrevistaReadDto
            {
                Id = e.Id,
                UsuarioId = e.UsuarioId,
                ContextoId = e.ContextoId,
                Titulo = e.Titulo,
                Descripcion = e.Descripcion,
                DuracionMin = e.DuracionMin,
                NumeroTurnos = e.NumeroTurnos,
                FechaCreacion = e.FechaCreacion,
                FechaCierre = e.FechaCierre,
                ContextoSnapshot = e.ContextoSnapshot
            });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EntrevistaReadDto>> GetById(int id)
        {
            var entrevista = await _service.GetByIdAsync(id);
            if (entrevista == null) return NotFound();

            return Ok(new EntrevistaReadDto
            {
                Id = entrevista.Id,
                UsuarioId = entrevista.UsuarioId,
                ContextoId = entrevista.ContextoId,
                Titulo = entrevista.Titulo,
                Descripcion = entrevista.Descripcion,
                DuracionMin = entrevista.DuracionMin,
                NumeroTurnos = entrevista.NumeroTurnos,
                FechaCreacion = entrevista.FechaCreacion,
                FechaCierre = entrevista.FechaCierre,
                ContextoSnapshot = entrevista.ContextoSnapshot
            });
        }

        [HttpPost]
        public async Task<ActionResult<int>> Crear([FromBody] EntrevistaCreateDto dto)
        {
            var entrevista = new Entrevista
            {
                UsuarioId = dto.UsuarioId,
                ContextoId = dto.ContextoId,
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                DuracionMin = dto.DuracionMin,
                NumeroTurnos = dto.NumeroTurnos,
                ContextoSnapshot = dto.ContextoSnapshot,
                FechaCreacion = DateTime.UtcNow
            };

            var id = await _service.CrearAsync(entrevista);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] EntrevistaUpdateDto dto)
        {
            var existente = await _service.GetByIdAsync(id);
            if (existente == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.Titulo)) existente.Titulo = dto.Titulo;
            if (!string.IsNullOrWhiteSpace(dto.Descripcion)) existente.Descripcion = dto.Descripcion;
            if (dto.DuracionMin.HasValue) existente.DuracionMin = dto.DuracionMin.Value;
            if (dto.NumeroTurnos.HasValue) existente.NumeroTurnos = dto.NumeroTurnos.Value;
            if (dto.FechaCierre.HasValue) existente.FechaCierre = dto.FechaCierre.Value;
            if (dto.ContextoSnapshot != null) existente.ContextoSnapshot = dto.ContextoSnapshot;

            await _service.ActualizarAsync(id, existente);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var entrevista = await _service.GetByIdAsync(id);
            if (entrevista == null) return NotFound();

            await _service.EliminarAsync(id);
            return NoContent();
        }
    }
}
