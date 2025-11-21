using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public interface IDialogoRepository
    {
        Task<IEnumerable<Dialogo>> GetAllAsync();
        Task<Dialogo?> GetByIdAsync(int id);
        Task<int> CrearAsync(Dialogo dialogo);
        Task ActualizarAsync(Dialogo dialogo);
        Task EliminarAsync(int id);

        // dialogos por entrevista
        Task<IEnumerable<Dialogo>> GetByEntrevistaIdAsync(int entrevistaId);
    }
}
