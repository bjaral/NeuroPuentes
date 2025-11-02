using NeuroPuentesAPI.models;
using NeuroPuentesAPI.repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public class CaracteristicaService : ICaracteristicaService
    {
        private readonly ICaracteristicaRepository _repo;

        public CaracteristicaService(ICaracteristicaRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Caracteristica>> GetAllAsync() =>
            await _repo.GetAllAsync();

        public async Task<IEnumerable<Caracteristica>> GetAllVigentesAsync() =>
            await _repo.GetAllVigentesAsync();

        public async Task<Caracteristica?> GetByIdAsync(int id) =>
            await _repo.GetByIdAsync(id);

        public async Task<int> CrearAsync(Caracteristica caracteristica) =>
            await _repo.CrearAsync(caracteristica);

        public async Task ActualizarAsync(int id, Caracteristica caracteristica)
        {
            var existente = await _repo.GetByIdAsync(id);
            if (existente == null)
                throw new KeyNotFoundException($"Característica con id {id} no encontrada.");

            existente.Nombre = string.IsNullOrWhiteSpace(caracteristica.Nombre) ? existente.Nombre : caracteristica.Nombre;
            existente.Descripcion = string.IsNullOrWhiteSpace(caracteristica.Descripcion) ? existente.Descripcion : caracteristica.Descripcion;
            existente.Grupo = string.IsNullOrWhiteSpace(caracteristica.Grupo) ? existente.Grupo : caracteristica.Grupo;
            existente.Vigencia = caracteristica.Vigencia;

            await _repo.ActualizarAsync(existente);
        }

        public async Task EliminarAsync(int id) =>
            await _repo.EliminarAsync(id);
        // public async Task<IEnumerable<Caracteristica>> GetByContextoIdAsync(int contextoId)
        // {
        //     var caracteristicas = await _repo.GetByContextoIdAsync(contextoId);
        //     return caracteristicas.Select(c => new Caracteristica
        //     {
        //         Id = c.Id,
        //         Nombre = c.Nombre,
        //         Descripcion = c.Descripcion,
        //         Grupo = c.Grupo,
        //         Vigencia = c.Vigencia
        //     });
        // }

        public async Task<IEnumerable<Caracteristica>> GetByContextoIdAsync(int contextoId) =>
            await _repo.GetByContextoIdAsync(contextoId);
    }
}