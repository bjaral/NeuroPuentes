using NeuroPuentesAPI.models;
using NeuroPuentesAPI.repositories;

namespace NeuroPuentesAPI.services
{
    public class FeedbackUsuarioService : IFeedbackUsuarioService
    {
        private readonly IFeedbackUsuarioRepository _repo;

        public FeedbackUsuarioService(IFeedbackUsuarioRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Feedback_Usuario>> GetAllAsync() =>
            await _repo.GetAllAsync();

        public async Task<Feedback_Usuario?> GetByIdAsync(int id) =>
            await _repo.GetByIdAsync(id);

        public async Task<int> CrearAsync(Feedback_Usuario feedback)
        {
            if (feedback.Fecha == default)
                feedback.Fecha = DateTime.UtcNow;

            return await _repo.CrearAsync(feedback);
        }

        public async Task ActualizarAsync(int id, Feedback_Usuario feedback)
        {
            var existente = await _repo.GetByIdAsync(id);
            if (existente == null)
                throw new KeyNotFoundException($"Feedback con id {id} no encontrado.");

            existente.Tipo = feedback.Tipo;
            existente.Mensaje = string.IsNullOrWhiteSpace(feedback.Mensaje) ? existente.Mensaje : feedback.Mensaje;
            existente.Categoria = string.IsNullOrWhiteSpace(feedback.Categoria) ? existente.Categoria : feedback.Categoria;
            existente.UsuarioId = feedback.UsuarioId;

            await _repo.ActualizarAsync(existente);
        }

        public async Task EliminarAsync(int id) =>
            await _repo.EliminarAsync(id);
    }
}
