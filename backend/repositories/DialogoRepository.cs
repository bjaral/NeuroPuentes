using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NeuroPuentesAPI.models;
using System.Data;

namespace NeuroPuentesAPI.repositories
{
    public class DialogoRepository : IDialogoRepository
    {
        private readonly string _connectionString;

        public DialogoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        public async Task<IEnumerable<Dialogo>> GetAllAsync()
        {
            const string query = "SELECT * FROM \"dialogos\" ORDER BY timestamp ASC";
            using var connection = CreateConnection();
            var result = await connection.QueryAsync<Dialogo>(query);
            return result.ToList();
        }

        public async Task<Dialogo?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM \"dialogos\" WHERE _id = @Id";
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Dialogo>(query, new { Id = id });
        }

        public async Task CrearAsync(Dialogo dialogo)
        {
            const string query = @"
                INSERT INTO ""dialogos"" 
                (entrevista_id, turno, sender, texto, texto_procesado, timestamp, audio_url)
                VALUES (@EntrevistaId, @Turno, @Sender, @Texto, @TextoProcesado, @Timestamp, @AudioUrl)";
            using var connection = CreateConnection();
            await connection.ExecuteAsync(query, dialogo);
        }

        public async Task ActualizarAsync(Dialogo dialogo)
        {
            const string query = @"
                UPDATE ""dialogos"" SET
                entrevista_id = @EntrevistaId,
                turno = @Turno,
                sender = @Sender,
                texto = @Texto,
                texto_procesado = @TextoProcesado,
                audio_url = @AudioUrl
                WHERE _id = @Id";
            using var connection = CreateConnection();
            await connection.ExecuteAsync(query, dialogo);
        }

        public async Task EliminarAsync(int id)
        {
            const string query = "DELETE FROM \"dialogos\" WHERE _id = @Id";
            using var connection = CreateConnection();
            await connection.ExecuteAsync(query, new { Id = id });
        }

        public Task<IEnumerable<Dialogo>> GetByEntrevistaIdAsync(int entrevistaId)
        {
            throw new NotImplementedException();
        }
    }
}
