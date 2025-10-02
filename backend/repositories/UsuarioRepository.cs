using Dapper;
using Npgsql;
using NeuroPuentesAPI.models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string _connectionString;

        public UsuarioRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Usuario>(
                @"SELECT 
                    _id AS Id,
                    password_hash AS PasswordHash,
                    rol AS Rol,
                    vigencia AS Vigencia,
                    fecha_registro AS FechaRegistro,
                    nombre_usuario AS NombreUsuario,
                    nombre AS Nombre,
                    email AS Email
                  FROM ""usuarios"" ");
        }

        public async Task<IEnumerable<Usuario>> GetAllVigentesAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Usuario>(
                @"SELECT 
                    _id AS Id,
                    password_hash AS PasswordHash,
                    rol AS Rol,
                    vigencia AS Vigencia,
                    fecha_registro AS FechaRegistro,
                    nombre_usuario AS NombreUsuario,
                    nombre AS Nombre,
                    email AS Email
                  FROM ""usuarios"" 
                  WHERE vigencia = true");
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Usuario>(
                @"SELECT 
                    _id AS Id,
                    password_hash AS PasswordHash,
                    rol AS Rol,
                    vigencia AS Vigencia,
                    fecha_registro AS FechaRegistro,
                    nombre_usuario AS NombreUsuario,
                    nombre AS Nombre,
                    email AS Email
                  FROM ""usuarios"" 
                  WHERE _id = @Id",
                new { Id = id });
        }

        public async Task<Usuario?> GetByUsuarioNombreAsync(string usuarioNombre)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Usuario>(
                @"SELECT 
                    _id AS Id,
                    password_hash AS PasswordHash,
                    rol AS Rol,
                    vigencia AS Vigencia,
                    fecha_registro AS FechaRegistro,
                    nombre_usuario AS NombreUsuario,
                    nombre AS Nombre,
                    email AS Email
                  FROM ""usuarios"" 
                  WHERE LOWER(nombre_usuario) = LOWER(@NombreUsuario)",
                new { NombreUsuario = usuarioNombre.Trim() });
        }

        public async Task<Usuario?> GetByCorreoAsync(string correo)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Usuario>(
                @"SELECT 
                    _id AS Id,
                    password_hash AS PasswordHash,
                    rol AS Rol,
                    vigencia AS Vigencia,
                    fecha_registro AS FechaRegistro,
                    nombre_usuario AS NombreUsuario,
                    nombre AS Nombre,
                    email AS Email
                FROM ""usuarios"" 
                WHERE LOWER(email) = LOWER(@Email)",
                new { Email = correo.Trim() });
        }

        public async Task<int> CrearAsync(Usuario usuario)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO ""usuarios"" 
                    (password_hash, rol, vigencia, fecha_registro, nombre_usuario, nombre, email) 
                  VALUES (@PasswordHash, @Rol::tipo_usuario, @Vigencia, @FechaRegistro, @NombreUsuario, @Nombre, @Email) 
                  RETURNING _id",
                new {
                    usuario.PasswordHash,
                    Rol = usuario.Rol.ToString(),
                    usuario.Vigencia,
                    usuario.FechaRegistro,
                    usuario.NombreUsuario,
                    usuario.Nombre,
                    usuario.Email
                });
        }

        public async Task ActualizarAsync(Usuario usuario)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"UPDATE ""usuarios"" SET 
                        password_hash = @PasswordHash,
                        rol = @Rol::tipo_usuario,
                        vigencia = @Vigencia,
                        fecha_registro = @FechaRegistro,
                        nombre_usuario = @NombreUsuario,
                        nombre = @Nombre,
                        email = @Email
                    WHERE _id = @Id",
                new {
                    usuario.PasswordHash,
                    Rol = usuario.Rol.ToString(),
                    usuario.Vigencia,
                    usuario.FechaRegistro,
                    usuario.NombreUsuario,
                    usuario.Nombre,
                    usuario.Email,
                    usuario.Id
                });
        }

        public async Task EliminarAsync(int id) =>
            await new NpgsqlConnection(_connectionString)
                .ExecuteAsync(@"DELETE FROM ""usuarios"" WHERE _id = @Id", new { Id = id });
    }
}
