using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NeuroPuentesAPI.models;
using System.Data;

namespace NeuroPuentesAPI.repositories
{
    public class Feedback_EntrevistaRepository : IFeedback_EntrevistaRepository
    {
        private readonly string _connectionString;

        public Feedback_EntrevistaRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        public async Task<IEnumerable<Feedback_Entrevista>> GetAllAsync()
        {
            const string query = "SELECT * FROM \"Feedback_Entrevista\" ORDER BY fecha ASC";
            using var connection = CreateConnection();
            var result = await connection.QueryAsync<Feedback_Entrevista>(query);
            return result.ToList();
        }

        public async Task<Feedback_Entrevista?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM \"Feedback_Entrevista\" WHERE _id=@Id";
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Feedback_Entrevista>(query, new { Id = id });
        }

        public async Task CrearAsync(Feedback_Entrevista feedback)
        {
            const string query = @"
                INSERT INTO ""Feedback_Entrevista"" 
                (entrevista_id, tipo, mensaje, categoria, fecha)
                VALUES (@EntrevistaId, @Tipo, @Mensaje, @Categoria, @Fecha)";
            using var connection = CreateConnection();
            await connection.ExecuteAsync(query, feedback);
        }

        public async Task ActualizarAsync(Feedback_Entrevista feedback)
        {
            const string query = @"
                UPDATE ""Feedback_Entrevista"" SET
                entrevista_id=@EntrevistaId,
                tipo=@Tipo,
                mensaje=@Mensaje,
                categoria=@Categoria
                WHERE _id=@Id";
            using var connection = CreateConnection();
            await connection.ExecuteAsync(query, feedback);
        }

        public async Task EliminarAsync(int id)
        {
            const string query = "DELETE FROM \"Feedback_Entrevista\" WHERE _id=@Id";
            using var connection = CreateConnection();
            await connection.ExecuteAsync(query, new { Id = id });
        }
    }
}
