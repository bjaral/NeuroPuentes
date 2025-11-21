using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.repositories
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<IEnumerable<Usuario>> GetAllVigentesAsync();
        Task<Usuario?> GetByIdAsync(int id);
        Task<int> CrearAsync(Usuario usuario);
        Task ActualizarAsync(Usuario usuario);
        Task EliminarAsync(int id);

        Task<Usuario?> GetByCorreoAsync(string correo);
        Task<Usuario?> GetByUsuarioNombreAsync(string usuarioNombre);
    }
}