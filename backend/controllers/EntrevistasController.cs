using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
    public class EntrevistasController : ControllerBase
    {
        private readonly IEntrevistaService _service;
        private readonly IDialogoService _dialogoService; 
        private readonly IaApiService _iaService;
        private readonly ILogger<EntrevistasController> _logger;
        private readonly IWebHostEnvironment _env;

        public EntrevistasController(
            IEntrevistaService service,
            IDialogoService dialogoService, 
            IaApiService iaService, 
            ILogger<EntrevistasController> logger,
            IWebHostEnvironment env)
        {
            _service = service;
            _dialogoService = dialogoService;
            _iaService = iaService;
            _logger = logger;
            _env = env;
        }

        [HttpPost("iniciar-con-ia")]
        [ProducesResponseType(typeof(IAResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> IniciarEntrevistaConIA(
            [FromForm] EntrevistaIaCreateDto dto
        )
        {
            if (dto.Audio == null || dto.Audio.Length == 0)
            {
                return BadRequest("No se proporcionó archivo de audio.");
            }

            _logger.LogInformation("Iniciando nueva entrevista con IA para Usuario {UsuarioId}", dto.UsuarioId);

            try
            {
                // 1. Llamar al servicio de IA
                IAResponse resultadoIA = await _iaService.ProcessAudioAsync(dto.Audio, dto.ContextTraits, null);

                if (!resultadoIA.Success)
                {
                    _logger.LogWarning("La llamada al servicio de IA falló: {Error}", resultadoIA.Error);
                    return StatusCode(502, resultadoIA.Error); 
                }

                // 2. Crear la Entrevista en la BD
                var entrevistaDto = new EntrevistaCreateDto
                {
                    UsuarioId = dto.UsuarioId,
                    ContextoId = dto.ContextoId,
                    Titulo = dto.Titulo,
                    Descripcion = "Entrevista iniciada con IA",
                    NumeroTurnos = 2, 
                    ContextoSnapshot = dto.ContextTraits 
                };
                
                var nuevaEntrevista = await _service.CrearAsync(entrevistaDto); 
                _logger.LogInformation("Entrevista {EntrevistaId} creada.", nuevaEntrevista.Id);

                // 3. Guardar los dos primeros turnos de diálogo
                
                // 3a. Guardar el audio del estudiante
                string audioEstudianteUrl = await GuardarArchivo(dto.Audio);
                var dialogoEstudiante = new DialogoCreateDto
                {
                    EntrevistaId = nuevaEntrevista.Id,
                    Turno = 1,
                    Sender = "Estudiante",
                    Texto = resultadoIA.Transcription, 
                    AudioUrl = audioEstudianteUrl
                };
                await _dialogoService.CrearAsync(dialogoEstudiante);

                // 3b. Guardar el audio de la IA
                string audioIaUrl = await GuardarAudioBase64(resultadoIA.AudioBase64);
                var dialogoIA = new DialogoCreateDto
                {
                    EntrevistaId = nuevaEntrevista.Id,
                    Turno = 2,
                    Sender = "IA",
                    Texto = resultadoIA.ResponseText, 
                    AudioUrl = audioIaUrl
                };
                await _dialogoService.CrearAsync(dialogoIA);
                
                _logger.LogInformation("Diálogos iniciales guardados para Entrevista {EntrevistaId}", nuevaEntrevista.Id);

                // 4. Devolver la respuesta al frontend
                return Ok(resultadoIA);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar entrevista con IA.");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
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
            var nuevaEntrevista = await _service.CrearAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = nuevaEntrevista.Id }, nuevaEntrevista);
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

        [HttpGet("estudiante/{estudianteId:int}")]
        public async Task<IActionResult> GetByEstudianteId(int estudianteId)
        {
            var entrevistas = await _service.GetByEstudianteIdAsync(estudianteId);
            return Ok(entrevistas);
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