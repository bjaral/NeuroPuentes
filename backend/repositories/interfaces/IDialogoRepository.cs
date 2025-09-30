using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public interface IDialogoRepository
    {
        Task<IEnumerable<Dialogo>> GetAllAsync();
        Task<Dialogo?> GetByIdAsync(int id);
        Task CrearAsync(Dialogo dialogo);
        Task ActualizarAsync(Dialogo dialogo);
        Task EliminarAsync(int id);
    }
}
