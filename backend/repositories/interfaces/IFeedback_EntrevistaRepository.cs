using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public interface IFeedbackEntrevistaRepository
    {
        Task<IEnumerable<Feedback_Entrevista>> GetAllAsync();
        Task<Feedback_Entrevista?> GetByIdAsync(int id);
        Task<int> CrearAsync(Feedback_Entrevista feedback);
        Task ActualizarAsync(Feedback_Entrevista feedback);
        Task EliminarAsync(int id);

        // obtener by entrevista
        Task<IEnumerable<Feedback_Entrevista>> GetByEntrevistaIdAsync(int entrevistaId);
    }
}
