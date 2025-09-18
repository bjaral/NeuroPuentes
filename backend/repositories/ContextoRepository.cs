using Dapper;
using Npgsql;
using NeuroPuentesAPI.models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public class ContextoRepository : IContextoRepository
    {
        private readonly string _connectionString;

        public ContextoRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<IEnumerable<Contexto>> GetAllAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Contexto>(
                @"SELECT 
                    _id,
                    nombre,
                    descripcion,
                    scope,
                    creado_por,
                    origen,
                    prompt_seed,
                    vigencia,
                    fecha_creacion
                  FROM ""contextos"" ");
        }

        public async Task<IEnumerable<Contexto>> GetAllVigentesAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Contexto>(
                @"SELECT 
                    _id,
                    nombre,
                    descripcion,
                    scope,
                    creado_por,
                    origen,
                    prompt_seed,
                    vigencia,
                    fecha_creacion
                  FROM ""contextos"" 
                  WHERE vigencia = true");
        }

        public async Task<Contexto?> GetByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Contexto>(
                @"SELECT 
                    _id,
                    nombre,
                    descripcion,
                    scope,
                    creado_por,
                    origen,
                    prompt_seed,
                    vigencia,
                    fecha_creacion
                  FROM ""contextos"" 
                  WHERE _id = @Id",
                new { Id = id });
        }

        public async Task<int> CrearAsync(Contexto contexto)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO ""contextos"" 
                    (nombre, descripcion, scope, creado_por, origen, prompt_seed, vigencia, fecha_creacion) 
                  VALUES (@Nombre, @Descripcion, @Scope::scope_contexto, @Creado_por, @Origen::origen_contexto, @Prompt_seed, @Vigencia, @Fecha_creacion) 
                  RETURNING _id",
                new {
                    contexto.Nombre,
                    contexto.Descripcion,
                    Scope = contexto.Scope.ToString().ToLower(),
                    contexto.Creado_por,
                    Origen = contexto.Origen.ToString().ToLower(),
                    contexto.Prompt_seed,
                    contexto.Vigencia,
                    contexto.Fecha_creacion
                });
        }

        public async Task ActualizarAsync(Contexto contexto)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"UPDATE ""contextos"" SET 
                        nombre = @Nombre,
                        descripcion = @Descripcion,
                        scope = @Scope::scope_contexto,
                        creado_por = @Creado_por,
                        origen = @Origen::origen_contexto,
                        prompt_seed = @Prompt_seed,
                        vigencia = @Vigencia,
                        fecha_creacion = @Fecha_creacion
                    WHERE _id = @_id",
                new {
                    contexto.Nombre,
                    contexto.Descripcion,
                    Scope = contexto.Scope.ToString().ToLower(),
                    contexto.Creado_por,
                    Origen = contexto.Origen.ToString().ToLower(),
                    contexto.Prompt_seed,
                    contexto.Vigencia,
                    contexto.Fecha_creacion,
                    contexto._id
                });
        }

        public async Task EliminarAsync(int id) =>
            await new NpgsqlConnection(_connectionString)
                .ExecuteAsync(@"DELETE FROM ""contextos"" WHERE _id = @Id", new { Id = id });
    }
}
