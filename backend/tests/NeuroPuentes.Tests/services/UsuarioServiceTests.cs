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
        /// <summary>
        /// TEST DE SEGURIDAD: Hashing de contraseñas con BCrypt
        /// 
        /// Este test verifica el aspecto crítico de seguridad donde:
        /// 1. Un usuario se registra con una contraseña en texto plano
        /// 2. El servicio aplica el algoritmo de hashing BCrypt
        /// 3. La contraseña hasheada se guarda en la base de datos
        /// 4. La contraseña original NUNCA se almacena en texto plano
        /// 
        /// Verifica que:
        /// - El hash generado es diferente al texto original
        /// - El hash es válido y puede ser verificado por BCrypt
        /// - El repositorio recibe la contraseña hasheada, no la original
        /// - Se cumple el principio fundamental de seguridad: nunca almacenar passwords en texto plano
        /// 
        /// Esto garantiza la protección de las credenciales de los usuarios
        /// incluso en caso de brecha de seguridad en la base de datos.
        /// </summary>
        [Fact(DisplayName = "SEGURIDAD — Contraseña se hashea con BCrypt antes de guardar")]
        public async Task CrearAsync_HashesPassword_BeforeSaving()
        {
            // PREPARACIÓN: Configurar mock del repositorio para capturar el usuario recibido
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
                PasswordHash = "contraseña_original", // Contraseña en texto plano

                // Campos required del modelo
                Rol = ENUM_TIPO_USUARIO.Estudiante,
                Vigencia = true,
                FechaRegistro = DateTime.UtcNow
            };

            // EJECUCIÓN: Llamar al servicio para crear el usuario
            await service.CrearAsync(usuario);

            // VERIFICACIÓN: Validar que la contraseña fue hasheada correctamente
            Assert.NotNull(usuarioRecibidoPorElRepo);
            
            // La contraseña hasheada debe ser DIFERENTE a la original
            Assert.NotEqual("contraseña_original", usuarioRecibidoPorElRepo!.PasswordHash);

            // Verificar que el hash es válido según BCrypt (puede verificar la contraseña original)
            Assert.True(BCrypt.Net.BCrypt.Verify("contraseña_original", usuarioRecibidoPorElRepo!.PasswordHash));

            // Verificar que el repositorio fue llamado exactamente una vez
            mockRepo.Verify(r => r.CrearAsync(It.IsAny<Usuario>()), Times.Once);
        }

        /// <summary>
        /// TEST DE AUTENTICACIÓN: Verificación de credenciales con BCrypt
        /// 
        /// Este test verifica que el servicio puede autenticar usuarios
        /// comparando la contraseña proporcionada con el hash almacenado.
        /// 
        /// Escenario:
        /// 1. Un usuario existe en el sistema con contraseña hasheada
        /// 2. Se proporcionan credenciales para autenticar
        /// 3. El servicio verifica usando BCrypt.Verify()
        /// 4. Retorna el usuario si las credenciales son válidas
        /// 
        /// Esto prueba el flujo completo de login desde el lado del servicio.
        /// </summary>
        [Fact(DisplayName = "AUTENTICACIÓN — Verificación de credenciales con hash BCrypt")]
        public async Task AuthenticateByCorreoAsync_WithValidCredentials_ReturnsUser()
        {
            // PREPARACIÓN: Crear un usuario con contraseña hasheada
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("contraseña123");
            var usuarioExistente = new Usuario
            {
                Id = 1,
                Nombre = "Maria",
                Email = "maria@test.com",
                NombreUsuario = "maria456",
                PasswordHash = passwordHash,
                Rol = ENUM_TIPO_USUARIO.Estudiante,
                Vigencia = true,
                FechaRegistro = DateTime.UtcNow
            };

            var mockRepo = new Mock<IUsuarioRepository>();
            mockRepo.Setup(r => r.GetByCorreoAsync("maria@test.com"))
                   .ReturnsAsync(usuarioExistente);

            var service = new UsuarioService(mockRepo.Object);

            // EJECUCIÓN: Intentar autenticar con credenciales correctas
            var resultado = await service.AuthenticateByCorreoAsync("maria@test.com", "contraseña123");

            // VERIFICACIÓN: Debe retornar el usuario ya que las credenciales son válidas
            Assert.NotNull(resultado);
            Assert.Equal(usuarioExistente.Id, resultado.Id);
            Assert.Equal(usuarioExistente.Email, resultado.Email);
        }

        /// <summary>
        /// TEST DE SEGURIDAD: Autenticación fallida con credenciales incorrectas
        /// 
        /// Este test verifica que el servicio rechaza credenciales incorrectas
        /// incluso cuando el usuario existe en el sistema.
        /// 
        /// Escenario:
        /// 1. Existe un usuario con email "maria@test.com"
        /// 2. Se proporciona una contraseña incorrecta
        /// 3. El servicio debe retornar null
        /// 
        /// Esto prueba la robustez del sistema contra ataques de fuerza bruta
        /// y asegura que solo las credenciales exactas permitan el acceso.
        /// </summary>
        [Fact(DisplayName = "SEGURIDAD — Credenciales incorrectas retornan null")]
        public async Task AuthenticateByCorreoAsync_WithInvalidPassword_ReturnsNull()
        {
            // PREPARACIÓN: Usuario existe pero contraseña incorrecta
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("contraseña_correcta");
            var usuarioExistente = new Usuario
            {
                Id = 1,
                Nombre = "Maria",
                Email = "maria@test.com",
                NombreUsuario = "maria456",
                PasswordHash = passwordHash,
                Rol = ENUM_TIPO_USUARIO.Estudiante,
                Vigencia = true,
                FechaRegistro = DateTime.UtcNow
            };

            var mockRepo = new Mock<IUsuarioRepository>();
            mockRepo.Setup(r => r.GetByCorreoAsync("maria@test.com"))
                   .ReturnsAsync(usuarioExistente);

            var service = new UsuarioService(mockRepo.Object);

            // EJECUCIÓN: Intentar autenticar con contraseña INCORRECTA
            var resultado = await service.AuthenticateByCorreoAsync("maria@test.com", "contraseña_incorrecta");

            // VERIFICACIÓN: Debe retornar null por contraseña incorrecta
            Assert.Null(resultado);
        }

        /// <summary>
        /// TEST DE USUARIO INEXISTENTE: Autenticación con email no registrado
        /// 
        /// Este test verifica el comportamiento cuando se intenta autenticar
        /// un usuario que no existe en el sistema.
        /// 
        /// Escenario:
        /// 1. No existe usuario con el email proporcionado
        /// 2. El repositorio retorna null
        /// 3. El servicio debe retornar null
        /// 
        /// Esto prueba el manejo de casos edge y evita excepciones
        /// por usuarios no encontrados.
        /// </summary>
        [Fact(DisplayName = "AUTENTICACIÓN — Usuario inexistente retorna null")]
        public async Task AuthenticateByCorreoAsync_WithNonExistentEmail_ReturnsNull()
        {
            // PREPARACIÓN: No existe usuario con este email
            var mockRepo = new Mock<IUsuarioRepository>();
            mockRepo.Setup(r => r.GetByCorreoAsync("noexiste@test.com"))
                   .ReturnsAsync((Usuario?)null);

            var service = new UsuarioService(mockRepo.Object);

            // EJECUCIÓN: Intentar autenticar usuario que no existe
            var resultado = await service.AuthenticateByCorreoAsync("noexiste@test.com", "cualquierpassword");

            // VERIFICACIÓN: Debe retornar null por usuario no encontrado
            Assert.Null(resultado);
        }

        /// <summary>
        /// TEST DE USUARIO INACTIVO: No permite autenticación de usuarios desactivados
        /// 
        /// Este test verifica que usuarios marcados como no vigentes
        /// no puedan autenticarse en el sistema, incluso con credenciales correctas.
        /// 
        /// Escenario:
        /// 1. Usuario existe con credenciales correctas
        /// 2. Pero tiene Vigencia = false
        /// 3. El servicio debe retornar null
        /// 
        /// Esto garantiza el control de acceso y la capacidad de desactivar usuarios
        /// sin eliminarlos del sistema.
        /// </summary>
        [Fact(DisplayName = "SEGURIDAD — Usuario inactivo no puede autenticarse")]
        public async Task AuthenticateByCorreoAsync_WithInactiveUser_ReturnsNull()
        {
            // PREPARACIÓN: Usuario existe pero está inactivo
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("contraseña123");
            var usuarioInactivo = new Usuario
            {
                Id = 1,
                Nombre = "Usuario Inactivo",
                Email = "inactivo@test.com",
                NombreUsuario = "inactivo123",
                PasswordHash = passwordHash,
                Rol = ENUM_TIPO_USUARIO.Estudiante,
                Vigencia = false, // USUARIO INACTIVO
                FechaRegistro = DateTime.UtcNow
            };

            var mockRepo = new Mock<IUsuarioRepository>();
            mockRepo.Setup(r => r.GetByCorreoAsync("inactivo@test.com"))
                   .ReturnsAsync(usuarioInactivo);

            var service = new UsuarioService(mockRepo.Object);

            // EJECUCIÓN: Intentar autenticar usuario inactivo
            var resultado = await service.AuthenticateByCorreoAsync("inactivo@test.com", "contraseña123");

            // VERIFICACIÓN: Debe retornar null aunque las credenciales sean correctas
            Assert.Null(resultado);
        }
    }
}