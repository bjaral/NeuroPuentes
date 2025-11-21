using Dapper;
using Npgsql;
using NeuroPuentesAPI.models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public class EvalEntrevistaRepository : IEvalEntrevistaRepository
    {
        private readonly string _connectionString;

        public EvalEntrevistaRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<IEnumerable<Eval_Entrevista>> GetAllAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Eval_Entrevista>(
                @"SELECT 
                    _id AS Id,
                    entrevista_id AS EntrevistaId,
                    score_final AS ScoreFinal
                  FROM ""eval_entrevista""");
        }

        public async Task<Eval_Entrevista?> GetByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Eval_Entrevista>(
                @"SELECT 
                    _id AS Id,
                    entrevista_id AS EntrevistaId,
                    score_final AS ScoreFinal
                  FROM ""eval_entrevista""
                  WHERE _id = @Id",
                new { Id = id });
        }

        public async Task<Eval_Entrevista?> GetByEntrevistaIdAsync(int entrevistaId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Eval_Entrevista>(
                @"SELECT 
                    _id AS Id,
                    entrevista_id AS EntrevistaId,
                    score_final AS ScoreFinal
                  FROM ""eval_entrevista""
                  WHERE entrevista_id = @EntrevistaId",
                new { EntrevistaId = entrevistaId });
        }

        public async Task<int> CrearAsync(Eval_Entrevista eval)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO ""eval_entrevista"" 
                    (entrevista_id, score_final)
                  VALUES (@EntrevistaId, @ScoreFinal)
                  RETURNING _id",
                eval);
        }

        public async Task ActualizarAsync(Eval_Entrevista eval)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"UPDATE ""eval_entrevista""
                    SET entrevista_id = @EntrevistaId,
                        score_final = @ScoreFinal
                  WHERE _id = @Id",
                eval);
        }

        public async Task EliminarAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"DELETE FROM ""eval_entrevista"" WHERE _id = @Id",
                new { Id = id });
        }
    }
}
