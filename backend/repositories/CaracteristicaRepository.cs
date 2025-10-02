using Dapper;
using Npgsql;
using NeuroPuentesAPI.models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public class CaracteristicaRepository : ICaracteristicaRepository
    {
        private readonly string _connectionString;

        public CaracteristicaRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<IEnumerable<Caracteristica>> GetAllAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Caracteristica>(
                @"SELECT 
                    _id AS Id,
                    nombre AS Nombre,
                    descripcion AS Descripcion,
                    grupo AS Grupo,
                    vigencia AS Vigencia
                  FROM ""caracteristicas""");
        }

        public async Task<IEnumerable<Caracteristica>> GetAllVigentesAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Caracteristica>(
                @"SELECT 
                    _id AS Id,
                    nombre AS Nombre,
                    descripcion AS Descripcion,
                    grupo AS Grupo,
                    vigencia AS Vigencia
                  FROM ""caracteristicas"" 
                  WHERE vigencia = true");
        }

        public async Task<Caracteristica?> GetByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Caracteristica>(
                @"SELECT 
                    _id AS Id,
                    nombre AS Nombre,
                    descripcion AS Descripcion,
                    grupo AS Grupo,
                    vigencia AS Vigencia
                  FROM ""caracteristicas"" 
                  WHERE _id = @Id",
                new { Id = id });
        }

        public async Task<int> CrearAsync(Caracteristica caracteristica)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO ""caracteristicas"" 
                    (nombre, descripcion, grupo, vigencia) 
                  VALUES (
                    @Nombre, 
                    @Descripcion, 
                    @Grupo, 
                    @Vigencia
                  ) 
                  RETURNING _id",
                caracteristica);
        }

        public async Task ActualizarAsync(Caracteristica caracteristica)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"UPDATE ""caracteristicas"" SET 
                        nombre = @Nombre,
                        descripcion = @Descripcion,
                        grupo = @Grupo,
                        vigencia = @Vigencia
                    WHERE _id = @Id",
                caracteristica);
        }

        public async Task EliminarAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"DELETE FROM ""caracteristicas"" WHERE _id = @Id", 
                new { Id = id });
        }
    }
}
