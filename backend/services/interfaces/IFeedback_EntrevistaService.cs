using NeuroPuentesAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public interface IFeedback_EntrevistaService
    {
        Task<IEnumerable<Feedback_EntrevistaDto>> GetAllAsync();
        Task<Feedback_EntrevistaDto?> GetByIdAsync(int id);
        Task CrearAsync(Feedback_EntrevistaCreateDto dto);
        Task ActualizarAsync(int id, Feedback_EntrevistaCreateDto dto);
        Task EliminarAsync(int id);
    }
}
