using NeuroPuentesAPI.models;
using NeuroPuentesAPI.repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public class FeedbackEntrevistaService : IFeedbackEntrevistaService
    {
        private readonly IFeedbackEntrevistaRepository _repo;

        public FeedbackEntrevistaService(IFeedbackEntrevistaRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Feedback_Entrevista>> GetAllAsync() =>
            await _repo.GetAllAsync();

        public async Task<Feedback_Entrevista?> GetByIdAsync(int id) =>
            await _repo.GetByIdAsync(id);



        public async Task<int> CrearAsync(Feedback_Entrevista feedback)
        {
            if (feedback.Fecha == default)
                feedback.Fecha = DateTime.UtcNow;

            return await _repo.CrearAsync(feedback);
        }

        public async Task ActualizarAsync(int id, Feedback_Entrevista feedback)
        {
            var existente = await _repo.GetByIdAsync(id);
            if (existente == null)
                throw new KeyNotFoundException($"Feedback con id {id} no encontrado.");

            existente.EntrevistaId = feedback.EntrevistaId;
            existente.Tipo = feedback.Tipo;
            existente.Mensaje = feedback.Mensaje;
            existente.Categoria = feedback.Categoria;
            existente.Fecha = feedback.Fecha;

            await _repo.ActualizarAsync(existente);
        }

        public async Task EliminarAsync(int id) =>
            await _repo.EliminarAsync(id);


        //busqueda por entrevista
        public async Task<IEnumerable<Feedback_Entrevista>> GetByEntrevistaIdAsync(int entrevistaId) =>
            await _repo.GetByEntrevistaIdAsync(entrevistaId);

    }
}
