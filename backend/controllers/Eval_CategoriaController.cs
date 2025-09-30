using Microsoft.AspNetCore.Mvc;
using NeuroPuentesAPI.DTOs;
using NeuroPuentesAPI.services;

namespace NeuroPuentesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Eval_CategoriaController : ControllerBase
{
    private readonly IEval_CategoriaService _service;

    public Eval_CategoriaController(IEval_CategoriaService service)
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
    public async Task<IActionResult> Post([FromBody] Eval_CategoriaCreateDto dto)
    {
        await _service.CrearAsync(dto);
        return Ok();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] Eval_CategoriaCreateDto dto)
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
