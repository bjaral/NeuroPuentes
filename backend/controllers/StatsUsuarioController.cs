using Microsoft.AspNetCore.Mvc;
using NeuroPuentesAPI.models;
using NeuroPuentesAPI.services;
using NeuroPuentesAPI.DTOs;

namespace NeuroPuentesAPI.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatsUsuarioController : ControllerBase
    {
        private readonly IStatsUsuarioService _service;

        public StatsUsuarioController(IStatsUsuarioService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StatsUsuarioReadDto>>> GetAll()
        {
            var stats = await _service.GetAllAsync();
            var result = stats.Select(s => new StatsUsuarioReadDto
            {
                Id = s.Id,
                UsuarioId = s.UsuarioId,
                FechaCorte = s.FechaCorte,
                TotalEntrevistas = s.TotalEntrevistas,
                TiempoTotalMin = s.TiempoTotalMin,
                ScorePromedio = s.ScorePromedio
            });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StatsUsuarioReadDto>> GetById(int id)
        {
            var stats = await _service.GetByIdAsync(id);
            if (stats == null) return NotFound();

            return Ok(new StatsUsuarioReadDto
            {
                Id = stats.Id,
                UsuarioId = stats.UsuarioId,
                FechaCorte = stats.FechaCorte,
                TotalEntrevistas = stats.TotalEntrevistas,
                TiempoTotalMin = stats.TiempoTotalMin,
                ScorePromedio = stats.ScorePromedio
            });
        }



        [HttpPost]
        public async Task<ActionResult<int>> Crear([FromBody] StatsUsuarioCreateDto dto)
        {
            var stats = new Stats_Usuario
            {
                UsuarioId = dto.UsuarioId,
                FechaCorte = dto.FechaCorte,
                TotalEntrevistas = dto.TotalEntrevistas,
                TiempoTotalMin = dto.TiempoTotalMin,
                ScorePromedio = dto.ScorePromedio
            };

            var id = await _service.CrearAsync(stats);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] StatsUsuarioUpdateDto dto)
        {
            var existente = await _service.GetByIdAsync(id);
            if (existente == null) return NotFound();

            if (dto.UsuarioId.HasValue) existente.UsuarioId = dto.UsuarioId.Value;
            if (dto.FechaCorte.HasValue) existente.FechaCorte = dto.FechaCorte.Value;
            if (dto.TotalEntrevistas.HasValue) existente.TotalEntrevistas = dto.TotalEntrevistas.Value;
            if (dto.TiempoTotalMin.HasValue) existente.TiempoTotalMin = dto.TiempoTotalMin.Value;
            if (dto.ScorePromedio.HasValue) existente.ScorePromedio = dto.ScorePromedio.Value;

            await _service.ActualizarAsync(id, existente);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var stats = await _service.GetByIdAsync(id);
            if (stats == null) return NotFound();

            await _service.EliminarAsync(id);
            return NoContent();
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<StatsUsuarioReadDto>>> GetByUsuarioId(int usuarioId)
        {
            var stats = await _service.GetByUsuarioIdAsync(usuarioId);
            if (stats == null || !stats.Any()) return NotFound();

            var result = stats.Select(s => new StatsUsuarioReadDto
            {
                Id = s.Id,
                UsuarioId = s.UsuarioId,
                FechaCorte = s.FechaCorte,
                TotalEntrevistas = s.TotalEntrevistas,
                TiempoTotalMin = s.TiempoTotalMin,
                ScorePromedio = s.ScorePromedio
            });

            return Ok(result);
        }



        [HttpGet("resumen")]
        public async Task<ActionResult<StatsUsuarioResumenDto>> GetResumen(
            [FromQuery] int usuarioId,
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            var resumen = await _service.GetResumenPorUsuarioAsync(usuarioId, fechaInicio, fechaFin);
            if (resumen == null)
                return NotFound(new { mensaje = "No se encontraron entrevistas en el rango indicado." });

            return Ok(resumen);
        }

    }
}
