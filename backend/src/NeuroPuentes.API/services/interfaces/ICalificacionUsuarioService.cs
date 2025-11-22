using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public interface ICalificacionUsuarioService
    {
        Task<IEnumerable<Calificacion_Usuario>> GetAllAsync();
        Task<Calificacion_Usuario?> GetByIdAsync(int id);
        Task<int> CrearAsync(Calificacion_Usuario calificacion);
        Task ActualizarAsync(int id, Calificacion_Usuario calificacion);
        Task EliminarAsync(int id);

        // obtener todas las calificaciones por usuario
        Task<IEnumerable<Calificacion_Usuario>> GetByUsuarioIdAsync(int usuarioId);
    }
}
