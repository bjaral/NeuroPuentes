using NeuroPuentesAPI.models;
using NeuroPuentesAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public interface IStatsUsuarioRepository
    {
        // default
        Task<IEnumerable<Stats_Usuario>> GetAllAsync();
        Task<Stats_Usuario?> GetByIdAsync(int id);
        Task<int> CrearAsync(Stats_Usuario stats);
        Task ActualizarAsync(Stats_Usuario stats);
        Task EliminarAsync(int id);

        // datos por usuario
        Task<IEnumerable<Stats_Usuario>> GetByUsuarioIdAsync(int usuarioId); // nuevo

        //datos por usuario y rango de fecha
        Task<StatsUsuarioResumenDto?> GetResumenPorUsuarioAsync(int usuarioId, DateTime fechaInicio, DateTime fechaFin);

    }

}
