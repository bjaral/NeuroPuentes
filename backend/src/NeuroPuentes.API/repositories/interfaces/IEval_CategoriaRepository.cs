using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public interface IEvalCategoriaRepository
    {
        Task<IEnumerable<Eval_Categoria>> GetAllAsync();
        Task<Eval_Categoria?> GetByIdAsync(int id);
        Task<int> CrearAsync(Eval_Categoria evalCategoria);
        Task ActualizarAsync(Eval_Categoria evalCategoria);
        Task EliminarAsync(int id);

        //obtener eval categorias dada un entrevistaID
        Task<IEnumerable<Eval_Categoria>> GetByEntrevistaIdAsync(int entrevistaId);
    }
}
