using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public interface IContextoRepository
    {
        Task<IEnumerable<Contexto>> GetAllAsync();
        Task<IEnumerable<Contexto>> GetAllVigentesAsync();
        Task<Contexto?> GetByIdAsync(int id);
        Task<int> CrearAsync(Contexto contexto);
        Task ActualizarAsync(Contexto contexto);
        Task EliminarAsync(int id);
    }
}
