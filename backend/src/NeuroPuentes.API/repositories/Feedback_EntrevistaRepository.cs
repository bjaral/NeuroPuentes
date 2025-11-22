using Dapper;
using Npgsql;
using NeuroPuentesAPI.models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public class FeedbackEntrevistaRepository : IFeedbackEntrevistaRepository
    {
        private readonly string _connectionString;

        public FeedbackEntrevistaRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<IEnumerable<Feedback_Entrevista>> GetAllAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Feedback_Entrevista>(
                @"SELECT 
                    _id AS Id,
                    entrevista_id AS EntrevistaId,
                    tipo AS Tipo,
                    mensaje AS Mensaje,
                    categoria AS Categoria,
                    fecha AS Fecha
                  FROM ""feedback_entrevista""");
        }

        public async Task<Feedback_Entrevista?> GetByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Feedback_Entrevista>(
                @"SELECT 
                    _id AS Id,
                    entrevista_id AS EntrevistaId,
                    tipo AS Tipo,
                    mensaje AS Mensaje,
                    categoria AS Categoria,
                    fecha AS Fecha
                  FROM ""feedback_entrevista""
                  WHERE _id = @Id",
                new { Id = id });
        }

        public async Task<IEnumerable<Feedback_Entrevista>> GetByEntrevistaIdAsync(int entrevistaId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Feedback_Entrevista>(
                @"SELECT 
                    _id AS Id,
                    entrevista_id AS EntrevistaId,
                    tipo AS Tipo,
                    mensaje AS Mensaje,
                    categoria AS Categoria,
                    fecha AS Fecha
                  FROM ""feedback_entrevista""
                  WHERE entrevista_id = @EntrevistaId",
                new { EntrevistaId = entrevistaId });
        }

        public async Task<int> CrearAsync(Feedback_Entrevista feedback)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO ""feedback_entrevista""
                    (entrevista_id, tipo, mensaje, categoria, fecha)
                  VALUES (
                    @EntrevistaId,
                    @Tipo::tipo_feedback,
                    @Mensaje,
                    @Categoria,
                    @Fecha
                  )
                  RETURNING _id",
                new
                {
                    feedback.EntrevistaId,
                    Tipo = feedback.Tipo.ToLower(),
                    feedback.Mensaje,
                    feedback.Categoria,
                    feedback.Fecha
                });
        }

        public async Task ActualizarAsync(Feedback_Entrevista feedback)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"UPDATE ""feedback_entrevista"" SET
                        entrevista_id = @EntrevistaId,
                        tipo = @Tipo::tipo_feedback,
                        mensaje = @Mensaje,
                        categoria = @Categoria,
                        fecha = @Fecha
                    WHERE _id = @Id",
                new
                {
                    feedback.Id,
                    feedback.EntrevistaId,
                    Tipo = feedback.Tipo.ToLower(),
                    feedback.Mensaje,
                    feedback.Categoria,
                    feedback.Fecha
                });
        }

        public async Task EliminarAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"DELETE FROM ""feedback_entrevista"" WHERE _id = @Id",
                new { Id = id });
        }
    }
}
