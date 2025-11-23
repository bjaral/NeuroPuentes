using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public interface IContextoService
    {
        Task<IEnumerable<Contexto>> GetAllAsync();
        Task<IEnumerable<Contexto>> GetAllVigentesAsync();
        Task<Contexto?> GetByIdAsync(int id);
        Task<int> CrearAsync(Contexto contexto);
        Task ActualizarAsync(int id, Contexto contexto);
        Task EliminarAsync(int id);

        // obtener contexto por entrevistaId
        Task<Contexto?> GetByEntrevistaIdAsync(int entrevistaId);
    }
}
