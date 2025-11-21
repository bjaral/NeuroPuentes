using NeuroPuentesAPI.models;
using NeuroPuentesAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public interface IStatsUsuarioService
    {
        // default crud
        Task<IEnumerable<Stats_Usuario>> GetAllAsync();
        Task<Stats_Usuario?> GetByIdAsync(int id);
        Task<int> CrearAsync(Stats_Usuario stats);
        Task ActualizarAsync(int id, Stats_Usuario stats);
        Task EliminarAsync(int id);

        // extraer por usuario id
        Task<IEnumerable<Stats_Usuario>> GetByUsuarioIdAsync(int usuarioId); 

        //extra por usuario id y rango de fecha.
        Task<StatsUsuarioResumenDto?> GetResumenPorUsuarioAsync(int usuarioId, DateTime fechaInicio, DateTime fechaFin);

    }

}
