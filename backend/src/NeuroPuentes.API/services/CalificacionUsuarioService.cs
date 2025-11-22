using NeuroPuentesAPI.models;
using NeuroPuentesAPI.repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public class CalificacionUsuarioService : ICalificacionUsuarioService
    {
        private readonly ICalificacionUsuarioRepository _repo;

        public CalificacionUsuarioService(ICalificacionUsuarioRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Calificacion_Usuario>> GetAllAsync() =>
            await _repo.GetAllAsync();

        public async Task<Calificacion_Usuario?> GetByIdAsync(int id) =>
            await _repo.GetByIdAsync(id);

        public async Task<int> CrearAsync(Calificacion_Usuario calificacion)
        {
            if (calificacion.Fecha == default)
                calificacion.Fecha = DateTime.UtcNow;

            return await _repo.CrearAsync(calificacion);
        }

        public async Task ActualizarAsync(int id, Calificacion_Usuario calificacion)
        {
            var existente = await _repo.GetByIdAsync(id);
            if (existente == null)
                throw new KeyNotFoundException($"Calificación con id {id} no encontrada.");

            existente.UsuarioId = calificacion.UsuarioId;
            existente.Calificacion = calificacion.Calificacion;
            existente.Mensaje = calificacion.Mensaje ?? existente.Mensaje;
            existente.Fecha = existente.Fecha; // no se cambia

            await _repo.ActualizarAsync(existente);
        }

        public async Task EliminarAsync(int id) =>
            await _repo.EliminarAsync(id);

        // obtener todas las calificaciones por usuario
        public async Task<IEnumerable<Calificacion_Usuario>> GetByUsuarioIdAsync(int usuarioId) =>
            await _repo.GetByUsuarioIdAsync(usuarioId);
    }
}
