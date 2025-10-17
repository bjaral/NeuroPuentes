using Microsoft.AspNetCore.Mvc;
using NeuroPuentesAPI.DTOs;
using NeuroPuentesAPI.services;

namespace NeuroPuentesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Eval_EntrevistaController : ControllerBase
{
    private readonly IEval_EntrevistaService _service;

    public Eval_EntrevistaController(IEval_EntrevistaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var eval = await _service.GetByIdAsync(id);
        if (eval == null) return NotFound();
        return Ok(eval);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Eval_EntrevistaCreateDto dto)
    {
        await _service.CrearAsync(dto);
        return Ok();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] Eval_EntrevistaCreateDto dto)
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
    [HttpGet("entrevista/{entrevistaId:int}")]
    public async Task<IActionResult> GetByEntrevistaId(int entrevistaId)
    {
        var eval = await _service.GetByEntrevistaIdAsync(entrevistaId);
        if (eval == null) return NotFound();
        return Ok(eval);
    }
}
