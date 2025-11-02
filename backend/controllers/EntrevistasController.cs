using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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

        // --- ENDPOINT DE IA  ---
        [HttpPost("iniciar-con-ia")]
        [ProducesResponseType(typeof(IAResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
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
                IAResponse resultadoIA = await _iaService.ProcessAudioAsync(dto.Audio, dto.ContextTraits, null);
                if (!resultadoIA.Success)
                {
                    _logger.LogWarning("La llamada al servicio de IA falló: {Error}", resultadoIA.Error);
                    return StatusCode(502, resultadoIA.Error); 
                }


                var entrevista = new Entrevista
                {
                    UsuarioId = dto.UsuarioId,
                    ContextoId = dto.ContextoId,
                    Titulo = dto.Titulo,
                    Descripcion = "Entrevista iniciada con IA", 
                    NumeroTurnos = 2, 
                    ContextoSnapshot = dto.ContextTraits,
                    DuracionMin = dto.DuracionMin
                };
               
                int nuevaEntrevistaId = await _service.CrearAsync(entrevista); 
                _logger.LogInformation("Entrevista {EntrevistaId} creada.", nuevaEntrevistaId);

                string audioEstudianteUrl = await GuardarArchivo(dto.Audio);
                var dialogoEstudiante = new Dialogo
                {
                    EntrevistaId = nuevaEntrevistaId,
                    Turno = 1,
                    Sender = "Estudiante", 
                    Texto = resultadoIA.Transcription, 
                    AudioUrl = audioEstudianteUrl
                };
                await _dialogoService.CrearAsync(dialogoEstudiante);

                string audioIaUrl = await GuardarAudioBase64(resultadoIA.AudioBase64);
                var dialogoIA = new Dialogo
                {
                    EntrevistaId = nuevaEntrevistaId,
                    Turno = 2,
                    Sender = "IA",
                    Texto = resultadoIA.ResponseText, 
                    AudioUrl = audioIaUrl
                };
                await _dialogoService.CrearAsync(dialogoIA);
                
                _logger.LogInformation("Diálogos iniciales guardados para Entrevista {EntrevistaId}", nuevaEntrevistaId);
                
                resultadoIA.SessionId = nuevaEntrevistaId.ToString(); 
                return Ok(resultadoIA);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar entrevista con IA.");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

      
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntrevistaReadDto>>> GetAll()
        {
            var entrevistas = await _service.GetAllAsync();
            var result = entrevistas.Select(e => new EntrevistaReadDto
            {
                Id = e.Id,
                UsuarioId = e.UsuarioId,
                ContextoId = e.ContextoId,
                Titulo = e.Titulo,
                Descripcion = e.Descripcion,
                DuracionMin = e.DuracionMin,
                NumeroTurnos = e.NumeroTurnos,
                FechaCreacion = e.FechaCreacion,
                FechaCierre = e.FechaCierre,
                ContextoSnapshot = e.ContextoSnapshot
            });
            return Ok(result);
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<EntrevistaReadDto>>> GetByUsuarioId(int usuarioId)
        {
            var entrevistas = await _service.GetByUsuarioIdAsync(usuarioId);
            var result = entrevistas.Select(e => new EntrevistaReadDto
            {
                Id = e.Id,
                UsuarioId = e.UsuarioId,
                ContextoId = e.ContextoId,
                Titulo = e.Titulo,
                Descripcion = e.Descripcion,
                DuracionMin = e.DuracionMin,
                NumeroTurnos = e.NumeroTurnos,
                FechaCreacion = e.FechaCreacion,
                FechaCierre = e.FechaCierre,
                ContextoSnapshot = e.ContextoSnapshot
            });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EntrevistaReadDto>> GetById(int id)
        {
            var entrevista = await _service.GetByIdAsync(id);
            if (entrevista is null) return NotFound(); // Arreglo CS0019

            return Ok(new EntrevistaReadDto
            {
                Id = entrevista.Id,
                UsuarioId = entrevista.UsuarioId,
                ContextoId = entrevista.ContextoId,
                Titulo = entrevista.Titulo,
                Descripcion = entrevista.Descripcion,
                DuracionMin = entrevista.DuracionMin,
                NumeroTurnos = entrevista.NumeroTurnos,
                FechaCreacion = entrevista.FechaCreacion,
                FechaCierre = entrevista.FechaCierre,
                ContextoSnapshot = entrevista.ContextoSnapshot
            });
        }

        [HttpPost]
        public async Task<ActionResult<int>> Crear([FromBody] EntrevistaCreateDto dto)
        {
            var entrevista = new Entrevista
            {
                UsuarioId = dto.UsuarioId,
                ContextoId = dto.ContextoId,
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                DuracionMin = dto.DuracionMin,
                NumeroTurnos = dto.NumeroTurnos,
                ContextoSnapshot = dto.ContextoSnapshot
            };

            var id = await _service.CrearAsync(entrevista);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] EntrevistaUpdateDto dto)
        {
            var entrevista = new Entrevista
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                DuracionMin = dto.DuracionMin ?? 0, 
                NumeroTurnos = dto.NumeroTurnos ?? 0,
                FechaCierre = dto.FechaCierre,
                ContextoSnapshot = dto.ContextoSnapshot
            };

            await _service.ActualizarAsync(id, entrevista);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
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
    }
}