using Microsoft.AspNetCore.Mvc;
using NeuroPuentesAPI.DTOs;
using NeuroPuentesAPI.services;
using NeuroPuentesAPI.models;
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

        // --- ENDPOINT DE IA ---
        [HttpPost("continuar-con-ia")]
        [ProducesResponseType(typeof(IAResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> ContinuarDialogoConIA(
            [FromForm] DialogoIaCreateDto dto 
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
                
                resultadoIA.SessionId = sessionId; 

                // --- Lógica ADAPTADA a 'develop' ---
                // 2. Guardar el turno del estudiante (Mapeando a Modelo)
                string audioEstudianteUrl = await GuardarArchivo(dto.Audio);
                var dialogoEstudiante = new Dialogo
                {
                    EntrevistaId = dto.EntrevistaId,
                    Turno = dto.Turno,
                    Sender = dto.Sender,
                    Texto = resultadoIA.Transcription, 
                    AudioUrl = audioEstudianteUrl
                };
                await _service.CrearAsync(dialogoEstudiante);

                // 3. Guardar el turno de la IA (Mapeando a Modelo)
                string audioIaUrl = await GuardarAudioBase64(resultadoIA.AudioBase64);
                var dialogoIA = new Dialogo
                {
                    EntrevistaId = dto.EntrevistaId,
                    Turno = dto.Turno + 1,
                    Sender = "IA",
                    Texto = resultadoIA.ResponseText, 
                    AudioUrl = audioIaUrl
                };
                await _service.CrearAsync(dialogoIA);

                _logger.LogInformation("Diálogos (Turnos {Turno}, {TurnoIA}) guardados para Entrevista {EntrevistaId}", dto.Turno, dto.Turno + 1, dto.EntrevistaId);
                
                return Ok(resultadoIA);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al continuar diálogo con IA.");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
        

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DialogoDto>>> GetAll() 
        {
            var dialogos = await _service.GetAllAsync();
     
            var result = dialogos.Select(d => MapToDto(d));
            return Ok(result);
        }

        [HttpGet("entrevista/{entrevistaId:int}")]
        public async Task<ActionResult<IEnumerable<DialogoDto>>> GetByEntrevistaId(int entrevistaId)
        {
            var dialogos = await _service.GetByEntrevistaIdAsync(entrevistaId);
         
            var result = dialogos.Select(d => MapToDto(d));
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DialogoDto>> GetById(int id)
        {
            var d = await _service.GetByIdAsync(id);
            if (d is null) return NotFound(); 
            return Ok(MapToDto(d)); 
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] DialogoCreateDto dto)
        {
            
            var dialogo = new Dialogo
            {
                EntrevistaId = dto.EntrevistaId,
                Turno = dto.Turno,
                Sender = dto.Sender,
                Texto = dto.Texto,
                TextoProcesado = dto.TextoProcesado,
                AudioUrl = dto.AudioUrl
            };

            var id = await _service.CrearAsync(dialogo);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] DialogoUpdateDto dto)
        {
             var dialogo = new Dialogo
            {
                Turno = dto.Turno ?? 0, 
                Sender = dto.Sender,
                Texto = dto.Texto,
                TextoProcesado = dto.TextoProcesado,
                AudioUrl = dto.AudioUrl
            };

            await _service.ActualizarAsync(id, dialogo);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.EliminarAsync(id); 
            return NoContent();
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
        private DialogoDto MapToDto(Dialogo d)
        {
            return new DialogoDto
            {
                Id = d.Id,
                EntrevistaId = d.EntrevistaId,
                Turno = d.Turno,
                Sender = d.Sender, /
                Texto = d.Texto,
                TextoProcesado = d.TextoProcesado,
                Timestamp = d.Timestamp,
                AudioUrl = d.AudioUrl
            };
        }
    }
}