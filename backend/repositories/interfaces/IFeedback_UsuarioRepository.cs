using NeuroPuentesAPI.models;

namespace NeuroPuentesAPI.repositories
{
    public interface IFeedbackUsuarioRepository
    {
        Task<IEnumerable<Feedback_Usuario>> GetAllAsync();
        Task<Feedback_Usuario?> GetByIdAsync(int id);
        Task<int> CrearAsync(Feedback_Usuario feedback);
        Task ActualizarAsync(Feedback_Usuario feedback);
        Task EliminarAsync(int id);
    }
}
