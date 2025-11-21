using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public interface ICalificacionUsuarioRepository
    {
        Task<IEnumerable<Calificacion_Usuario>> GetAllAsync();
        Task<Calificacion_Usuario?> GetByIdAsync(int id);
        Task<int> CrearAsync(Calificacion_Usuario calificacion);
        Task ActualizarAsync(Calificacion_Usuario calificacion);
        Task EliminarAsync(int id);

        //obtener todas las calificaciones por usuario
        Task<IEnumerable<Calificacion_Usuario>> GetByUsuarioIdAsync(int usuarioId);
    }
}
