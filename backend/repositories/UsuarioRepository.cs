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
                    _id,
                    password_hash,
                    rol,
                    vigencia,
                    fecha_registro,
                    nombre_usuario,
                    nombre,
                    email
                  FROM ""usuarios"" WHERE vigencia = true");
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Usuario>(
                @"SELECT 
                    _id,
                    password_hash,
                    rol,
                    vigencia,
                    fecha_registro,
                    nombre_usuario,
                    nombre,
                    email
                  FROM ""usuarios"" 
                  WHERE _id = @Id AND vigencia = true",
                new { Id = id });
        }

        public async Task<Usuario?> GetByUsuarioNombreAsync(string usuarioNombre)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Usuario>(
                @"SELECT 
                    _id,
                    password_hash,
                    rol,
                    vigencia,
                    fecha_registro,
                    nombre_usuario,
                    nombre,
                    email
                  FROM ""usuarios"" 
                  WHERE LOWER(nombre_usuario) = LOWER(@Nombre_usuario) AND vigencia = true",
                new { Nombre_usuario = usuarioNombre.Trim() });
        }

        public async Task<Usuario?> GetByCorreoAsync(string correo)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Usuario>(
                @"SELECT 
                    _id,
                    password_hash,
                    rol,
                    vigencia,
                    fecha_registro,
                    nombre_usuario,
                    nombre,
                    email
                FROM ""usuarios"" 
                WHERE LOWER(email) = LOWER(@Email) AND vigencia = true",
                new { Email = correo.Trim() });
        }

        public async Task<int> CrearAsync(Usuario usuario)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO ""usuarios"" 
                    (password_hash, rol, vigencia, fecha_registro, nombre_usuario, nombre, email) 
                  VALUES (@Password_hash, @Rol::tipo_usuario, @Vigencia, @Fecha_registro, @Nombre_usuario, @Nombre, @Email) 
                  RETURNING _id",
                new {
                    usuario.Password_hash,
                    Rol = usuario.Rol.ToString(),
                    usuario.Vigencia,
                    usuario.Fecha_registro,
                    usuario.Nombre_usuario,
                    usuario.Nombre,
                    usuario.Email
                });
        }

        public async Task ActualizarAsync(Usuario usuario)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"UPDATE ""usuarios"" SET 
                        password_hash = @Password_hash,
                        rol = @Rol::tipo_usuario,
                        vigencia = @Vigencia,
                        fecha_registro = @Fecha_registro,
                        nombre_usuario = @Nombre_usuario,
                        nombre = @Nombre,
                        email = @Email
                    WHERE _id = @_id",
                new {
                    usuario.Password_hash,
                    Rol = usuario.Rol.ToString(),
                    usuario.Vigencia,
                    usuario.Fecha_registro,
                    usuario.Nombre_usuario,
                    usuario.Nombre,
                    usuario.Email,
                    usuario._id
                });
        }

        public async Task EliminarAsync(int id) =>
            await new NpgsqlConnection(_connectionString)
                .ExecuteAsync(@"DELETE FROM ""usuarios"" WHERE _id = @Id", new { Id = id });
    }
}
