using Dapper;
using Npgsql;
using NeuroPuentesAPI.models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public class CalificacionUsuarioRepository : ICalificacionUsuarioRepository
    {
        private readonly string _connectionString;

        public CalificacionUsuarioRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<IEnumerable<Calificacion_Usuario>> GetAllAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Calificacion_Usuario>(
                @"SELECT 
                    _id AS Id,
                    usuario_id AS UsuarioId,
                    calificacion AS Calificacion,
                    mensaje AS Mensaje,
                    fecha AS Fecha
                  FROM ""calificacion_usuario""");
        }

        public async Task<Calificacion_Usuario?> GetByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Calificacion_Usuario>(
                @"SELECT 
                    _id AS Id,
                    usuario_id AS UsuarioId,
                    calificacion AS Calificacion,
                    mensaje AS Mensaje,
                    fecha AS Fecha
                  FROM ""calificacion_usuario""
                  WHERE _id = @Id",
                new { Id = id });
        }

        public async Task<int> CrearAsync(Calificacion_Usuario calificacion)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO ""calificacion_usuario""
                    (usuario_id, calificacion, mensaje, fecha)
                  VALUES (@UsuarioId, @Calificacion, @Mensaje, @Fecha)
                  RETURNING _id",
                new {
                    calificacion.UsuarioId,
                    calificacion.Calificacion,
                    calificacion.Mensaje,
                    calificacion.Fecha
                });
        }

        public async Task ActualizarAsync(Calificacion_Usuario calificacion)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"UPDATE ""calificacion_usuario"" SET
                    usuario_id = @UsuarioId,
                    calificacion = @Calificacion,
                    mensaje = @Mensaje
                  WHERE _id = @Id",
                new {
                    calificacion.UsuarioId,
                    calificacion.Calificacion,
                    calificacion.Mensaje,
                    calificacion.Id
                });
        }

        public async Task EliminarAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"DELETE FROM ""calificacion_usuario"" WHERE _id = @Id",
                new { Id = id });
        }



        // obtener todas las calificaciones por usuario
        public async Task<IEnumerable<Calificacion_Usuario>> GetByUsuarioIdAsync(int usuarioId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Calificacion_Usuario>(
                @"SELECT 
                    _id AS Id,
                    usuario_id AS UsuarioId,
                    calificacion AS Calificacion,
                    mensaje AS Mensaje,
                    fecha AS Fecha
                  FROM ""calificacion_usuario""
                  WHERE usuario_id = @UsuarioId
                  ORDER BY fecha DESC",
                new { UsuarioId = usuarioId });
        }
    }
}
