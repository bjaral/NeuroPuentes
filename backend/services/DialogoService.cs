using NeuroPuentesAPI.models;
using NeuroPuentesAPI.repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public class DialogoService : IDialogoService
    {
        private readonly IDialogoRepository _repo;

        public DialogoService(IDialogoRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Dialogo>> GetAllAsync() =>
            await _repo.GetAllAsync();

        public async Task<IEnumerable<Dialogo>> GetByEntrevistaIdAsync(int entrevistaId) =>
            await _repo.GetByEntrevistaIdAsync(entrevistaId);

        public async Task<Dialogo?> GetByIdAsync(int id) =>
            await _repo.GetByIdAsync(id);

        public async Task<int> CrearAsync(Dialogo dialogo)
        {
            if (dialogo.Timestamp == default)
            {
                dialogo.Timestamp = DateTime.UtcNow;
            }
            return await _repo.CrearAsync(dialogo);
        }

        public async Task ActualizarAsync(int id, Dialogo dialogo)
        {
            var existente = await _repo.GetByIdAsync(id);
            if (existente is null)
                throw new KeyNotFoundException($"Diálogo con id {id} no encontrado.");
            
            // Lógica de 'develop' (adaptada)
            existente.Turno = dialogo.Turno != 0 ? dialogo.Turno : existente.Turno;
            // 'Sender' es un ENUM, no puede ser null, la lógica de '??' falla
            existente.Sender = dialogo.Sender; 
            existente.Texto = dialogo.Texto ?? existente.Texto;
            existente.TextoProcesado = dialogo.TextoProcesado ?? existente.TextoProcesado;
            existente.AudioUrl = dialogo.AudioUrl ?? existente.AudioUrl;

            await _repo.ActualizarAsync(existente);
        }

        public async Task EliminarAsync(int id)
        {
            var existente = await _repo.GetByIdAsync(id);
            if (existente is null)
                throw new KeyNotFoundException($"Diálogo con id {id} no encontrado.");

            await _repo.EliminarAsync(id);
        }
    }
}