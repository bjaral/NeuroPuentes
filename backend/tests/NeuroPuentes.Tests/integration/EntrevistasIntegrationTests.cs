using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NeuroPuentes.Tests.fixtures;
using NeuroPuentesAPI.DTOs;
using NeuroPuentesAPI.Data;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Net;
using Xunit;
using NeuroPuentesAPI.models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace NeuroPuentes.Tests.integration
{
    public class EntrevistasIntegrationTests : IClassFixture<TestWebApplicationFactory<Program>>, IDisposable
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _context;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IServiceScope _scope;

        public EntrevistasIntegrationTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            
            // Crear scope y obtener el DbContext para manipular la base de datos de prueba
            _scope = factory.Services.CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // Limpiar y crear una base de datos en memoria nueva para cada test
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // Agregar datos de prueba básicos que todos los tests necesitan
            SeedTestData();
        }

        private void SeedTestData()
        {
            // Agregar un usuario de prueba si no existe
            if (!_context.Usuarios.Any())
            {
                var usuario = new Usuario
                {
                    Id = 1,
                    NombreUsuario = "testuser",
                    Nombre = "Usuario Test",
                    Email = "test@neuropuentes.com",
                    PasswordHash = "hashed_password",
                    Rol = ENUM_TIPO_USUARIO.Estudiante,
                    Vigencia = true,
                    FechaRegistro = DateTime.UtcNow
                };
                _context.Usuarios.Add(usuario);
            }

            // Agregar un contexto de prueba si no existe
            if (!_context.Contextos.Any())
            {
                var contexto = new Contexto
                {
                    Id = 1,
                    Nombre = "Contexto Test",
                    Descripcion = "Descripción del contexto de prueba",
                    Scope = ENUM_SCOPE_CONTEXTO.Global,
                    Origen = ENUM_ORIGEN_CONTEXTO.Preset,
                    Vigencia = true,
                    FechaCreacion = DateTime.UtcNow,
                    PromptSeed = "Prompt de prueba"
                };
                _context.Contextos.Add(contexto);
            }

            _context.SaveChanges();
        }

        /// <summary>
        /// TEST PRINCIPAL: Flujo completo de creación de entrevista con IA
        /// 
        /// Este test simula el escenario ideal donde:
        /// 1. Un usuario envía un audio para iniciar una entrevista
        /// 2. El servicio de IA procesa el audio y genera una respuesta
        /// 3. El sistema crea una entrevista en la base de datos
        /// 4. Se generan los diálogos iniciales (usuario + IA)
        /// 5. Se retorna una respuesta exitosa con el ID de la entrevista
        /// 
        /// Verifica que todo el pipeline funcione correctamente:
        /// Controller → Services → Database → Mock IA → HTTP Response
        /// </summary>
        [Fact(DisplayName = "INTEGRACIÓN — Iniciar entrevista con IA crea entrevista y diálogos")]
        public async Task IniciarEntrevistaConIA_IntegrationTest()
        {
            // PREPARACIÓN: Crear un audio simulado y formulario multipart
            var audioBytes = new byte[] { 1, 2, 3, 4 };
            var audioStream = new MemoryStream(audioBytes);

            var form = new MultipartFormDataContent();
            var audio = new StreamContent(audioStream);
            audio.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/wav");

            // Agregar todos los campos requeridos por el endpoint
            form.Add(audio, "Audio", "test.wav");
            form.Add(new StringContent("1"), "UsuarioId");
            form.Add(new StringContent("1"), "ContextoId");
            form.Add(new StringContent("Titulo de prueba"), "Titulo");
            form.Add(new StringContent("5"), "DuracionMin");
            form.Add(new StringContent("{}"), "ContextTraits");

            // EJECUCIÓN: Llamar al endpoint real de la API
            var response = await _client.PostAsync("/api/entrevistas/iniciar-con-ia", form);

            // VERIFICACIÓN: Validar respuesta y interacciones con los servicios
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            Assert.NotNull(result);
            
            // Verificar estructura de la respuesta de IA
            Assert.True(result!.ContainsKey("session_id"));
            Assert.True(result.ContainsKey("success"));
            Assert.True(result.ContainsKey("transcription"));
            Assert.True(result.ContainsKey("response_text"));

            // Extraer y validar el ID de la entrevista creada
            string sessionId = result["session_id"]?.ToString() ?? "";
            Assert.True(!string.IsNullOrEmpty(sessionId));
            Assert.True(int.TryParse(sessionId, out int entrevistaId));
            Assert.True(entrevistaId > 0);

            // Verificar que los servicios mockeados fueron llamados correctamente
            var factory = (TestWebApplicationFactory<Program>)_factory;
            
            // El servicio de IA debe procesar el audio una vez
            factory.MockIaService.Verify(
                x => x.ProcessAudioAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);

            // El servicio de entrevistas debe crear una entrevista
            factory.MockEntrevistaService.Verify(
                x => x.CrearAsync(It.IsAny<Entrevista>()),
                Times.Once);

            // El servicio de diálogos debe crear al menos 2 diálogos (usuario + IA)
            factory.MockDialogoService.Verify(
                x => x.CrearAsync(It.IsAny<Dialogo>()),
                Times.AtLeast(2));
        }

        
        /// <summary>
        /// TEST DE VALIDACIÓN: Endpoint rechaza datos inválidos
        /// 
        /// Este test verifica que el sistema maneja correctamente las entradas inválidas.
        /// Específicamente, cuando no se envía el archivo de audio requerido.
        /// 
        /// Escenario:
        /// 1. Se envía un formulario SIN el archivo de audio
        /// 2. El endpoint debe detectar la falta del archivo obligatorio
        /// 3. Debe retornar HTTP 400 BadRequest
        /// 
        /// Esto prueba las validaciones del controller y la robustez del sistema.
        /// </summary>
        [Fact(DisplayName = "VALIDACIÓN — Datos inválidos retornan BadRequest")]
        public async Task IniciarEntrevistaConIA_WithInvalidData_ShouldReturnBadRequest()
        {
            // PREPARACIÓN: Crear formulario sin archivo de audio (campo requerido)
            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent("1"), "UsuarioId");
            formData.Add(new StringContent("1"), "ContextoId");
            formData.Add(new StringContent("Test"), "Titulo");
            formData.Add(new StringContent("15.0"), "DuracionMin");
            formData.Add(new StringContent("{}"), "ContextTraits");
            // NOTA: No se agrega el archivo "Audio" - esto debería causar error

            // EJECUCIÓN: Llamar al endpoint
            var response = await _client.PostAsync("/api/entrevistas/iniciar-con-ia", formData);

            // VERIFICACIÓN: El endpoint debe rechazar la petición inválida
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }


        /// <summary>
        /// TEST DE ROBUSTEZ: Sistema maneja usuarios inexistentes
        /// 
        /// Este test evalúa cómo se comporta el sistema cuando se referencia
        /// un usuario que no existe en la base de datos.
        /// 
        /// Escenario:
        /// 1. Se envía una petición con UsuarioId = 999 (no existe)
        /// 2. El sistema debe manejar esta situación sin fallar
        /// 3. Debe procesar la entrevista normalmente o según la lógica de negocio
        /// 
        /// Esto prueba la resiliencia del sistema ante datos inconsistentes
        /// y la implementación de la lógica de negocio para usuarios no existentes.
        /// </summary>
        [Fact(DisplayName = "ROBUSTEZ — Usuario inexistente no causa error")]
        public async Task IniciarEntrevistaConIA_WithNonExistentUser_ShouldStillWork()
        {
            // PREPARACIÓN: Usar un ID de usuario que no existe en la base de datos
            var audioContent = new byte[] { 0x52, 0x49, 0x46, 0x46 };
            using var audioStream = new MemoryStream(audioContent);
            using var audioContentStream = new StreamContent(audioStream);
            audioContentStream.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");

            using var formData = new MultipartFormDataContent();
            formData.Add(audioContentStream, "Audio", "test_audio.wav");
            formData.Add(new StringContent("999"), "UsuarioId"); // Usuario que no existe
            formData.Add(new StringContent("1"), "ContextoId");
            formData.Add(new StringContent("Test Usuario Inexistente"), "Titulo");
            formData.Add(new StringContent("10.0"), "DuracionMin");
            formData.Add(new StringContent("{}"), "ContextTraits");

            // EJECUCIÓN: Llamar al endpoint
            var response = await _client.PostAsync("/api/entrevistas/iniciar-con-ia", formData);

            // VERIFICACIÓN: El sistema debe manejar esta situación sin fallar
            response.EnsureSuccessStatusCode(); // No debe lanzar excepción (200-299)
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var iaResponse = JsonSerializer.Deserialize<IAResponse>(responseContent, _jsonOptions);

            // La respuesta de IA debe indicar éxito
            Assert.NotNull(iaResponse);
            Assert.True(iaResponse.Success);
        }

        public void Dispose()
        {
            _scope?.Dispose();
            _client?.Dispose();
            _context?.Dispose();
        }
    }
}