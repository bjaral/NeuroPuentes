using NeuroPuentesAPI.DTOs;
using NeuroPuentesAPI.models;
using NeuroPuentesAPI.Repositories;

namespace NeuroPuentesAPI.services
{
    public class EntrevistaService : IEntrevistaService
    {
        private readonly IEntrevistaRepository _repo;

        public EntrevistaService(IEntrevistaRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<EntrevistaDto>> GetAllAsync()
        {
            var entrevistas = await _repo.GetAllAsync();
            return entrevistas.Select(e => new EntrevistaDto
            {
                Id = e.Id,
                UsuarioId = e.UsuarioId,
                ContextoId = e.ContextoId,
                Titulo = e.Titulo,
                Descripcion = e.Descripcion,
                DuracionMin = e.DuracionMin,
                NumeroTurnos = e.NumeroTurnos,
                FechaCreacion = e.FechaCreacion,
                FechaCierre = e.FechaCierre,
                ContextoSnapshot = e.ContextoSnapshot
            });
        }

        public async Task<EntrevistaDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;

            return new EntrevistaDto
            {
                Id = e.Id,
                UsuarioId = e.UsuarioId,
                ContextoId = e.ContextoId,
                Titulo = e.Titulo,
                Descripcion = e.Descripcion,
                DuracionMin = e.DuracionMin,
                NumeroTurnos = e.NumeroTurnos,
                FechaCreacion = e.FechaCreacion,
                FechaCierre = e.FechaCierre,
                ContextoSnapshot = e.ContextoSnapshot
            };
        }

        public async Task CrearAsync(EntrevistaCreateDto dto)
        {
            var e = new Entrevista
            {
                UsuarioId = dto.UsuarioId,
                ContextoId = dto.ContextoId,
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                DuracionMin = dto.DuracionMin,
                NumeroTurnos = dto.NumeroTurnos,
                ContextoSnapshot = dto.ContextoSnapshot,
                FechaCreacion = DateTime.UtcNow
            };

            await _repo.CrearAsync(e);
        }

        public async Task ActualizarAsync(int id, EntrevistaCreateDto dto)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) throw new KeyNotFoundException("Entrevista no encontrada");

            e.Titulo = dto.Titulo;
            e.Descripcion = dto.Descripcion;
            e.DuracionMin = dto.DuracionMin;
            e.NumeroTurnos = dto.NumeroTurnos;
            e.ContextoSnapshot = dto.ContextoSnapshot;

            await _repo.ActualizarAsync(e);
        }

        public async Task EliminarAsync(int id)
        {
            await _repo.EliminarAsync(id);
        }
    }
}
