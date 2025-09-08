using NeuroPuentesAPI.models;
using NeuroPuentesAPI.repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repo;

        public UsuarioService(IUsuarioRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync() =>
            await _repo.GetAllAsync();

        public async Task<Usuario?> GetByIdAsync(int id) =>
            await _repo.GetByIdAsync(id);

        public async Task<int> CrearAsync(Usuario usuario)
        {
            usuario.Password_hash = BCrypt.Net.BCrypt.HashPassword(usuario.Password_hash);
            return await _repo.CrearAsync(usuario);
        }

        public async Task<Usuario?> GetByCorreoAsync(string correo)
        {
            return await _repo.GetByCorreoAsync(correo.Trim());
        }


        public async Task ActualizarAsync(int id, Usuario usuario)
        {
            // Obtener el usuario existente
            var existente = await _repo.GetByIdAsync(id);
            if (existente == null)
                throw new KeyNotFoundException($"Usuario con id {id} no encontrado.");

            // Solo actualizar campos que vienen con valor
            existente.Nombre = string.IsNullOrWhiteSpace(usuario.Nombre) ? existente.Nombre : usuario.Nombre;
            existente.Email = string.IsNullOrWhiteSpace(usuario.Email) ? existente.Email : usuario.Email;
            existente.Nombre_usuario = string.IsNullOrWhiteSpace(usuario.Nombre_usuario) ? existente.Nombre_usuario : usuario.Nombre_usuario;

            // Rol y Vigencia: si son nullables en el DTO, solo actualizar si vienen con valor
            existente.Rol = usuario.Rol; // Si tu DTO hace que Rol sea opcional, agregar chequeo: usuario.Rol.HasValue ? usuario.Rol.Value : existente.Rol;
            existente.Vigencia = usuario.Vigencia; // Igual que Rol

            // Password: solo actualizar si viene no vacío
            if (!string.IsNullOrWhiteSpace(usuario.Password_hash))
                existente.Password_hash = BCrypt.Net.BCrypt.HashPassword(usuario.Password_hash);

            // Fecha_registro normalmente no se modifica
            // existente.Fecha_registro = existente.Fecha_registro;

            // Finalmente actualizar en el repositorio
            await _repo.ActualizarAsync(existente);
        }


        public async Task EliminarAsync(int id) =>
            await _repo.EliminarAsync(id);





        public async Task<Usuario?> AuthenticateAsync(string usuarioNombre, string plainPassword)
        {
            var user = await _repo.GetByUsuarioNombreAsync(usuarioNombre.Trim());
            if (user == null) return null;

            return BCrypt.Net.BCrypt.Verify(plainPassword, user.Password_hash) ? user : null;
        }

        // Autenticación por correo
        public async Task<Usuario?> AuthenticateByCorreoAsync(string correo, string plainPassword)
        {
            var user = await _repo.GetByCorreoAsync(correo.Trim());
            if (user == null) return null;

            return BCrypt.Net.BCrypt.Verify(plainPassword, user.Password_hash) ? user : null;
        }
    }
}