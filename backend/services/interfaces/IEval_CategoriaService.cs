using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public interface IEvalCategoriaService
    {
        Task<IEnumerable<Eval_Categoria>> GetAllAsync();
        Task<Eval_Categoria?> GetByIdAsync(int id);
        Task<int> CrearAsync(Eval_Categoria evalCategoria);
        Task ActualizarAsync(int id, Eval_Categoria evalCategoria);
        Task EliminarAsync(int id);

        // obtener eval_categorias dada un entrevistaId
        Task<IEnumerable<Eval_Categoria>> GetByEntrevistaIdAsync(int entrevistaId);

    }
}
