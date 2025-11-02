using Microsoft.AspNetCore.Mvc;
using NeuroPuentesAPI.DTOs;
using NeuroPuentesAPI.services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting; 

namespace NeuroPuentesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DialogosController : ControllerBase
    {
        private readonly IDialogoService _service;
        private readonly IaApiService _iaService; 
        private readonly ILogger<DialogosController> _logger;
        private readonly IWebHostEnvironment _env;

        public DialogosController(
            IDialogoService service,
            IaApiService iaService, 
            ILogger<DialogosController> logger,
            IWebHostEnvironment env)
        {
            _service = service;
            _iaService = iaService;
            _logger = logger;
            _env = env;
        }

        [HttpPost("continuar-con-ia")]
        [ProducesResponseType(typeof(IAResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> ContinuarDialogoConIA(
            [FromForm] DialogoIaCreateDto dto // <-- ¡Este es el cambio!
        )
        {
            _logger.LogInformation("Continuando diálogo para la entrevista {EntrevistaId}", dto.EntrevistaId);

            if (dto.Audio == null || dto.Audio.Length == 0)
            {
                return BadRequest("No se proporcionó archivo de audio.");
            }

            try
            {
                // 1. Llamar al servicio de IA
                string sessionId = dto.EntrevistaId.ToString(); 
                IAResponse resultadoIA = await _iaService.ProcessAudioAsync(dto.Audio, dto.ContextTraits, sessionId);

                if (!resultadoIA.Success)
                {
                    _logger.LogWarning("La llamada al servicio de IA falló: {Error}", resultadoIA.Error);
                    return StatusCode(502, resultadoIA.Error);
                }

                // 2. Guardar el turno del estudiante
                string audioEstudianteUrl = await GuardarArchivo(dto.Audio);
                var dialogoEstudiante = new DialogoCreateDto
                {
                    EntrevistaId = dto.EntrevistaId,
                    Turno = dto.Turno, // ej: 3
                    Sender = dto.Sender,
                    Texto = resultadoIA.Transcription, 
                    AudioUrl = audioEstudianteUrl
                };
                await _service.CrearAsync(dialogoEstudiante);

                // 3. Guardar el turno de la IA
                string audioIaUrl = await GuardarAudioBase64(resultadoIA.AudioBase64);
                var dialogoIA = new DialogoCreateDto
                {
                    EntrevistaId = dto.EntrevistaId,
                    Turno = dto.Turno + 1, // ej: 4
                    Sender = "IA",
                    Texto = resultadoIA.ResponseText, 
                    AudioUrl = audioIaUrl
                };
                await _service.CrearAsync(dialogoIA);

                _logger.LogInformation("Diálogos (Turnos {Turno}, {TurnoIA}) guardados para Entrevista {EntrevistaId}", dto.Turno, dto.Turno + 1, dto.EntrevistaId);
                
                // 4. Devolver la respuesta al frontend
                return Ok(resultadoIA);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al continuar diálogo con IA.");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
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
            return Ok(); // O cambia a CreatedAtAction si lo necesitas
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

        [HttpGet("entrevista/{entrevistaId:int}")]
        public async Task<IActionResult> GetByEntrevistaId(int entrevistaId)
        {
            var dialogos = await _service.GetByEntrevistaIdAsync(entrevistaId);
            return Ok(dialogos);
        }
        private async Task<string> GuardarArchivo(IFormFile file)
        {
            var mediaPath = Path.Combine(_env.ContentRootPath, "media");
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(mediaPath, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return $"/media/{fileName}";
        }

        private async Task<string> GuardarAudioBase64(string base64String)
        {
            var mediaPath = Path.Combine(_env.ContentRootPath, "media");
            var fileName = $"{Guid.NewGuid()}.wav"; 
            var filePath = Path.Combine(mediaPath, fileName);
            
            var bytes = Convert.FromBase64String(base64String);
            
            await System.IO.File.WriteAllBytesAsync(filePath, bytes);
            return $"/media/{fileName}";
        }
    }
}