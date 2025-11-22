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

        // -----------------------------------------------------------
        // TEST
        // IniciarEntrevistaConIA - Éxito
        // Debe llamar a IAService, guardar entrevista y diálogos,
        // y retornar 200 OK con la respuesta de IA.
        // -----------------------------------------------------------
        [Fact]
        public async Task IniciarEntrevistaConIA_Success_ShouldCallPersistenceServices()
        {
            // Arrange
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

            _mockIaService.Setup(s => s.ProcessAudioAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>()))
                         .ReturnsAsync(iaResponse);

            _mockEntrevistaService.Setup(s => s.CrearAsync(It.IsAny<Entrevista>()))
                                 .ReturnsAsync(1); // Retorna ID 1

            _mockDialogoService.Setup(s => s.CrearAsync(It.IsAny<Dialogo>()))
                              .ReturnsAsync(1); // Retorna ID 1 para cada diálogo

            // Crear directorio media si no existe
            var mediaPath = Path.Combine(Directory.GetCurrentDirectory(), "media");
            if (!Directory.Exists(mediaPath))
            {
                Directory.CreateDirectory(mediaPath);
            }

            // Act
            var result = await _controller.IniciarEntrevistaConIA(dto);

            // Assert
            // Verificar que se llamó al servicio de IA
            _mockIaService.Verify(s => s.ProcessAudioAsync(
                It.IsAny<IFormFile>(), 
                It.IsAny<string>(), 
                It.IsAny<string>()), 
                Times.Once);

            // Verificar que se llamó al servicio de entrevista para crear
            _mockEntrevistaService.Verify(s => s.CrearAsync(It.Is<Entrevista>(e =>
                e.UsuarioId == dto.UsuarioId &&
                e.ContextoId == dto.ContextoId &&
                e.Titulo == dto.Titulo &&
                e.DuracionMin == dto.DuracionMin &&
                e.NumeroTurnos == 2 && // Valor esperado según el código
                e.ContextoSnapshot == dto.ContextTraits
            )), Times.Once);

            // Verificar que se llamó al servicio de diálogo DOS veces
            _mockDialogoService.Verify(s => s.CrearAsync(It.IsAny<Dialogo>()), Times.Exactly(2));

            // Verificaciones más específicas para cada diálogo
            _mockDialogoService.Verify(s => s.CrearAsync(It.Is<Dialogo>(d =>
                d.EntrevistaId == 1 &&
                d.Turno == 1 &&
                d.Sender == ENUM_SENDER_DIALOGO.User &&
                d.Texto == iaResponse.Transcription &&
                !string.IsNullOrEmpty(d.AudioUrl)
            )), Times.Once);

            _mockDialogoService.Verify(s => s.CrearAsync(It.Is<Dialogo>(d =>
                d.EntrevistaId == 1 &&
                d.Turno == 2 &&
                d.Sender == ENUM_SENDER_DIALOGO.Ai &&
                d.Texto == iaResponse.ResponseText &&
                !string.IsNullOrEmpty(d.AudioUrl)
            )), Times.Once);

            // Verificar que el resultado es exitoso
            var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
            var returnValue = Assert.IsType<IAResponse>(okResult.Value);
            Assert.True(returnValue.Success);
            Assert.Equal("1", returnValue.SessionId);

            // Cleanup
            if (File.Exists(sourceFile))
                File.Delete(sourceFile);
            
            // Limpiar archivos creados en media durante el test
            CleanupMediaFiles(mediaPath);
        }

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