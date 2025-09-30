using Microsoft.AspNetCore.Mvc;
using NeuroPuentesAPI.DTOs;
using NeuroPuentesAPI.services;

namespace NeuroPuentesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Feedback_EntrevistaController : ControllerBase
{
    private readonly IFeedback_EntrevistaService _service;

    public Feedback_EntrevistaController(IFeedback_EntrevistaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var f = await _service.GetByIdAsync(id);
        if (f == null) return NotFound();
        return Ok(f);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Feedback_EntrevistaCreateDto dto)
    {
        await _service.CrearAsync(dto);
        return Ok();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] Feedback_EntrevistaCreateDto dto)
    {
        await _service.ActualizarAsync(id, dto);
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.EliminarAsync(id);
        return NoContent();
    }
}
