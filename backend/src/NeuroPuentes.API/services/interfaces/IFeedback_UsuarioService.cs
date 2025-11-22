using NeuroPuentesAPI.models;

namespace NeuroPuentesAPI.services
{
    public interface IFeedbackUsuarioService
    {
        Task<IEnumerable<Feedback_Usuario>> GetAllAsync();
        Task<Feedback_Usuario?> GetByIdAsync(int id);
        Task<int> CrearAsync(Feedback_Usuario feedback);
        Task ActualizarAsync(int id, Feedback_Usuario feedback);
        Task EliminarAsync(int id);

        // obtener por usuario ID
        Task<IEnumerable<Feedback_Usuario>> GetByUsuarioIdAsync(int usuarioId);
    }
}
