using NeuroPuentesAPI.DTOs;
using NeuroPuentesAPI.models;
using NeuroPuentesAPI.repositories;

namespace NeuroPuentesAPI.services
{
    public class Feedback_EntrevistaService : IFeedback_EntrevistaService
    {
        private readonly IFeedback_EntrevistaRepository _repo;

        public Feedback_EntrevistaService(IFeedback_EntrevistaRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Feedback_EntrevistaDto>> GetAllAsync()
        {
            var lista = await _repo.GetAllAsync();
            return lista.Select(f => new Feedback_EntrevistaDto
            {
                Id = f.Id,
                EntrevistaId = f.EntrevistaId,
                Tipo = f.Tipo,
                Mensaje = f.Mensaje,
                Categoria = f.Categoria,
                Fecha = f.Fecha
            });
        }

        public async Task<Feedback_EntrevistaDto?> GetByIdAsync(int id)
        {
            var f = await _repo.GetByIdAsync(id);
            if (f == null) return null;
            return new Feedback_EntrevistaDto
            {
                Id = f.Id,
                EntrevistaId = f.EntrevistaId,
                Tipo = f.Tipo,
                Mensaje = f.Mensaje,
                Categoria = f.Categoria,
                Fecha = f.Fecha
            };
        }

        public async Task CrearAsync(Feedback_EntrevistaCreateDto dto)
        {
            var f = new Feedback_Entrevista
            {
                EntrevistaId = dto.EntrevistaId,
                Tipo = dto.Tipo,
                Mensaje = dto.Mensaje,
                Categoria = dto.Categoria,
                Fecha = DateTime.UtcNow
            };
            await _repo.CrearAsync(f);
        }

        public async Task ActualizarAsync(int id, Feedback_EntrevistaCreateDto dto)
        {
            var f = await _repo.GetByIdAsync(id);
            if (f == null) throw new KeyNotFoundException("Feedback_Entrevista no encontrado");

            f.EntrevistaId = dto.EntrevistaId;
            f.Tipo = dto.Tipo;
            f.Mensaje = dto.Mensaje;
            f.Categoria = dto.Categoria;

            await _repo.ActualizarAsync(f);
        }

        public async Task EliminarAsync(int id)
        {
            await _repo.EliminarAsync(id);
        }
    }
}
