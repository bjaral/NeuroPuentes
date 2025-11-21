using Dapper;
using Npgsql;
using NeuroPuentesAPI.models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public class EvalCategoriaRepository : IEvalCategoriaRepository
    {
        private readonly string _connectionString;

        public EvalCategoriaRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<IEnumerable<Eval_Categoria>> GetAllAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Eval_Categoria>(
                @"SELECT 
                    _id AS Id,
                    eval_entrevista_id AS EvalEntrevistaId,
                    categoria AS Categoria,
                    score AS Score
                  FROM ""eval_categoria""");
        }

    

        public async Task<Eval_Categoria?> GetByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Eval_Categoria>(
                @"SELECT 
                    _id AS Id,
                    eval_entrevista_id AS EvalEntrevistaId,
                    categoria AS Categoria,
                    score AS Score
                  FROM ""eval_categoria"" 
                  WHERE _id = @Id",
                new { Id = id });
        }

        public async Task<int> CrearAsync(Eval_Categoria evalCategoria)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO ""eval_categoria"" 
                    (eval_entrevista_id, categoria, score)
                  VALUES (@EvalEntrevistaId, @Categoria, @Score)
                  RETURNING _id",
                evalCategoria);
        }

        public async Task ActualizarAsync(Eval_Categoria evalCategoria)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"UPDATE ""eval_categoria"" SET 
                        eval_entrevista_id = @EvalEntrevistaId,
                        categoria = @Categoria,
                        score = @Score
                  WHERE _id = @Id",
                evalCategoria);
        }

        public async Task EliminarAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"DELETE FROM ""eval_categoria"" WHERE _id = @Id",
                new { Id = id });
        }


        // dada un entrevista ID

        public async Task<IEnumerable<Eval_Categoria>> GetByEntrevistaIdAsync(int entrevistaId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Eval_Categoria>(
                @"SELECT 
                    _id AS Id,
                    eval_entrevista_id AS EvalEntrevistaId,
                    categoria AS Categoria,
                    score AS Score
                  FROM ""eval_categoria"" 
                  WHERE eval_entrevista_id = @EntrevistaId",
                new { EntrevistaId = entrevistaId });
        }
    }
}
