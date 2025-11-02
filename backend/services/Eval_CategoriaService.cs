using NeuroPuentesAPI.models;
using NeuroPuentesAPI.repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public class EvalCategoriaService : IEvalCategoriaService
    {
        private readonly IEvalCategoriaRepository _repo;

        public EvalCategoriaService(IEvalCategoriaRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Eval_Categoria>> GetAllAsync() =>
            await _repo.GetAllAsync();



        public async Task<Eval_Categoria?> GetByIdAsync(int id) =>
            await _repo.GetByIdAsync(id);

        public async Task<int> CrearAsync(Eval_Categoria evalCategoria) =>
            await _repo.CrearAsync(evalCategoria);

        public async Task ActualizarAsync(int id, Eval_Categoria evalCategoria)
        {
            var existente = await _repo.GetByIdAsync(id);
            if (existente == null)
                throw new KeyNotFoundException($"Eval_Categoria con id {id} no encontrada.");

            existente.EvalEntrevistaId = evalCategoria.EvalEntrevistaId;
            existente.Categoria = evalCategoria.Categoria;
            existente.Score = evalCategoria.Score;

            await _repo.ActualizarAsync(existente);
        }

        public async Task EliminarAsync(int id) =>
            await _repo.EliminarAsync(id);



        // busqueda por entrevistaId
        public async Task<IEnumerable<Eval_Categoria>> GetByEntrevistaIdAsync(int entrevistaId) =>
            await _repo.GetByEntrevistaIdAsync(entrevistaId);
    }
}
