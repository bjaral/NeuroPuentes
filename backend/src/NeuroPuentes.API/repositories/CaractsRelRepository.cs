using Dapper;
using Npgsql;
using NeuroPuentesAPI.models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public class CaractsRelRepository : ICaractsRelRepository
    {
        private readonly string _connectionString;

        public CaractsRelRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<IEnumerable<Caracts_Rel>> GetAllAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Caracts_Rel>(
                @"SELECT caracteristica_id AS CaracteristicaId, contexto_id AS ContextoId
                  FROM ""caracts_rel""");
        }

        public async Task<Caracts_Rel?> GetByIdsAsync(int caracteristicaId, int contextoId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Caracts_Rel>(
                @"SELECT caracteristica_id AS CaracteristicaId, contexto_id AS ContextoId
                  FROM ""caracts_rel""
                  WHERE caracteristica_id = @CaracteristicaId AND contexto_id = @ContextoId",
                new { CaracteristicaId = caracteristicaId, ContextoId = contextoId });
        }

        public async Task CrearAsync(Caracts_Rel rel)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"INSERT INTO ""caracts_rel"" (caracteristica_id, contexto_id)
                  VALUES (@CaracteristicaId, @ContextoId)",
                rel);
        }

        public async Task EliminarAsync(int caracteristicaId, int contextoId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"DELETE FROM ""caracts_rel""
                  WHERE caracteristica_id = @CaracteristicaId AND contexto_id = @ContextoId",
                new { CaracteristicaId = caracteristicaId, ContextoId = contextoId });
        }
    }
}
