using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public interface IStatsUsuarioService
    {
        Task<IEnumerable<Stats_Usuario>> GetAllAsync();
        Task<Stats_Usuario?> GetByIdAsync(int id);
        Task<int> CrearAsync(Stats_Usuario stats);
        Task ActualizarAsync(int id, Stats_Usuario stats);
        Task EliminarAsync(int id);
    }
}
