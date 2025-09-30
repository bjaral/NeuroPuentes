using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public interface IEntrevistaRepository
    {
        Task<IEnumerable<Entrevista>> GetAllAsync();
        Task<Entrevista?> GetByIdAsync(int id);
        Task CrearAsync(Entrevista entrevista);
        Task ActualizarAsync(Entrevista entrevista);
        Task EliminarAsync(int id);
    }
}
