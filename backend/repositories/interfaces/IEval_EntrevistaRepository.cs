using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public interface IEval_EntrevistaRepository
    {
        Task<IEnumerable<Eval_Entrevista>> GetAllAsync();
        Task<Eval_Entrevista?> GetByIdAsync(int id);
        Task CrearAsync(Eval_Entrevista eval);
        Task ActualizarAsync(Eval_Entrevista eval);
        Task EliminarAsync(int id);
        Task<Eval_Entrevista?> GetByEntrevistaIdAsync(int entrevistaId);
    }
}
