using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public interface ITipService
    {
        Task<IEnumerable<Tip>> GetAllAsync();
        Task<IEnumerable<Tip>> GetAllVigentesAsync();
        Task<Tip?> GetByIdAsync(int id);
        Task<int> CrearAsync(Tip tip);
        Task ActualizarAsync(int id, Tip tip);
        Task EliminarAsync(int id);
        Task<IEnumerable<Tip>> GetByEstudianteIdAsync(int estudianteId);
        Task<IEnumerable<Tip>> GetByEntrevistaIdAsync(int entrevistaId);
    }
}
