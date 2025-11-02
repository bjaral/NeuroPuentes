using NeuroPuentesAPI.models;
using NeuroPuentesAPI.repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public class EntrevistaService : IEntrevistaService
    {
        private readonly IEntrevistaRepository _repo;

        public EntrevistaService(IEntrevistaRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Entrevista>> GetAllAsync() =>
            await _repo.GetAllAsync();

        public async Task<IEnumerable<Entrevista>> GetByUsuarioIdAsync(int usuarioId) =>
            await _repo.GetByUsuarioIdAsync(usuarioId);

        public async Task<Entrevista?> GetByIdAsync(int id) =>
            await _repo.GetByIdAsync(id);

        public async Task<int> CrearAsync(Entrevista entrevista)
        {
            entrevista.FechaCreacion = DateTime.UtcNow;
            return await _repo.CrearAsync(entrevista);
        }

        public async Task ActualizarAsync(int id, Entrevista entrevista)
        {
            var existente = await _repo.GetByIdAsync(id);
            if (existente == null)
                throw new KeyNotFoundException($"Entrevista con id {id} no encontrada.");

            existente.Titulo = entrevista.Titulo ?? existente.Titulo;
            existente.Descripcion = entrevista.Descripcion ?? existente.Descripcion;
            existente.DuracionMin = entrevista.DuracionMin != 0 ? entrevista.DuracionMin : existente.DuracionMin;
            existente.NumeroTurnos = entrevista.NumeroTurnos != 0 ? entrevista.NumeroTurnos : existente.NumeroTurnos;
            existente.FechaCierre = entrevista.FechaCierre ?? existente.FechaCierre;
            existente.ContextoSnapshot = entrevista.ContextoSnapshot ?? existente.ContextoSnapshot;

            await _repo.ActualizarAsync(existente);
        }

        public async Task EliminarAsync(int id) =>
            await _repo.EliminarAsync(id);
    }
}
