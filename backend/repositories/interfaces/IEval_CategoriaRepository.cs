using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public interface IEval_CategoriaRepository
    {
        Task<IEnumerable<Eval_Categoria>> GetAllAsync();
        Task<Eval_Categoria?> GetByIdAsync(int id);
        Task CrearAsync(Eval_Categoria categoria);
        Task ActualizarAsync(Eval_Categoria categoria);
        Task EliminarAsync(int id);
    }
}
