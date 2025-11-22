using NeuroPuentesAPI.models;
using NeuroPuentesAPI.repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public class CaractsRelService : ICaractsRelService
    {
        private readonly ICaractsRelRepository _repo;

        public CaractsRelService(ICaractsRelRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Caracts_Rel>> GetAllAsync() =>
            await _repo.GetAllAsync();

        public async Task<Caracts_Rel?> GetByIdsAsync(int caracteristicaId, int contextoId) =>
            await _repo.GetByIdsAsync(caracteristicaId, contextoId);

        public async Task CrearAsync(Caracts_Rel rel) =>
            await _repo.CrearAsync(rel);

        public async Task EliminarAsync(int caracteristicaId, int contextoId) =>
            await _repo.EliminarAsync(caracteristicaId, contextoId);
    }
}
