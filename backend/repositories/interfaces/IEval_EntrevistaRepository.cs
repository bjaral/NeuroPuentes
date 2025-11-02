using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public interface IEvalEntrevistaRepository
    {
        Task<IEnumerable<Eval_Entrevista>> GetAllAsync();
        Task<Eval_Entrevista?> GetByIdAsync(int id);
        Task<int> CrearAsync(Eval_Entrevista eval);
        Task ActualizarAsync(Eval_Entrevista eval);
        Task EliminarAsync(int id);

        //busqueda por entervista ID
        Task<Eval_Entrevista?> GetByEntrevistaIdAsync(int entrevistaId);
    }
}
