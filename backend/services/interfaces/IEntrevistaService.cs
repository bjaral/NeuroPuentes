using NeuroPuentesAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public interface IEntrevistaService
    {
        Task<IEnumerable<EntrevistaDto>> GetAllAsync();
        Task<EntrevistaDto?> GetByIdAsync(int id);
        Task CrearAsync(EntrevistaCreateDto dto);
        Task ActualizarAsync(int id, EntrevistaCreateDto dto);
        Task EliminarAsync(int id);
        Task<IEnumerable<EntrevistaDto>> GetByEstudianteIdAsync(int estudianteId);
    }
}
