using Dapper;
using Npgsql;
using NeuroPuentesAPI.models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public class DialogoRepository : IDialogoRepository
    {
        private readonly string _connectionString;

        public DialogoRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<IEnumerable<Dialogo>> GetAllAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Dialogo>(
                @"SELECT 
                    _id AS Id,
                    entrevista_id AS EntrevistaId,
                    turno AS Turno,
                    sender AS Sender,
                    texto AS Texto,
                    texto_procesado AS TextoProcesado,
                    timestamp AS Timestamp,
                    audio_url AS AudioUrl
                  FROM ""dialogos""");
        }

        public async Task<IEnumerable<Dialogo>> GetByEntrevistaIdAsync(int entrevistaId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Dialogo>(
                @"SELECT 
                    _id AS Id,
                    entrevista_id AS EntrevistaId,
                    turno AS Turno,
                    sender AS Sender,
                    texto AS Texto,
                    texto_procesado AS TextoProcesado,
                    timestamp AS Timestamp,
                    audio_url AS AudioUrl
                  FROM ""dialogos""
                  WHERE entrevista_id = @EntrevistaId",
                new { EntrevistaId = entrevistaId });
        }

        public async Task<Dialogo?> GetByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Dialogo>(
                @"SELECT 
                    _id AS Id,
                    entrevista_id AS EntrevistaId,
                    turno AS Turno,
                    sender AS Sender,
                    texto AS Texto,
                    texto_procesado AS TextoProcesado,
                    timestamp AS Timestamp,
                    audio_url AS AudioUrl
                  FROM ""dialogos""
                  WHERE _id = @Id",
                new { Id = id });
        }

        public async Task<int> CrearAsync(Dialogo dialogo)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO ""dialogos"" 
                    (entrevista_id, turno, sender, texto, texto_procesado, timestamp, audio_url)
                  VALUES (
                    @EntrevistaId, 
                    @Turno, 
                    @Sender::sender_dialogo, 
                    @Texto, 
                    @TextoProcesado, 
                    @Timestamp, 
                    @AudioUrl
                  )
                  RETURNING _id",
                new {
                    dialogo.EntrevistaId,
                    dialogo.Turno,
                    Sender = dialogo.Sender.ToString().ToLower(),
                    dialogo.Texto,
                    dialogo.TextoProcesado,
                    dialogo.Timestamp,
                    dialogo.AudioUrl
                });
        }

        public async Task ActualizarAsync(Dialogo dialogo)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"UPDATE ""dialogos"" SET 
                        turno = @Turno,
                        sender = @Sender::sender_dialogo,
                        texto = @Texto,
                        texto_procesado = @TextoProcesado,
                        audio_url = @AudioUrl
                  WHERE _id = @Id",
                new {
                    dialogo.Turno,
                    Sender = dialogo.Sender.ToString().ToLower(),
                    dialogo.Texto,
                    dialogo.TextoProcesado,
                    dialogo.AudioUrl,
                    dialogo.Id
                });
        }

        public async Task EliminarAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(@"DELETE FROM ""dialogos"" WHERE _id = @Id", new { Id = id });
        }
    }
}
