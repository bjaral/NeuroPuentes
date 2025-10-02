using Microsoft.AspNetCore.Mvc;
using NeuroPuentesAPI.models;
using NeuroPuentesAPI.services;
using NeuroPuentesAPI.DTOs;

namespace NeuroPuentesAPI.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CaracteristicasController : ControllerBase
    {
        private readonly ICaracteristicaService _service;

        public CaracteristicasController(ICaracteristicaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CaracteristicaReadDto>>> GetAll()
        {
            var items = await _service.GetAllAsync();
            var result = items.Select(c => new CaracteristicaReadDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                Grupo = c.Grupo,
                Vigencia = c.Vigencia
            });
            return Ok(result);
        }

        [HttpGet("Vigentes")]
        public async Task<ActionResult<IEnumerable<CaracteristicaReadDto>>> GetAllVigentes()
        {
            var items = await _service.GetAllVigentesAsync();
            var result = items.Select(c => new CaracteristicaReadDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                Grupo = c.Grupo,
                Vigencia = c.Vigencia
            });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CaracteristicaReadDto>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();

            return Ok(new CaracteristicaReadDto
            {
                Id = item.Id,
                Nombre = item.Nombre,
                Descripcion = item.Descripcion,
                Grupo = item.Grupo,
                Vigencia = item.Vigencia
            });
        }

        [HttpPost]
        public async Task<ActionResult<int>> Crear([FromBody] CaracteristicaCreateDto dto)
        {
            var entity = new Caracteristica
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Grupo = dto.Grupo,
                Vigencia = dto.Vigencia
            };

            var id = await _service.CrearAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] CaracteristicaUpdateDto dto)
        {
            var existente = await _service.GetByIdAsync(id);
            if (existente == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.Nombre)) existente.Nombre = dto.Nombre;
            if (!string.IsNullOrWhiteSpace(dto.Descripcion)) existente.Descripcion = dto.Descripcion;
            if (!string.IsNullOrWhiteSpace(dto.Grupo)) existente.Grupo = dto.Grupo;
            if (dto.Vigencia.HasValue) existente.Vigencia = dto.Vigencia.Value;

            await _service.ActualizarAsync(id, existente);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();

            await _service.EliminarAsync(id);
            return NoContent();
        }
    }
}
