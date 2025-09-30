using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NeuroPuentesAPI.DTOs;
using NeuroPuentesAPI.services;

namespace NeuroPuentesAPI.Controllers;

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
    public async Task<IActionResult> Get() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var entrevista = await _service.GetByIdAsync(id);
        if (entrevista == null) return NotFound();
        return Ok(entrevista);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] EntrevistaCreateDto dto)
    {
        await _service.CrearAsync(dto);
        return Ok();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] EntrevistaCreateDto dto)
    {
        await _service.ActualizarAsync(id, dto);
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.EliminarAsync(id);
        return NoContent();
    }
}
