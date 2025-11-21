using NeuroPuentesAPI.models;
using NeuroPuentesAPI.repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public class EvalEntrevistaService : IEvalEntrevistaService
    {
        private readonly IEvalEntrevistaRepository _repo;

        public EvalEntrevistaService(IEvalEntrevistaRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Eval_Entrevista>> GetAllAsync() =>
            await _repo.GetAllAsync();

        public async Task<Eval_Entrevista?> GetByIdAsync(int id) =>
            await _repo.GetByIdAsync(id);

        public async Task<int> CrearAsync(Eval_Entrevista eval) =>
            await _repo.CrearAsync(eval);

        public async Task ActualizarAsync(int id, Eval_Entrevista eval)
        {
            var existente = await _repo.GetByIdAsync(id);
            if (existente == null)
                throw new KeyNotFoundException($"Evaluación con id {id} no encontrada.");

            existente.EntrevistaId = eval.EntrevistaId;
            existente.ScoreFinal = eval.ScoreFinal;

            await _repo.ActualizarAsync(existente);
        }

        public async Task EliminarAsync(int id) =>
            await _repo.EliminarAsync(id);


        // busqueda por entrevistaId
        public async Task<Eval_Entrevista?> GetByEntrevistaIdAsync(int entrevistaId) =>
            await _repo.GetByEntrevistaIdAsync(entrevistaId);
    }
}
