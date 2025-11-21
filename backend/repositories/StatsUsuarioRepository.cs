using Dapper;
using Npgsql;
using NeuroPuentesAPI.models;
using NeuroPuentesAPI.DTOs;
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
    

        public async Task<IEnumerable<Stats_Usuario>> GetByUsuarioIdAsync(int usuarioId)
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
                FROM ""stats_usuario""
                WHERE usuario_id = @UsuarioId
                ORDER BY fecha_corte ASC",
                new { UsuarioId = usuarioId });
        }


        public async Task<StatsUsuarioResumenDto?> GetResumenPorUsuarioAsync(int usuarioId, DateTime fechaInicio, DateTime fechaFin)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var query = @"
            SELECT 
                @UsuarioId AS UsuarioId,
                (SELECT COUNT(*) FROM entrevistas e 
                    WHERE e.usuario_id = @UsuarioId 
                    AND e.fecha_creacion BETWEEN @FechaInicio AND @FechaFin
                ) AS TotalEntrevistas,
                
                (SELECT COALESCE(SUM(e.duracion_min),0) FROM entrevistas e
                    WHERE e.usuario_id = @UsuarioId
                    AND e.fecha_creacion BETWEEN @FechaInicio AND @FechaFin
                ) AS TiempoTotalMin,
                
                (SELECT COALESCE(AVG(ec.score),0)
                    FROM eval_entrevista ee
                    JOIN eval_categoria ec ON ee._id = ec.eval_entrevista_id
                    JOIN entrevistas e ON ee.entrevista_id = e._id
                    WHERE e.usuario_id = @UsuarioId
                    AND e.fecha_creacion BETWEEN @FechaInicio AND @FechaFin
                    AND ec.categoria = 'fluidez'
                ) AS FluidezPromedio,

                (SELECT COALESCE(AVG(ec.score),0)
                    FROM eval_entrevista ee
                    JOIN eval_categoria ec ON ee._id = ec.eval_entrevista_id
                    JOIN entrevistas e ON ee.entrevista_id = e._id
                    WHERE e.usuario_id = @UsuarioId
                    AND e.fecha_creacion BETWEEN @FechaInicio AND @FechaFin
                    AND ec.categoria = 'empatia'
                ) AS EmpatiaPromedio,

                (SELECT COALESCE(AVG(ee.score_final),0)
                    FROM eval_entrevista ee
                    JOIN entrevistas e ON ee.entrevista_id = e._id
                    WHERE e.usuario_id = @UsuarioId
                    AND e.fecha_creacion BETWEEN @FechaInicio AND @FechaFin
                ) AS ScorePromedio,

                (SELECT COUNT(*) FROM feedback_entrevista fe
                    JOIN entrevistas e ON fe.entrevista_id = e._id
                    WHERE e.usuario_id = @UsuarioId
                    AND e.fecha_creacion BETWEEN @FechaInicio AND @FechaFin
                    AND fe.tipo = 'fortaleza'
                ) AS FeedbackFortalezas,

                (SELECT COUNT(*) FROM feedback_entrevista fe
                    JOIN entrevistas e ON fe.entrevista_id = e._id
                    WHERE e.usuario_id = @UsuarioId
                    AND e.fecha_creacion BETWEEN @FechaInicio AND @FechaFin
                    AND fe.tipo = 'debilidad'
                ) AS FeedbackDebilidades,

                (SELECT COUNT(*) FROM entrevistas e
                    JOIN contextos c ON e.contexto_id = c._id
                    WHERE e.usuario_id = @UsuarioId
                    AND e.fecha_creacion BETWEEN @FechaInicio AND @FechaFin
                    AND LOWER(c.nombre) LIKE '%dificil%'
                ) AS ContextosDificiles,

                (SELECT COUNT(*) FROM entrevistas e
                    JOIN contextos c ON e.contexto_id = c._id
                    WHERE e.usuario_id = @UsuarioId
                    AND e.fecha_creacion BETWEEN @FechaInicio AND @FechaFin
                    AND LOWER(c.nombre) LIKE '%facil%'
                ) AS ContextosFaciles,

                (SELECT COALESCE(AVG(e.numero_turnos),0)
                    FROM entrevistas e
                    WHERE e.usuario_id = @UsuarioId
                    AND e.fecha_creacion BETWEEN @FechaInicio AND @FechaFin
                ) AS NumTurnosPromedio
            ";



            return await connection.QueryFirstOrDefaultAsync<StatsUsuarioResumenDto>(query, new
            {
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            });
        }



    }
}
