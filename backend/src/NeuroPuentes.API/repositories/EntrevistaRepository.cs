using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;
using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public class EntrevistaRepository : IEntrevistaRepository
    {
        private readonly string _connectionString;

        public EntrevistaRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<IEnumerable<Entrevista>> GetAllAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Entrevista>(
                @"SELECT 
                    _id AS Id,
                    usuario_id AS UsuarioId,
                    contexto_id AS ContextoId,
                    titulo AS Titulo,
                    descripcion AS Descripcion,
                    duracion_min AS DuracionMin,
                    numero_turnos AS NumeroTurnos,
                    fecha_creacion AS FechaCreacion,
                    fecha_cierre AS FechaCierre,
                    contexto_snapshot AS ContextoSnapshot
                FROM ""entrevistas""");
        }

        public async Task<IEnumerable<Entrevista>> GetByUsuarioIdAsync(int usuarioId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Entrevista>(
                @"SELECT 
                    _id AS Id,
                    usuario_id AS UsuarioId,
                    contexto_id AS ContextoId,
                    titulo AS Titulo,
                    descripcion AS Descripcion,
                    duracion_min AS DuracionMin,
                    numero_turnos AS NumeroTurnos,
                    fecha_creacion AS FechaCreacion,
                    fecha_cierre AS FechaCierre,
                    contexto_snapshot AS ContextoSnapshot
                FROM ""entrevistas"" 
                WHERE usuario_id = @UsuarioId",
                new { UsuarioId = usuarioId });
        }

        public async Task<Entrevista?> GetByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Entrevista>(
                @"SELECT 
                    _id AS Id,
                    usuario_id AS UsuarioId,
                    contexto_id AS ContextoId,
                    titulo AS Titulo,
                    descripcion AS Descripcion,
                    duracion_min AS DuracionMin,
                    numero_turnos AS NumeroTurnos,
                    fecha_creacion AS FechaCreacion,
                    fecha_cierre AS FechaCierre,
                    contexto_snapshot AS ContextoSnapshot
                FROM ""entrevistas"" 
                WHERE _id = @Id",
                new { Id = id });
        }

        public async Task<int> CrearAsync(Entrevista entrevista)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO ""entrevistas"" 
                    (usuario_id, contexto_id, titulo, descripcion, duracion_min, numero_turnos, fecha_creacion, contexto_snapshot)
                  VALUES (@UsuarioId, @ContextoId, @Titulo, @Descripcion, @DuracionMin, @NumeroTurnos, @FechaCreacion, @ContextoSnapshot)
                  RETURNING _id",
                entrevista);
        }

        public async Task ActualizarAsync(Entrevista entrevista)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"UPDATE ""entrevistas"" SET 
                        titulo = @Titulo,
                        descripcion = @Descripcion,
                        duracion_min = @DuracionMin,
                        numero_turnos = @NumeroTurnos,
                        fecha_cierre = @FechaCierre,
                        contexto_snapshot = @ContextoSnapshot
                    WHERE _id = @Id",
                entrevista);
        }

        public async Task EliminarAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"DELETE FROM ""entrevistas"" WHERE _id = @Id", new { Id = id });
        }
    }
}
