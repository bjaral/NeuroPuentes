using Dapper;
using Npgsql;
using NeuroPuentesAPI.models;
using Microsoft.Extensions.Configuration;

namespace NeuroPuentesAPI.repositories
{
    public class FeedbackUsuarioRepository : IFeedbackUsuarioRepository
    {
        private readonly string _connectionString;

        public FeedbackUsuarioRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<IEnumerable<Feedback_Usuario>> GetAllAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Feedback_Usuario>(
                @"SELECT 
                    _id AS Id,
                    usuario_id AS UsuarioId,
                    tipo AS Tipo,
                    mensaje AS Mensaje,
                    categoria AS Categoria,
                    fecha AS Fecha
                  FROM ""feedback_usuario""");
        }

        public async Task<Feedback_Usuario?> GetByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Feedback_Usuario>(
                @"SELECT 
                    _id AS Id,
                    usuario_id AS UsuarioId,
                    tipo AS Tipo,
                    mensaje AS Mensaje,
                    categoria AS Categoria,
                    fecha AS Fecha
                  FROM ""feedback_usuario"" 
                  WHERE _id = @Id",
                new { Id = id });
        }

        public async Task<int> CrearAsync(Feedback_Usuario feedback)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO ""feedback_usuario"" 
                    (usuario_id, tipo, mensaje, categoria, fecha) 
                  VALUES (
                    @UsuarioId,
                    @Tipo::tipo_feedback,
                    @Mensaje,
                    @Categoria,
                    @Fecha
                  )
                  RETURNING _id",
                new {
                    feedback.UsuarioId,
                    Tipo = feedback.Tipo.ToString().ToLower(),
                    feedback.Mensaje,
                    feedback.Categoria,
                    feedback.Fecha
                });
        }

        public async Task ActualizarAsync(Feedback_Usuario feedback)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"UPDATE ""feedback_usuario"" SET 
                        usuario_id = @UsuarioId,
                        tipo = @Tipo::tipo_feedback,
                        mensaje = @Mensaje,
                        categoria = @Categoria
                    WHERE _id = @Id",
                new {
                    feedback.UsuarioId,
                    Tipo = feedback.Tipo.ToString().ToLower(),
                    feedback.Mensaje,
                    feedback.Categoria,
                    feedback.Id
                });
        }

        public async Task EliminarAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"DELETE FROM ""feedback_usuario"" WHERE _id = @Id", 
                new { Id = id });
        }
    }
}
