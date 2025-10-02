using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public interface ITipRepository
    {
        Task<IEnumerable<Tip>> GetAllAsync();
        Task<IEnumerable<Tip>> GetAllVigentesAsync();
        Task<Tip?> GetByIdAsync(int id);
        Task<int> CrearAsync(Tip tip);
        Task ActualizarAsync(Tip tip);
        Task EliminarAsync(int id);
    }
}
