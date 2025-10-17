using NeuroPuentesAPI.DTOs;
using NeuroPuentesAPI.models;
using NeuroPuentesAPI.repositories;

namespace NeuroPuentesAPI.services
{
    public class DialogoService : IDialogoService
    {
        private readonly IDialogoRepository _repo;

        public DialogoService(IDialogoRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<DialogoDto>> GetAllAsync()
        {
            var lista = await _repo.GetAllAsync();
            return lista.Select(d => new DialogoDto
            {
                Id = d.Id,
                EntrevistaId = d.EntrevistaId,
                Turno = d.Turno,
                Sender = d.Sender,
                Texto = d.Texto,
                TextoProcesado = d.TextoProcesado,
                Timestamp = d.Timestamp,
                AudioUrl = d.AudioUrl
            });
        }

        public async Task<DialogoDto?> GetByIdAsync(int id)
        {
            var d = await _repo.GetByIdAsync(id);
            if (d == null) return null;
            return new DialogoDto
            {
                Id = d.Id,
                EntrevistaId = d.EntrevistaId,
                Turno = d.Turno,
                Sender = d.Sender,
                Texto = d.Texto,
                TextoProcesado = d.TextoProcesado,
                Timestamp = d.Timestamp,
                AudioUrl = d.AudioUrl
            };
        }

        public async Task CrearAsync(DialogoCreateDto dto)
        {
            var d = new Dialogo
            {
                EntrevistaId = dto.EntrevistaId,
                Turno = dto.Turno,
                Sender = dto.Sender,
                Texto = dto.Texto,
                TextoProcesado = dto.TextoProcesado,
                AudioUrl = dto.AudioUrl,
                Timestamp = DateTime.UtcNow
            };
            await _repo.CrearAsync(d);
        }

        public async Task ActualizarAsync(int id, DialogoCreateDto dto)
        {
            var d = await _repo.GetByIdAsync(id);
            if (d == null) throw new KeyNotFoundException("Dialogo no encontrado");

            d.Turno = dto.Turno;
            d.Sender = dto.Sender;
            d.Texto = dto.Texto;
            d.TextoProcesado = dto.TextoProcesado;
            d.AudioUrl = dto.AudioUrl;

            await _repo.ActualizarAsync(d);
        }

        public async Task EliminarAsync(int id)
        {
            await _repo.EliminarAsync(id);
        }
        public async Task<IEnumerable<DialogoDto>> GetByEntrevistaIdAsync(int entrevistaId)
        {
            var dialogos = await _repo.GetByEntrevistaIdAsync(entrevistaId);
            return dialogos.Select(d => new DialogoDto
            {
                Id = d.Id,
                EntrevistaId = d.EntrevistaId,
                Turno = d.Turno,
                Sender = d.Sender,
                Texto = d.Texto,
                TextoProcesado = d.TextoProcesado,
                Timestamp = d.Timestamp,
                AudioUrl = d.AudioUrl
            });
        }
    }
}
