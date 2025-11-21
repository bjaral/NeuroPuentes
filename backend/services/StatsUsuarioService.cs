using NeuroPuentesAPI.models;
using NeuroPuentesAPI.DTOs;
using NeuroPuentesAPI.repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public class StatsUsuarioService : IStatsUsuarioService
    {
        private readonly IStatsUsuarioRepository _repo;

        public StatsUsuarioService(IStatsUsuarioRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Stats_Usuario>> GetAllAsync() =>
            await _repo.GetAllAsync();

        public async Task<Stats_Usuario?> GetByIdAsync(int id) =>
            await _repo.GetByIdAsync(id);

        public async Task<int> CrearAsync(Stats_Usuario stats)
        {
            if (stats.FechaCorte == default)
                stats.FechaCorte = DateTime.UtcNow;

            return await _repo.CrearAsync(stats);
        }

        public async Task ActualizarAsync(int id, Stats_Usuario stats)
        {
            var existente = await _repo.GetByIdAsync(id);
            if (existente == null)
                throw new KeyNotFoundException($"StatsUsuario con id {id} no encontrado.");

            existente.UsuarioId = stats.UsuarioId;
            existente.FechaCorte = stats.FechaCorte;
            existente.TotalEntrevistas = stats.TotalEntrevistas;
            existente.TiempoTotalMin = stats.TiempoTotalMin;
            existente.ScorePromedio = stats.ScorePromedio;

            await _repo.ActualizarAsync(existente);
        }

        public async Task EliminarAsync(int id) =>
            await _repo.EliminarAsync(id);



        public async Task<IEnumerable<Stats_Usuario>> GetByUsuarioIdAsync(int usuarioId) =>
        await _repo.GetByUsuarioIdAsync(usuarioId);

        public async Task<StatsUsuarioResumenDto?> GetResumenPorUsuarioAsync(int usuarioId, DateTime fechaInicio, DateTime fechaFin)
        => await _repo.GetResumenPorUsuarioAsync(usuarioId, fechaInicio, fechaFin);
    }
}
