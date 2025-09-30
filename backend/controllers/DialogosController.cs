using Microsoft.AspNetCore.Mvc;
using NeuroPuentesAPI.DTOs;
using NeuroPuentesAPI.services;

namespace NeuroPuentesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DialogosController : ControllerBase
{
    private readonly IDialogoService _service;

    public DialogosController(IDialogoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dialogo = await _service.GetByIdAsync(id);
        if (dialogo == null) return NotFound();
        return Ok(dialogo);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] DialogoCreateDto dto)
    {
        await _service.CrearAsync(dto);
        return Ok();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] DialogoCreateDto dto)
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
