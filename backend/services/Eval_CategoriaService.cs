using NeuroPuentesAPI.DTOs;
using NeuroPuentesAPI.models;
using NeuroPuentesAPI.repositories;

namespace NeuroPuentesAPI.services
{
    public class Eval_CategoriaService : IEval_CategoriaService
    {
        private readonly IEval_CategoriaRepository _repo;

        public Eval_CategoriaService(IEval_CategoriaRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Eval_CategoriaDto>> GetAllAsync()
        {
            var lista = await _repo.GetAllAsync();
            return lista.Select(e => new Eval_CategoriaDto
            {
                Id = e.Id,
                EvalEntrevistaId = e.EvalEntrevistaId,
                Categoria = e.Categoria,
                Score = e.Score
            });
        }

        public async Task<Eval_CategoriaDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            return new Eval_CategoriaDto
            {
                Id = e.Id,
                EvalEntrevistaId = e.EvalEntrevistaId,
                Categoria = e.Categoria,
                Score = e.Score
            };
        }

        public async Task CrearAsync(Eval_CategoriaCreateDto dto)
        {
            var e = new Eval_Categoria
            {
                EvalEntrevistaId = dto.EvalEntrevistaId,
                Categoria = dto.Categoria,
                Score = dto.Score
            };
            await _repo.CrearAsync(e);
        }

        public async Task ActualizarAsync(int id, Eval_CategoriaCreateDto dto)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) throw new KeyNotFoundException("Eval_Categoria no encontrado");

            e.Categoria = dto.Categoria;
            e.Score = dto.Score;
            e.EvalEntrevistaId = dto.EvalEntrevistaId;

            await _repo.ActualizarAsync(e);
        }

        public async Task EliminarAsync(int id)
        {
            await _repo.EliminarAsync(id);
        }
        public async Task<IEnumerable<Eval_CategoriaDto>> GetByEntrevistaIdAsync(int entrevistaId)
        {
            var categorias = await _repo.GetByEntrevistaIdAsync(entrevistaId);
            return categorias.Select(c => new Eval_CategoriaDto
            {
                Id = c.Id,
                EvalEntrevistaId = c.EvalEntrevistaId,
                Categoria = c.Categoria,
                Score = c.Score
            });
        }
    }
}
