using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NeuroPuentesAPI.services;
using NeuroPuentesAPI.DTOs;
using NeuroPuentesAPI.models;
using NeuroPuentesAPI.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace NeuroPuentes.Tests.fixtures
{
    public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        public Mock<IIaApiService> MockIaService { get; } = new Mock<IIaApiService>();
        public Mock<IEntrevistaService> MockEntrevistaService { get; } = new Mock<IEntrevistaService>();
        public Mock<IDialogoService> MockDialogoService { get; } = new Mock<IDialogoService>();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // ===== REMOVER Y REEMPLAZAR DBCONTEXT CON IN-MEMORY =====
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                
                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                // Agregar DbContext en memoria para pruebas
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });

                // ===== MOCK SERVICIO IA =====
                var iaServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IIaApiService));
                
                if (iaServiceDescriptor != null)
                {
                    services.Remove(iaServiceDescriptor);
                }

                // En TestWebApplicationFactory.cs - actualizar el mock del servicio IA
                MockIaService.Setup(s => s.ProcessAudioAsync(
                    It.IsAny<IFormFile>(), 
                    It.IsAny<string>(), 
                    It.IsAny<string>()))
                    .ReturnsAsync((IFormFile audio, string contextTraits, string sessionId) => new IAResponse
                    {
                        Success = true,
                        Transcription = "Transcripción de prueba del usuario",
                        ResponseText = "Respuesta simulada de la IA",
                        AudioBase64 = Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5 }),
                        SessionId = sessionId // Usar el sessionId que viene del controlador
                    });

                services.AddSingleton(MockIaService.Object);

                // ===== MOCK SERVICIO ENTREVISTA =====
                var entrevistaServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IEntrevistaService));
                
                if (entrevistaServiceDescriptor != null)
                {
                    services.Remove(entrevistaServiceDescriptor);
                }

                MockEntrevistaService.Setup(s => s.CrearAsync(It.IsAny<Entrevista>()))
                                   .ReturnsAsync(1); // Siempre retorna ID 1

                MockEntrevistaService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                                   .ReturnsAsync((int id) => new Entrevista 
                                   { 
                                       Id = id, 
                                       UsuarioId = 1, 
                                       ContextoId = 1,
                                       Titulo = "Entrevista Test",
                                       Descripcion = "Descripción test",
                                       DuracionMin = 10.0f,
                                       NumeroTurnos = 2,
                                       FechaCreacion = DateTime.UtcNow,
                                       ContextoSnapshot = "{}"
                                   });

                services.AddSingleton(MockEntrevistaService.Object);

                // ===== MOCK SERVICIO DIÁLOGO =====
                var dialogoServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IDialogoService));
                
                if (dialogoServiceDescriptor != null)
                {
                    services.Remove(dialogoServiceDescriptor);
                }

                MockDialogoService.Setup(s => s.CrearAsync(It.IsAny<Dialogo>()))
                                .ReturnsAsync(1); // Siempre retorna ID 1

                services.AddSingleton(MockDialogoService.Object);
            });

            builder.UseEnvironment("Development");
        }
    }
}