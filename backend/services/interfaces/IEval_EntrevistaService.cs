using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public interface IEvalEntrevistaService
    {
        Task<IEnumerable<Eval_Entrevista>> GetAllAsync();
        Task<Eval_Entrevista?> GetByIdAsync(int id);
        Task<int> CrearAsync(Eval_Entrevista eval);
        Task ActualizarAsync(int id, Eval_Entrevista eval);
        Task EliminarAsync(int id);

        //busqueda por entrevista ID
        Task<Eval_Entrevista?> GetByEntrevistaIdAsync(int entrevistaId);
    }
}
