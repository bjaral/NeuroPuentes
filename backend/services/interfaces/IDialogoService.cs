using NeuroPuentesAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public interface IDialogoService
    {
        Task<IEnumerable<DialogoDto>> GetAllAsync();
        Task<DialogoDto?> GetByIdAsync(int id);
        Task CrearAsync(DialogoCreateDto dto);
        Task ActualizarAsync(int id, DialogoCreateDto dto);
        Task EliminarAsync(int id);
    }
}
