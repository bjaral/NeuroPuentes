using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NeuroPuentesAPI.models;
using System.Data;

namespace NeuroPuentesAPI.repositories
{
    public class Eval_CategoriaRepository : IEval_CategoriaRepository
    {
        private readonly string _connectionString;

        public Eval_CategoriaRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        public async Task<IEnumerable<Eval_Categoria>> GetAllAsync()
        {
            const string query = "SELECT * FROM \"eval_categoria\" ORDER BY _id ASC";
            using var connection = CreateConnection();
            var result = await connection.QueryAsync<Eval_Categoria>(query);
            return result.ToList();
        }

        public async Task<Eval_Categoria?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM \"eval_categoria\" WHERE _id=@Id";
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Eval_Categoria>(query, new { Id = id });
        }

        public async Task CrearAsync(Eval_Categoria categoria)
        {
            const string query = "INSERT INTO \"eval_categoria\" (eval_entrevista_id, categoria, score) VALUES (@EvalEntrevistaId, @Categoria, @Score)";
            using var connection = CreateConnection();
            await connection.ExecuteAsync(query, categoria);
        }

        public async Task ActualizarAsync(Eval_Categoria categoria)
        {
            const string query = "UPDATE \"eval_categoria\" SET eval_entrevista_id=@EvalEntrevistaId, categoria=@Categoria, score=@Score WHERE _id=@Id";
            using var connection = CreateConnection();
            await connection.ExecuteAsync(query, categoria);
        }

        public async Task EliminarAsync(int id)
        {
            const string query = "DELETE FROM \"eval_categoria\" WHERE _id=@Id";
            using var connection = CreateConnection();
            await connection.ExecuteAsync(query, new { Id = id });
        }
    }
}
