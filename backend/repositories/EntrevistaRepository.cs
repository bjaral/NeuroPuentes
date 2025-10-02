using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NeuroPuentesAPI.models;
using System.Data;

namespace NeuroPuentesAPI.repositories
{
    public class EntrevistaRepository : IEntrevistaRepository
    {
        private readonly string _connectionString;

        public EntrevistaRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        private IDbConnection CreateConnection()
            => new NpgsqlConnection(_connectionString);

        public async Task<IEnumerable<Entrevista>> GetAllAsync()
        {
            const string query = "SELECT * FROM \"entrevistas\" ORDER BY \"fecha_creacion\" DESC";
            using var connection = CreateConnection();
            var result = await connection.QueryAsync<Entrevista>(query);
            return result.ToList();
        }

        public async Task<Entrevista?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM \"entrevistas\" WHERE _id = @Id";
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Entrevista>(query, new { Id = id });
        }

        public async Task CrearAsync(Entrevista entrevista)
        {
            const string query = @"
                INSERT INTO ""entrevistas"" 
                (usuario_id, contexto_id, titulo, descripcion, duracion_min, numero_turnos, contexto_snapshot, fecha_creacion)
                VALUES 
                (@UsuarioId, @ContextoId, @Titulo, @Descripcion, @DuracionMin, @NumeroTurnos, @ContextoSnapshot, @FechaCreacion)";
            
            using var connection = CreateConnection();
            await connection.ExecuteAsync(query, entrevista);
        }

        public async Task ActualizarAsync(Entrevista entrevista)
        {
            const string query = @"
                UPDATE ""entrevistas"" SET
                usuario_id = @UsuarioId,
                contexto_id = @ContextoId,
                titulo = @Titulo,
                descripcion = @Descripcion,
                duracion_min = @DuracionMin,
                numero_turnos = @NumeroTurnos,
                contexto_snapshot = @ContextoSnapshot,
                fecha_cierre = @FechaCierre
                WHERE _id = @Id";

            using var connection = CreateConnection();
            await connection.ExecuteAsync(query, entrevista);
        }

        public async Task EliminarAsync(int id)
        {
            const string query = "DELETE FROM \"entrevistas\" WHERE _id = @Id";
            using var connection = CreateConnection();
            await connection.ExecuteAsync(query, new { Id = id });
        }
    }
}
