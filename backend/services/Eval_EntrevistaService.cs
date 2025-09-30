using NeuroPuentesAPI.DTOs;
using NeuroPuentesAPI.models;
using NeuroPuentesAPI.repositories;

namespace NeuroPuentesAPI.services
{
    public class Eval_EntrevistaService : IEval_EntrevistaService
    {
        private readonly IEval_EntrevistaRepository _repo;

        public Eval_EntrevistaService(IEval_EntrevistaRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Eval_EntrevistaDto>> GetAllAsync()
        {
            var lista = await _repo.GetAllAsync();
            return lista.Select(e => new Eval_EntrevistaDto
            {
                Id = e.Id,
                EntrevistaId = e.EntrevistaId,
                ScoreFinal = e.ScoreFinal
            });
        }

        public async Task<Eval_EntrevistaDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            return new Eval_EntrevistaDto
            {
                Id = e.Id,
                EntrevistaId = e.EntrevistaId,
                ScoreFinal = e.ScoreFinal
            };
        }

        public async Task CrearAsync(Eval_EntrevistaCreateDto dto)
        {
            var e = new Eval_Entrevista
            {
                EntrevistaId = dto.EntrevistaId,
                ScoreFinal = dto.ScoreFinal
            };
            await _repo.CrearAsync(e);
        }

        public async Task ActualizarAsync(int id, Eval_EntrevistaCreateDto dto)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) throw new KeyNotFoundException("Eval_Entrevista no encontrado");

            e.ScoreFinal = dto.ScoreFinal;
            e.EntrevistaId = dto.EntrevistaId;

            await _repo.ActualizarAsync(e);
        }

        public async Task EliminarAsync(int id)
        {
            await _repo.EliminarAsync(id);
        }
    }
}
