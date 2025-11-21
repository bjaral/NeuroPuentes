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
                    _id AS Id,
                    nombre AS Nombre,
                    descripcion AS Descripcion,
                    scope AS Scope,
                    creado_por AS CreadoPor,
                    origen AS Origen,
                    prompt_seed AS PromptSeed,
                    vigencia AS Vigencia,
                    fecha_creacion AS FechaCreacion
                  FROM ""contextos""");
        }

        public async Task<IEnumerable<Contexto>> GetAllVigentesAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Contexto>(
                @"SELECT 
                    _id AS Id,
                    nombre AS Nombre,
                    descripcion AS Descripcion,
                    scope AS Scope,
                    creado_por AS CreadoPor,
                    origen AS Origen,
                    prompt_seed AS PromptSeed,
                    vigencia AS Vigencia,
                    fecha_creacion AS FechaCreacion
                  FROM ""contextos"" 
                  WHERE vigencia = true");
        }

        public async Task<Contexto?> GetByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Contexto>(
                @"SELECT 
                    _id AS Id,
                    nombre AS Nombre,
                    descripcion AS Descripcion,
                    scope AS Scope,
                    creado_por AS CreadoPor,
                    origen AS Origen,
                    prompt_seed AS PromptSeed,
                    vigencia AS Vigencia,
                    fecha_creacion AS FechaCreacion
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
                  VALUES (
                    @Nombre, 
                    @Descripcion, 
                    @Scope::scope_contexto, 
                    @CreadoPor, 
                    @Origen::origen_contexto, 
                    @PromptSeed, 
                    @Vigencia, 
                    @FechaCreacion
                  ) 
                  RETURNING _id",
                new {
                    contexto.Nombre,
                    contexto.Descripcion,
                    Scope = contexto.Scope.ToString().ToLower(),
                    contexto.CreadoPor,
                    Origen = contexto.Origen.ToString().ToLower(),
                    contexto.PromptSeed,
                    contexto.Vigencia,
                    contexto.FechaCreacion
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
                        creado_por = @CreadoPor,
                        origen = @Origen::origen_contexto,
                        prompt_seed = @PromptSeed,
                        vigencia = @Vigencia
                    WHERE _id = @Id",
                new {
                    contexto.Nombre,
                    contexto.Descripcion,
                    Scope = contexto.Scope.ToString().ToLower(),
                    contexto.CreadoPor,
                    Origen = contexto.Origen.ToString().ToLower(),
                    contexto.PromptSeed,
                    contexto.Vigencia,
                    contexto.Id
                });
        }

        public async Task EliminarAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"DELETE FROM ""contextos"" WHERE _id = @Id", 
                new { Id = id });
        }


        // Obtener Contexto por entrevistaId
        public async Task<Contexto?> GetByEntrevistaIdAsync(int entrevistaId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Contexto>(
                @"SELECT c._id AS Id, c.nombre AS Nombre, c.descripcion AS Descripcion, c.scope AS Scope,
                         c.creado_por AS CreadoPor, c.origen AS Origen, c.prompt_seed AS PromptSeed,
                         c.vigencia AS Vigencia, c.fecha_creacion AS FechaCreacion
                  FROM ""contextos"" c
                  INNER JOIN ""entrevistas"" e ON e.contexto_id = c._id
                  WHERE e._id = @EntrevistaId",
                new { EntrevistaId = entrevistaId });
        }
    }
}
