
using Xunit;
using Moq;
using NeuroPuentesAPI.models;
using NeuroPuentesAPI.services;
using NeuroPuentesAPI.repositories;
using System.Threading.Tasks;
using BCrypt.Net;

namespace NeuroPuentes.Tests.Services
{
    public class UsuarioServiceTests
    {
        
        // -----------------------------------------------------------
        // TEST
        // UsuarioService - Hashing
        // Verificar que, al crear un usuario, la contraseña se procese mediante el algoritmo de hashing (bcrypt) 
        // antes de invocar al repositorio, asegurando que nunca se guarde en texto plano.
        // -----------------------------------------------------------
        [Fact]
        public async Task CrearAsync_HashesPassword_BeforeSaving()
        {
            // Arrange
            var mockRepo = new Mock<IUsuarioRepository>();

            Usuario? usuarioRecibidoPorElRepo = null;

            mockRepo
                .Setup(r => r.CrearAsync(It.IsAny<Usuario>()))
                .Callback<Usuario>(u => usuarioRecibidoPorElRepo = u)
                .ReturnsAsync(1);

            var service = new UsuarioService(mockRepo.Object);

            var usuario = new Usuario
            {
                Id = 1,
                Nombre = "Juan",
                Email = "juan@test.com",
                NombreUsuario = "juan123",
                PasswordHash = "contraseña_original",

                // Campos required del modelo
                Rol = ENUM_TIPO_USUARIO.Estudiante,
                Vigencia = true,
                FechaRegistro = DateTime.UtcNow
            };

            // Act
            await service.CrearAsync(usuario);

            // Assert
            Assert.NotNull(usuarioRecibidoPorElRepo);
            Assert.NotEqual("contraseña_original", usuarioRecibidoPorElRepo!.PasswordHash);

            // Verificar que el hash es válido según BCrypt
            Assert.True(BCrypt.Net.BCrypt.Verify("contraseña_original", usuarioRecibidoPorElRepo!.PasswordHash));

            // Verificar que el repositorio fue llamado exactamente una vez
            mockRepo.Verify(r => r.CrearAsync(It.IsAny<Usuario>()), Times.Once);
        }
    }
}
