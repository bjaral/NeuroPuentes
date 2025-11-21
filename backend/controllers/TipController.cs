using Microsoft.AspNetCore.Mvc;
using NeuroPuentesAPI.models;
using NeuroPuentesAPI.services;
using NeuroPuentesAPI.DTOs;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipsController : ControllerBase
    {
        private readonly ITipService _service;

        public TipsController(ITipService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipReadDto>>> GetAll()
        {
            var tips = await _service.GetAllAsync();
            var result = tips.Select(t => new TipReadDto
            {
                Id = t.Id,
                UsuarioId = t.UsuarioId,
                EntrevistaId = t.EntrevistaId,
                Titulo = t.Titulo,
                Contenido = t.Contenido,
                Categoria = t.Categoria,
                Fecha = t.Fecha,
                Usado = t.Usado,
                Vigencia = t.Vigencia
            });
            return Ok(result);
        }
        [HttpGet("Vigentes")]
        public async Task<ActionResult<IEnumerable<TipReadDto>>> GetAllVigentes()
        {
            var tips = await _service.GetAllVigentesAsync();
            var result = tips.Select(t => new TipReadDto
            {
                Id = t.Id,
                UsuarioId = t.UsuarioId,
                EntrevistaId = t.EntrevistaId,
                Titulo = t.Titulo,
                Contenido = t.Contenido,
                Categoria = t.Categoria,
                Fecha = t.Fecha,
                Usado = t.Usado,
                Vigencia = t.Vigencia
            });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TipReadDto>> GetById(int id)
        {
            var tip = await _service.GetByIdAsync(id);
            if (tip == null) return NotFound();

            return Ok(new TipReadDto
            {
                Id = tip.Id,
                UsuarioId = tip.UsuarioId,
                EntrevistaId = tip.EntrevistaId,
                Titulo = tip.Titulo,
                Contenido = tip.Contenido,
                Categoria = tip.Categoria,
                Fecha = tip.Fecha,
                Usado = tip.Usado,
                Vigencia = tip.Vigencia
            });
        }

        [HttpPost]
        public async Task<ActionResult<int>> Crear([FromBody] TipCreateDto dto)
        {
            var tip = new Tip
            {
                UsuarioId = dto.UsuarioId,
                EntrevistaId = dto.EntrevistaId,
                Titulo = dto.Titulo,
                Contenido = dto.Contenido,
                Categoria = dto.Categoria,
                Fecha = DateTime.UtcNow,
                Usado = dto.Usado,
                Vigencia = dto.Vigencia
            };

            var id = await _service.CrearAsync(tip);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] TipUpdateDto dto)
        {
            var existente = await _service.GetByIdAsync(id);
            if (existente == null) return NotFound();

            existente.UsuarioId = dto.UsuarioId ?? existente.UsuarioId;
            existente.EntrevistaId = dto.EntrevistaId ?? existente.EntrevistaId;
            existente.Titulo = dto.Titulo ?? existente.Titulo;
            existente.Contenido = dto.Contenido ?? existente.Contenido;
            existente.Categoria = dto.Categoria ?? existente.Categoria;
            existente.Usado = dto.Usado ?? existente.Usado;
            existente.Vigencia = dto.Vigencia ?? existente.Vigencia;

            await _service.ActualizarAsync(id, existente);
            return NoContent();
        }
        [HttpGet("estudiante/{estudianteId:int}")]
        public async Task<ActionResult<IEnumerable<TipReadDto>>> GetByEstudianteId(int estudianteId)
        {
            var tips = await _service.GetByEstudianteIdAsync(estudianteId);
            return Ok(tips);
        }

        [HttpGet("entrevista/{entrevistaId:int}")]
        public async Task<ActionResult<IEnumerable<TipReadDto>>> GetByEntrevistaId(int entrevistaId)
        {
            var tips = await _service.GetByEntrevistaIdAsync(entrevistaId);
            return Ok(tips);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var tip = await _service.GetByIdAsync(id);
            if (tip == null) return NotFound();

            await _service.EliminarAsync(id);
            return NoContent();
        }
    }
}
