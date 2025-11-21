using Dapper;
using Npgsql;
using NeuroPuentesAPI.models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public class TipRepository : ITipRepository
    {
        private readonly string _connectionString;

        public TipRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<IEnumerable<Tip>> GetAllAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Tip>(
                @"SELECT 
                    _id AS Id,
                    usuario_id AS UsuarioId,
                    entrevista_id AS EntrevistaId,
                    titulo AS Titulo,
                    contenido AS Contenido,
                    categoria AS Categoria,
                    fecha AS Fecha,
                    usado AS Usado,
                    vigencia AS Vigencia
                  FROM ""tips""");
        }

        public async Task<IEnumerable<Tip>> GetAllVigentesAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Tip>(
                @"SELECT 
                    _id AS Id,
                    usuario_id AS UsuarioId,
                    entrevista_id AS EntrevistaId,
                    titulo AS Titulo,
                    contenido AS Contenido,
                    categoria AS Categoria,
                    fecha AS Fecha,
                    usado AS Usado,
                    vigencia AS Vigencia
                  FROM ""tips""
                  WHERE vigencia = true");
        }

        public async Task<Tip?> GetByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Tip>(
                @"SELECT 
                    _id AS Id,
                    usuario_id AS UsuarioId,
                    entrevista_id AS EntrevistaId,
                    titulo AS Titulo,
                    contenido AS Contenido,
                    categoria AS Categoria,
                    fecha AS Fecha,
                    usado AS Usado,
                    vigencia AS Vigencia
                  FROM ""tips""
                  WHERE _id = @Id",
                new { Id = id });
        }

        public async Task<int> CrearAsync(Tip tip)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO ""tips"" 
                    (usuario_id, entrevista_id, titulo, contenido, categoria, fecha, usado, vigencia)
                  VALUES (
                    @UsuarioId,
                    @EntrevistaId,
                    @Titulo,
                    @Contenido,
                    @Categoria,
                    @Fecha,
                    @Usado,
                    @Vigencia
                  ) RETURNING _id",
                tip);
        }

        public async Task ActualizarAsync(Tip tip)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"UPDATE ""tips"" SET
                        usuario_id = @UsuarioId,
                        entrevista_id = @EntrevistaId,
                        titulo = @Titulo,
                        contenido = @Contenido,
                        categoria = @Categoria,
                        fecha = @Fecha,
                        usado = @Usado,
                        vigencia = @Vigencia
                  WHERE _id = @Id",
                tip);
        }

        public async Task EliminarAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"DELETE FROM ""tips"" WHERE _id = @Id",
                new { Id = id });
        }
        public async Task<IEnumerable<Tip>> GetByUsuarioIdAsync(int usuarioId)
        {
            const string query = @"
                SELECT 
                    _id AS Id,
                    usuario_id AS UsuarioId,
                    entrevista_id AS EntrevistaId,
                    titulo AS Titulo,
                    contenido AS Contenido,
                    categoria AS Categoria,
                    fecha AS Fecha,
                    usado AS Usado,
                    vigencia AS Vigencia
                FROM ""tips""
                WHERE usuario_id = @UsuarioId
                ORDER BY fecha DESC
                LIMIT 3";

            using var connection = new NpgsqlConnection(_connectionString);
            var tips = await connection.QueryAsync<Tip>(query, new { UsuarioId = usuarioId });
            return tips;
        }

        public async Task<IEnumerable<Tip>> GetByEntrevistaIdAsync(int entrevistaId)
        {
            const string query = @"
                SELECT 
                    _id AS Id,
                    usuario_id AS UsuarioId,
                    entrevista_id AS EntrevistaId,
                    titulo AS Titulo,
                    contenido AS Contenido,
                    categoria AS Categoria,
                    fecha AS Fecha,
                    usado AS Usado,
                    vigencia AS Vigencia
                FROM ""tips""
                WHERE entrevista_id = @EntrevistaId
                ORDER BY fecha DESC
                LIMIT 3";

            using var connection = new NpgsqlConnection(_connectionString);
            var tips = await connection.QueryAsync<Tip>(query, new { EntrevistaId = entrevistaId });
            return tips;
        }
    }
}
