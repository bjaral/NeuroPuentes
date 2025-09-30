using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NeuroPuentesAPI.models;
using System.Data;

namespace NeuroPuentesAPI.repositories
{
    public class Eval_EntrevistaRepository : IEval_EntrevistaRepository
    {
        private readonly string _connectionString;

        public Eval_EntrevistaRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        public async Task<IEnumerable<Eval_Entrevista>> GetAllAsync()
        {
            const string query = "SELECT * FROM \"Eval_Entrevista\" ORDER BY _id ASC";
            using var connection = CreateConnection();
            var result = await connection.QueryAsync<Eval_Entrevista>(query);
            return result.ToList();
        }

        public async Task<Eval_Entrevista?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM \"Eval_Entrevista\" WHERE _id = @Id";
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Eval_Entrevista>(query, new { Id = id });
        }

        public async Task CrearAsync(Eval_Entrevista eval)
        {
            const string query = "INSERT INTO \"Eval_Entrevista\" (entrevista_id, score_final) VALUES (@EntrevistaId, @ScoreFinal)";
            using var connection = CreateConnection();
            await connection.ExecuteAsync(query, eval);
        }

        public async Task ActualizarAsync(Eval_Entrevista eval)
        {
            const string query = "UPDATE \"Eval_Entrevista\" SET entrevista_id=@EntrevistaId, score_final=@ScoreFinal WHERE _id=@Id";
            using var connection = CreateConnection();
            await connection.ExecuteAsync(query, eval);
        }

        public async Task EliminarAsync(int id)
        {
            const string query = "DELETE FROM \"Eval_Entrevista\" WHERE _id=@Id";
            using var connection = CreateConnection();
            await connection.ExecuteAsync(query, new { Id = id });
        }
    }
}
