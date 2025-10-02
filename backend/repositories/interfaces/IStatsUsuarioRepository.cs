using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public interface IStatsUsuarioRepository
    {
        Task<IEnumerable<Stats_Usuario>> GetAllAsync();
        Task<Stats_Usuario?> GetByIdAsync(int id);
        Task<int> CrearAsync(Stats_Usuario stats);
        Task ActualizarAsync(Stats_Usuario stats);
        Task EliminarAsync(int id);
    }
}
