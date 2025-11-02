using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public interface IEntrevistaService
    {
        Task<IEnumerable<Entrevista>> GetAllAsync();
        
        Task<Entrevista?> GetByIdAsync(int id);
        Task<int> CrearAsync(Entrevista entrevista);
        Task ActualizarAsync(int id, Entrevista entrevista);
        Task EliminarAsync(int id);

        // dar entrevistas dada un ID de usuario
        Task<IEnumerable<Entrevista>> GetByUsuarioIdAsync(int usuarioId);
    }
}
