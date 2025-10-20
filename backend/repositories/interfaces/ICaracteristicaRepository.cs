using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public interface ICaracteristicaRepository
    {
        Task<IEnumerable<Caracteristica>> GetAllAsync();
        Task<IEnumerable<Caracteristica>> GetAllVigentesAsync();
        Task<Caracteristica?> GetByIdAsync(int id);
        Task<int> CrearAsync(Caracteristica caracteristica);
        Task ActualizarAsync(Caracteristica caracteristica);
        Task EliminarAsync(int id);
        Task<IEnumerable<Caracteristica>> GetByContextoIdAsync(int contextoId);
    }
}
