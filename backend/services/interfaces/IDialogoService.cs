using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public interface IDialogoService
    {
        Task<IEnumerable<Dialogo>> GetAllAsync();
        Task<Dialogo?> GetByIdAsync(int id);
        Task<int> CrearAsync(Dialogo dialogo); // <-- Devuelve int
        Task ActualizarAsync(int id, Dialogo dialogo);
        Task EliminarAsync(int id);
        Task<IEnumerable<Dialogo>> GetByEntrevistaIdAsync(int entrevistaId);
    }
}