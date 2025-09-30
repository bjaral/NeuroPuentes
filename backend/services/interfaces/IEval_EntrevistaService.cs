using NeuroPuentesAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public interface IEval_EntrevistaService
    {
        Task<IEnumerable<Eval_EntrevistaDto>> GetAllAsync();
        Task<Eval_EntrevistaDto?> GetByIdAsync(int id);
        Task CrearAsync(Eval_EntrevistaCreateDto dto);
        Task ActualizarAsync(int id, Eval_EntrevistaCreateDto dto);
        Task EliminarAsync(int id);
    }
}
