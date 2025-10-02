using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public interface ICaracteristicaService
    {
        Task<IEnumerable<Caracteristica>> GetAllAsync();
        Task<IEnumerable<Caracteristica>> GetAllVigentesAsync();
        Task<Caracteristica?> GetByIdAsync(int id);
        Task<int> CrearAsync(Caracteristica caracteristica);
        Task ActualizarAsync(int id, Caracteristica caracteristica);
        Task EliminarAsync(int id);
    }
}
