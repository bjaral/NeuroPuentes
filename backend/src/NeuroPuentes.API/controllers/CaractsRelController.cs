using Microsoft.AspNetCore.Mvc;
using NeuroPuentesAPI.models;
using NeuroPuentesAPI.services;
using NeuroPuentesAPI.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CaractsRelController : ControllerBase
    {
        private readonly ICaractsRelService _service;

        public CaractsRelController(ICaractsRelService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CaractsRelReadDto>>> GetAll()
        {
            var relaciones = await _service.GetAllAsync();
            return Ok(relaciones.Select(r => new CaractsRelReadDto
            {
                CaracteristicaId = r.CaracteristicaId,
                ContextoId = r.ContextoId
            }));
        }

        [HttpGet("{caracteristicaId}/{contextoId}")]
        public async Task<ActionResult<CaractsRelReadDto>> GetByIds(int caracteristicaId, int contextoId)
        {
            var rel = await _service.GetByIdsAsync(caracteristicaId, contextoId);
            if (rel == null) return NotFound();

            return Ok(new CaractsRelReadDto
            {
                CaracteristicaId = rel.CaracteristicaId,
                ContextoId = rel.ContextoId
            });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CaractsRelCreateDto dto)
        {
            var rel = new Caracts_Rel
            {
                CaracteristicaId = dto.CaracteristicaId,
                ContextoId = dto.ContextoId
            };

            await _service.CrearAsync(rel);
            return CreatedAtAction(nameof(GetByIds), new { caracteristicaId = dto.CaracteristicaId, contextoId = dto.ContextoId }, dto);
        }

        [HttpDelete("{caracteristicaId}/{contextoId}")]
        public async Task<IActionResult> Eliminar(int caracteristicaId, int contextoId)
        {
            var rel = await _service.GetByIdsAsync(caracteristicaId, contextoId);
            if (rel == null) return NotFound();

            await _service.EliminarAsync(caracteristicaId, contextoId);
            return NoContent();
        }
    }
}
