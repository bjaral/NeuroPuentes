using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public interface ICaractsRelService
    {
        Task<IEnumerable<Caracts_Rel>> GetAllAsync();
        Task<Caracts_Rel?> GetByIdsAsync(int caracteristicaId, int contextoId);
        Task CrearAsync(Caracts_Rel rel);
        Task EliminarAsync(int caracteristicaId, int contextoId);
    }
}
