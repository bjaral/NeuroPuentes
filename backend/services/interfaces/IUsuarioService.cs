using NeuroPuentesAPI.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public interface IUsuarioService
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<IEnumerable<Usuario>> GetAllVigentesAsync();
        Task<Usuario?> GetByIdAsync(int id);
        Task<int> CrearAsync(Usuario usuario);
        Task<Usuario?> GetByCorreoAsync(string correo);
        Task ActualizarAsync(int id, Usuario usuario);
        Task EliminarAsync(int id);
        Task<Usuario?> AuthenticateAsync(string usuarioNombre, string plainPassword);
        Task<Usuario?> AuthenticateByCorreoAsync(string correo, string plainPassword);
    }
}