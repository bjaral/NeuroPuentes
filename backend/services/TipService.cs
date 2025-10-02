using NeuroPuentesAPI.models;
using NeuroPuentesAPI.repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public class TipService : ITipService
    {
        private readonly ITipRepository _repo;

        public TipService(ITipRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Tip>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<IEnumerable<Tip>> GetAllVigentesAsync() => await _repo.GetAllVigentesAsync();

        public async Task<Tip?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

        public async Task<int> CrearAsync(Tip tip)
        {
            if (tip.Fecha == default)
                tip.Fecha = DateTime.UtcNow;
            return await _repo.CrearAsync(tip);
        }

        public async Task ActualizarAsync(int id, Tip tip)
        {
            var existente = await _repo.GetByIdAsync(id);
            if (existente == null)
                throw new KeyNotFoundException($"Tip con id {id} no encontrado.");

            existente.UsuarioId = tip.UsuarioId;
            existente.EntrevistaId = tip.EntrevistaId;
            existente.Titulo = tip.Titulo ?? existente.Titulo;
            existente.Contenido = tip.Contenido ?? existente.Contenido;
            existente.Categoria = tip.Categoria ?? existente.Categoria;
            existente.Fecha = tip.Fecha;
            existente.Usado = tip.Usado;
            existente.Vigencia = tip.Vigencia;

            await _repo.ActualizarAsync(existente);
        }

        public async Task EliminarAsync(int id) => await _repo.EliminarAsync(id);
    }
}
