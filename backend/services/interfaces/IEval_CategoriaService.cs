using NeuroPuentesAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public interface IEval_CategoriaService
    {
        Task<IEnumerable<Eval_CategoriaDto>> GetAllAsync();
        Task<Eval_CategoriaDto?> GetByIdAsync(int id);
        Task CrearAsync(Eval_CategoriaCreateDto dto);
        Task ActualizarAsync(int id, Eval_CategoriaCreateDto dto);
        Task EliminarAsync(int id);
        Task<IEnumerable<Eval_CategoriaDto>> GetByEntrevistaIdAsync(int entrevistaId);
    }
}
