using Dapper;
using Npgsql;
using NeuroPuentesAPI.models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public class StatsUsuarioRepository : IStatsUsuarioRepository
    {
        private readonly string _connectionString;

        public StatsUsuarioRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<IEnumerable<Stats_Usuario>> GetAllAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Stats_Usuario>(
                @"SELECT 
                    _id AS Id,
                    usuario_id AS UsuarioId,
                    fecha_corte AS FechaCorte,
                    total_entrevistas AS TotalEntrevistas,
                    tiempo_total_min AS TiempoTotalMin,
                    score_promedio AS ScorePromedio
                  FROM ""stats_usuario""");
        }

        public async Task<Stats_Usuario?> GetByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Stats_Usuario>(
                @"SELECT 
                    _id AS Id,
                    usuario_id AS UsuarioId,
                    fecha_corte AS FechaCorte,
                    total_entrevistas AS TotalEntrevistas,
                    tiempo_total_min AS TiempoTotalMin,
                    score_promedio AS ScorePromedio
                  FROM ""stats_usuario"" 
                  WHERE _id = @Id",
                new { Id = id });
        }

        public async Task<int> CrearAsync(Stats_Usuario stats)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO ""stats_usuario"" 
                    (usuario_id, fecha_corte, total_entrevistas, tiempo_total_min, score_promedio) 
                  VALUES (
                    @UsuarioId, 
                    @FechaCorte, 
                    @TotalEntrevistas, 
                    @TiempoTotalMin, 
                    @ScorePromedio
                  ) 
                  RETURNING _id",
                stats);
        }

        public async Task ActualizarAsync(Stats_Usuario stats)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"UPDATE ""stats_usuario"" SET 
                        usuario_id = @UsuarioId,
                        fecha_corte = @FechaCorte,
                        total_entrevistas = @TotalEntrevistas,
                        tiempo_total_min = @TiempoTotalMin,
                        score_promedio = @ScorePromedio
                  WHERE _id = @Id",
                stats);
        }

        public async Task EliminarAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"DELETE FROM ""stats_usuario"" WHERE _id = @Id", 
                new { Id = id });
        }
    }
}
