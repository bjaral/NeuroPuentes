using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public interface IFeedbackEntrevistaService
    {
        Task<IEnumerable<Feedback_Entrevista>> GetAllAsync();
        Task<Feedback_Entrevista?> GetByIdAsync(int id);
        Task<int> CrearAsync(Feedback_Entrevista feedback);
        Task ActualizarAsync(int id, Feedback_Entrevista feedback);
        Task EliminarAsync(int id);

        //busqueda por entrevistaId
        Task<IEnumerable<Feedback_Entrevista>> GetByEntrevistaIdAsync(int entrevistaId);
    }
}
