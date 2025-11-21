//dotnet 8 packages
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using FluentAssertions;

// API packages
using NeuroPuentesAPI.controllers;
using NeuroPuentesAPI.models;
using NeuroPuentesAPI.services;

namespace NeuroPuentes.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IUsuarioService> _usuarioServiceMock;
        private readonly IConfiguration _configuration;

        public AuthControllerTests()
        {
            _usuarioServiceMock = new Mock<IUsuarioService>();

            // Configuración mock para JWT
            var configData = new Dictionary<string, string?>
            {
                { "Jwt:Key", "ClaveSuperSecretaDePrueba1234567890" }, // ahora 32+ chars
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();
        }

        // -----------------------------------------------------------
        // TEST
        // Debe retornar 200 OK + token JWT cuando las credenciales son correctas.
        // Mock: AuthenticateByCorreoAsync debe devolver un usuario válido
        // -----------------------------------------------------------

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var fakeUser = new Usuario
            {
                Id = 1,
                NombreUsuario = "tato",
                Nombre = "Tato",
                Email = "tato@example.com",
                Rol = ENUM_TIPO_USUARIO.Supervisor,
                Vigencia = true,
                PasswordHash = "fake-hash",
                FechaRegistro = DateTime.UtcNow
            };

            _usuarioServiceMock
                .Setup(s => s.AuthenticateByCorreoAsync("tato@example.com", "1234"))
                .ReturnsAsync(fakeUser);

            var controller = new AuthController(_usuarioServiceMock.Object, _configuration);

            var request = new LoginRequest
            {
                Email = "tato@example.com",
                Password = "1234"
            };

            // Act
            var result = await controller.Login(request) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(200);

            // Convertir a JSON
            var json = System.Text.Json.JsonSerializer.Serialize(result.Value);
            var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Leer token correctamente
            string token = root.GetProperty("token").GetString()!;
            token.Should().NotBeNullOrWhiteSpace();

            // Validar usuario
            var usuario = root.GetProperty("usuario");
            usuario.GetProperty("Id").GetInt32().Should().Be(fakeUser.Id);
        }


        // -----------------------------------------------------------
        // TEST
        // Debe retornar un código HTTP 401 Unauthorized cuando el servicio de dominio indica que las credenciales son inválidas.
        // -----------------------------------------------------------
        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            _usuarioServiceMock
                .Setup(s => s.AuthenticateByCorreoAsync("wrong@example.com", "badpass"))
                .ReturnsAsync((Usuario?)null);

            var controller = new AuthController(_usuarioServiceMock.Object, _configuration);

            var request = new LoginRequest
            {
                Email = "wrong@example.com",
                Password = "badpass"
            };

            // Act
            var result = await controller.Login(request);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();

            var unauthorized = result as UnauthorizedObjectResult;
            unauthorized!.StatusCode.Should().Be(401);
            unauthorized.Value.Should().Be("Credenciales inválidas o usuario inactivo.");
        }
    }
}
