using NeuroPuentesAPI.models;
using NeuroPuentesAPI.repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public class ContextoService : IContextoService
    {
        private readonly IContextoRepository _repo;

        public ContextoService(IContextoRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Contexto>> GetAllAsync() =>
            await _repo.GetAllAsync();

        public async Task<IEnumerable<Contexto>> GetAllVigentesAsync() =>
            await _repo.GetAllVigentesAsync();

        public async Task<Contexto?> GetByIdAsync(int id) =>
            await _repo.GetByIdAsync(id);

        public async Task<int> CrearAsync(Contexto contexto)
        {
            // Puedes forzar la fecha de creación aquí si no viene del cliente
            if (contexto.FechaCreacion == default)
                contexto.FechaCreacion = DateTime.UtcNow;

            return await _repo.CrearAsync(contexto);
        }

        public async Task ActualizarAsync(int id, Contexto contexto)
        {
            var existente = await _repo.GetByIdAsync(id);
            if (existente == null)
                throw new KeyNotFoundException($"Contexto con id {id} no encontrado.");

            // Solo actualiza lo que viene con valor, si quieres permitir parches
            existente.Nombre = string.IsNullOrWhiteSpace(contexto.Nombre) ? existente.Nombre : contexto.Nombre;
            existente.Descripcion = string.IsNullOrWhiteSpace(contexto.Descripcion) ? existente.Descripcion : contexto.Descripcion;
            existente.Scope = contexto.Scope;
            existente.CreadoPor = contexto.CreadoPor ?? existente.CreadoPor;
            existente.Origen = contexto.Origen;
            existente.PromptSeed = contexto.PromptSeed ?? existente.PromptSeed;
            existente.Vigencia = contexto.Vigencia;
            existente.FechaCreacion = existente.FechaCreacion; // no se toca

            await _repo.ActualizarAsync(existente);
        }
        public async Task EliminarAsync(int id) =>
            await _repo.EliminarAsync(id);
    }
}
