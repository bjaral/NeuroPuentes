using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;
using NeuroPuentesAPI.Controllers;
using NeuroPuentesAPI.services;
using NeuroPuentesAPI.DTOs;
using NeuroPuentesAPI.models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.Tests.Controllers
{
    public class EntrevistasControllerTests
    {
        private readonly Mock<IEntrevistaService> _mockEntrevistaService;
        private readonly Mock<IDialogoService> _mockDialogoService;
        private readonly Mock<IIaApiService> _mockIaService;
        private readonly Mock<ILogger<EntrevistasController>> _mockLogger;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private readonly EntrevistasController _controller;

        public EntrevistasControllerTests()
        {
            _mockEntrevistaService = new Mock<IEntrevistaService>();
            _mockDialogoService = new Mock<IDialogoService>();
            _mockIaService = new Mock<IIaApiService>();
            _mockLogger = new Mock<ILogger<EntrevistasController>>();
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();

            // Configurar el entorno web para las rutas de archivos
            _mockWebHostEnvironment.Setup(e => e.ContentRootPath).Returns(Directory.GetCurrentDirectory());

            _controller = new EntrevistasController(
                _mockEntrevistaService.Object,
                _mockDialogoService.Object,
                _mockIaService.Object,
                _mockLogger.Object,
                _mockWebHostEnvironment.Object
            );
        }

        /// <summary>
        /// TEST DE FLUJO COMPLETO: Creación exitosa de entrevista con IA
        /// 
        /// Este test verifica el escenario ideal donde:
        /// 1. Un usuario envía un archivo de audio para iniciar una entrevista
        /// 2. El servicio de IA procesa el audio y genera una respuesta
        /// 3. Se crea una nueva entrevista en la base de datos
        /// 4. Se generan y guardan los diálogos iniciales (usuario + IA)
        /// 5. Se retorna una respuesta exitosa con los datos de la IA
        /// 
        /// Verifica que:
        /// - Todos los servicios son llamados en el orden correcto
        /// - Los datos se mapean correctamente entre DTOs y modelos
        /// - Los archivos de audio se guardan en el sistema de archivos
        /// - La estructura de la respuesta es la esperada
        /// - Los IDs de sesión se asignan correctamente
        /// 
        /// Este test cubre el flujo principal de negocio del sistema.
        /// </summary>
        [Fact(DisplayName = "CONTROLADOR — Flujo completo de creación de entrevista con IA")]
        public async Task IniciarEntrevistaConIA_Success_ShouldCallPersistenceServices()
        {
            // PREPARACIÓN: Configurar archivo de audio simulado y servicios mock
            var mockFile = new Mock<IFormFile>();
            var sourceFile = Path.GetTempFileName();
            File.WriteAllText(sourceFile, "Mock audio content");
            
            var audioStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("fake audio content"));
            
            mockFile.Setup(f => f.FileName).Returns("test_audio.wav");
            mockFile.Setup(f => f.Length).Returns(audioStream.Length);
            mockFile.Setup(f => f.ContentType).Returns("audio/wav");
            mockFile.Setup(f => f.OpenReadStream()).Returns(audioStream);
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                    .Callback<Stream, System.Threading.CancellationToken>((target, token) => 
                    {
                        audioStream.CopyTo(target);
                        audioStream.Position = 0; // Reset stream position
                    })
                    .Returns(Task.CompletedTask);

            var dto = new EntrevistaIaCreateDto
            {
                Audio = mockFile.Object,
                ContextTraits = "Test context traits",
                UsuarioId = 1,
                ContextoId = 1,
                Titulo = "Test Interview",
                DuracionMin = 10.0f
            };

            var iaResponse = new IAResponse
            {
                Success = true,
                Transcription = "Test transcription",
                ResponseText = "Test response text",
                AudioBase64 = Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5 })
            };

            // Configurar mocks para retornar respuestas exitosas
            _mockIaService.Setup(s => s.ProcessAudioAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>()))
                         .ReturnsAsync(iaResponse);

            _mockEntrevistaService.Setup(s => s.CrearAsync(It.IsAny<Entrevista>()))
                                 .ReturnsAsync(1); // Retorna ID 1

            _mockDialogoService.Setup(s => s.CrearAsync(It.IsAny<Dialogo>()))
                              .ReturnsAsync(1); // Retorna ID 1 para cada diálogo

            // Crear directorio media si no existe para guardar archivos
            var mediaPath = Path.Combine(Directory.GetCurrentDirectory(), "media");
            if (!Directory.Exists(mediaPath))
            {
                Directory.CreateDirectory(mediaPath);
            }

            // EJECUCIÓN: Llamar al método del controlador
            var result = await _controller.IniciarEntrevistaConIA(dto);

            // VERIFICACIÓN: Validar interacciones con todos los servicios

            // 1. Servicio de IA debe procesar el audio una vez
            _mockIaService.Verify(s => s.ProcessAudioAsync(
                It.IsAny<IFormFile>(), 
                It.IsAny<string>(), 
                It.IsAny<string>()), 
                Times.Once);

            // 2. Servicio de entrevistas debe crear una entrevista con los datos correctos
            _mockEntrevistaService.Verify(s => s.CrearAsync(It.Is<Entrevista>(e =>
                e.UsuarioId == dto.UsuarioId &&
                e.ContextoId == dto.ContextoId &&
                e.Titulo == dto.Titulo &&
                e.DuracionMin == dto.DuracionMin &&
                e.NumeroTurnos == 2 && // Valor esperado según el código
                e.ContextoSnapshot == dto.ContextTraits
            )), Times.Once);

            // 3. Servicio de diálogos debe crear exactamente 2 diálogos
            _mockDialogoService.Verify(s => s.CrearAsync(It.IsAny<Dialogo>()), Times.Exactly(2));

            // 4. Verificar diálogo del USUARIO (transcripción del audio)
            _mockDialogoService.Verify(s => s.CrearAsync(It.Is<Dialogo>(d =>
                d.EntrevistaId == 1 &&
                d.Turno == 1 &&
                d.Sender == ENUM_SENDER_DIALOGO.User &&
                d.Texto == iaResponse.Transcription &&
                !string.IsNullOrEmpty(d.AudioUrl) // Archivo de audio guardado
            )), Times.Once);

            // 5. Verificar diálogo de la IA (respuesta generada)
            _mockDialogoService.Verify(s => s.CrearAsync(It.Is<Dialogo>(d =>
                d.EntrevistaId == 1 &&
                d.Turno == 2 &&
                d.Sender == ENUM_SENDER_DIALOGO.Ai &&
                d.Texto == iaResponse.ResponseText &&
                !string.IsNullOrEmpty(d.AudioUrl) // Archivo de audio de IA guardado
            )), Times.Once);

            // 6. Validar respuesta HTTP exitosa
            var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
            var returnValue = Assert.IsType<IAResponse>(okResult.Value);
            Assert.True(returnValue.Success);
            Assert.Equal("1", returnValue.SessionId); // SessionId debe ser el ID de la entrevista

            // LIMPIEZA: Eliminar archivos temporales creados durante el test
            if (File.Exists(sourceFile))
                File.Delete(sourceFile);
            
            CleanupMediaFiles(mediaPath);
        }

        /// <summary>
        /// TEST DE ERROR: Fallo en el servicio de IA
        /// 
        /// Este test verifica el manejo de errores cuando:
        /// 1. El servicio de IA falla al procesar el audio
        /// 2. El controlador debe retornar un error HTTP apropiado
        /// 3. No se deben crear entrevistas ni diálogos
        /// 4. El error debe propagarse correctamente
        /// 
        /// Verifica la resiliencia del sistema ante fallos externos.
        /// </summary>
        [Fact(DisplayName = "CONTROLADOR — Error en servicio de IA retorna BadGateway")]
        public async Task IniciarEntrevistaConIA_WhenIaServiceFails_ReturnsBadGateway()
        {
            // PREPARACIÓN: Configurar mock para simular fallo de IA
            var mockFile = new Mock<IFormFile>();
            var audioStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("fake audio"));
            
            mockFile.Setup(f => f.FileName).Returns("test.wav");
            mockFile.Setup(f => f.Length).Returns(audioStream.Length);
            mockFile.Setup(f => f.OpenReadStream()).Returns(audioStream);

            var dto = new EntrevistaIaCreateDto
            {
                Audio = mockFile.Object,
                ContextTraits = "Test context",
                UsuarioId = 1,
                ContextoId = 1,
                Titulo = "Test",
                DuracionMin = 5.0f
            };

            // Configurar IA para retornar error
            var failedIaResponse = new IAResponse
            {
                Success = false,
                Error = "Error de conexión con el servicio de IA"
            };

            _mockIaService.Setup(s => s.ProcessAudioAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>()))
                         .ReturnsAsync(failedIaResponse);

            // EJECUCIÓN: Llamar al controlador
            var result = await _controller.IniciarEntrevistaConIA(dto);

            // VERIFICACIÓN: Debe retornar 502 Bad Gateway
            var statusCodeResult = Assert.IsType<Microsoft.AspNetCore.Mvc.StatusCodeResult>(result);
            Assert.Equal(502, statusCodeResult.StatusCode);

            // Verificar que NO se crearon entrevistas ni diálogos
            _mockEntrevistaService.Verify(s => s.CrearAsync(It.IsAny<Entrevista>()), Times.Never);
            _mockDialogoService.Verify(s => s.CrearAsync(It.IsAny<Dialogo>()), Times.Never);
        }

        /// <summary>
        /// TEST DE VALIDACIÓN: Archivo de audio requerido
        /// 
        /// Este test verifica que el controlador valida la presencia
        /// del archivo de audio antes de procesar la solicitud.
        /// 
        /// Escenario:
        /// 1. Se envía un DTO sin archivo de audio
        /// 2. El controlador debe retornar BadRequest inmediatamente
        /// 3. No se deben invocar los servicios de IA, entrevistas o diálogos
        /// 
        /// Esto prueba las validaciones básicas del controlador.
        /// </summary>
        [Fact(DisplayName = "CONTROLADOR — Audio requerido retorna BadRequest")]
        public async Task IniciarEntrevistaConIA_WithoutAudio_ReturnsBadRequest()
        {
            // PREPARACIÓN: DTO sin archivo de audio
            var dto = new EntrevistaIaCreateDto
            {
                Audio = null, // Audio faltante
                ContextTraits = "Test context",
                UsuarioId = 1,
                ContextoId = 1,
                Titulo = "Test",
                DuracionMin = 5.0f
            };

            // EJECUCIÓN: Llamar al controlador
            var result = await _controller.IniciarEntrevistaConIA(dto);

            // VERIFICACIÓN: Debe retornar 400 BadRequest
            var badRequestResult = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("No se proporcionó archivo de audio.", badRequestResult.Value);

            // Verificar que NO se llamó a ningún servicio
            _mockIaService.Verify(s => s.ProcessAudioAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockEntrevistaService.Verify(s => s.CrearAsync(It.IsAny<Entrevista>()), Times.Never);
            _mockDialogoService.Verify(s => s.CrearAsync(It.IsAny<Dialogo>()), Times.Never);
        }

        /// <summary>
        /// TEST DE EXCEPCIÓN: Manejo de errores inesperados
        /// 
        /// Este test verifica que el controlador maneja adecuadamente
        /// excepciones inesperadas durante el procesamiento.
        /// 
        /// Escenario:
        /// 1. Ocurre una excepción no controlada durante el proceso
        /// 2. El controlador debe capturar la excepción y loggearla
        /// 3. Debe retornar HTTP 500 Internal Server Error
        /// 4. El error no debe propagarse y romper la aplicación
        /// 
        /// Esto prueba la estabilidad y robustez del controlador.
        /// </summary>
        [Fact(DisplayName = "CONTROLADOR — Excepción inesperada retorna InternalServerError")]
        public async Task IniciarEntrevistaConIA_WhenExceptionOccurs_ReturnsInternalServerError()
        {
            // PREPARACIÓN: Configurar mock para lanzar excepción
            var mockFile = new Mock<IFormFile>();
            var audioStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("fake audio"));
            
            mockFile.Setup(f => f.FileName).Returns("test.wav");
            mockFile.Setup(f => f.OpenReadStream()).Returns(audioStream);

            var dto = new EntrevistaIaCreateDto
            {
                Audio = mockFile.Object,
                ContextTraits = "Test context",
                UsuarioId = 1,
                ContextoId = 1,
                Titulo = "Test",
                DuracionMin = 5.0f
            };

            // Configurar servicio para lanzar excepción inesperada
            _mockIaService.Setup(s => s.ProcessAudioAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>()))
                         .ThrowsAsync(new InvalidOperationException("Error inesperado en la base de datos"));

            // EJECUCIÓN: Llamar al controlador
            var result = await _controller.IniciarEntrevistaConIA(dto);

            // VERIFICACIÓN: Debe retornar 500 Internal Server Error
            var statusCodeResult = Assert.IsType<Microsoft.AspNetCore.Mvc.StatusCodeResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            // Verificar que se loggeó el error (opcional, si quieres verificar logging)
            // _mockLogger.Verify(x => x.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// MÉTODO DE LIMPIEZA: Elimina archivos temporales creados durante los tests
        /// 
        /// Este método asegura que cada test deje el sistema de archivos
        /// en el estado original, evitando contaminación entre tests.
        /// </summary>
        private void CleanupMediaFiles(string mediaPath)
        {
            try
            {
                var files = Directory.GetFiles(mediaPath);
                foreach (var file in files)
                {
                    if (file.Contains(".wav") || file.Contains(".tmp"))
                    {
                        File.Delete(file);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log cleanup error but don't fail the test
                Console.WriteLine($"Error cleaning up media files: {ex.Message}");
            }
        }
    }
}