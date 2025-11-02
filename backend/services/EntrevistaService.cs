using NeuroPuentesAPI.DTOs;
using NeuroPuentesAPI.models;
using NeuroPuentesAPI.repositories;

namespace NeuroPuentesAPI.services
{
    public class EntrevistaService : IEntrevistaService
    {
        private readonly IEntrevistaRepository _repo;

        public EntrevistaService(IEntrevistaRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<EntrevistaDto>> GetAllAsync()
        {
            var entrevistas = await _repo.GetAllAsync();
            return entrevistas.Select(e => new EntrevistaDto
            {
                Id = e.Id,
                UsuarioId = e.UsuarioId,
                ContextoId = e.ContextoId,
                Titulo = e.Titulo,
                Descripcion = e.Descripcion,
                DuracionMin = e.DuracionMin,
                NumeroTurnos = e.NumeroTurnos,
                FechaCreacion = e.FechaCreacion,
                FechaCierre = e.FechaCierre,
                ContextoSnapshot = e.ContextoSnapshot
            });
        }

        public async Task<EntrevistaDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;

            return new EntrevistaDto
            {
                Id = e.Id,
                UsuarioId = e.UsuarioId,
                ContextoId = e.ContextoId,
                Titulo = e.Titulo,
                Descripcion = e.Descripcion,
                DuracionMin = e.DuracionMin,
                NumeroTurnos = e.NumeroTurnos,
                FechaCreacion = e.FechaCreacion,
                FechaCierre = e.FechaCierre,
                ContextoSnapshot = e.ContextoSnapshot
            };
        }

        // --- ¡MODIFICACIÓN IMPORTANTE! ---
        // Ahora este método devuelve la entrevista creada, lo cual es
        // necesario para tu 'EntrevistasController' de IA.
        public async Task<EntrevistaDto> CrearAsync(EntrevistaCreateDto dto)
        {
            var e = new Entrevista
            {
                UsuarioId = dto.UsuarioId,
                ContextoId = dto.ContextoId,
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                DuracionMin = dto.DuracionMin,
                NumeroTurnos = dto.NumeroTurnos,
                ContextoSnapshot = dto.ContextoSnapshot,
                FechaCreacion = DateTime.UtcNow
            };

            // Asumimos que _repo.CrearAsync actualiza el objeto 'e' con el Id
            // (EF Core lo hace automáticamente).
            await _repo.CrearAsync(e);

            // Mapeamos la entidad 'e' (que ahora tiene Id) de vuelta a un DTO
            return new EntrevistaDto
            {
                Id = e.Id, // <-- El ID es crucial para el controlador de IA
                UsuarioId = e.UsuarioId,
                ContextoId = e.ContextoId,
                Titulo = e.Titulo,
                Descripcion = e.Descripcion,
                DuracionMin = e.DuracionMin,
                NumeroTurnos = e.NumeroTurnos,
                FechaCreacion = e.FechaCreacion,
                ContextoSnapshot = e.ContextoSnapshot
            };
        }

        public async Task ActualizarAsync(int id, EntrevistaCreateDto dto)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) throw new KeyNotFoundException("Entrevista no encontrada");

            e.Titulo = dto.Titulo;
            e.Descripcion = dto.Descripcion;
            e.DuracionMin = dto.DuracionMin;
            e.NumeroTurnos = dto.NumeroTurnos;
            e.ContextoSnapshot = dto.ContextoSnapshot;

            await _repo.ActualizarAsync(e);
        }

        public async Task EliminarAsync(int id)
        {
            await _repo.EliminarAsync(id);
        }
        
        public async Task<IEnumerable<EntrevistaDto>> GetByEstudianteIdAsync(int estudianteId)
        {
            var entrevistas = await _repo.GetByUsuarioIdAsync(estudianteId);
            return entrevistas.Select(e => new EntrevistaDto
            {
                Id = e.Id,
                UsuarioId = e.UsuarioId,
                ContextoId = e.ContextoId,
                Titulo = e.Titulo,
                Descripcion = e.Descripcion,
                DuracionMin = e.DuracionMin,
                NumeroTurnos = e.NumeroTurnos,
                FechaCreacion = e.FechaCreacion,
                FechaCierre = e.FechaCierre,
                ContextoSnapshot = e.ContextoSnapshot
            });
        }

        // --- ¡NUEVO MÉTODO AÑADIDO! ---
        // Esta es la implementación que soluciona tu error CS1061
        public async Task<EntrevistaDto?> GetLatestByUsuarioIdAsync(int estudianteId)
        {
            var entrevistas = await _repo.GetByUsuarioIdAsync(estudianteId);
            if (entrevistas == null || !entrevistas.Any())
            {
                return null; // No hay entrevistas para este usuario
            }

            // Ordena por fecha de creación descendente y toma la primera
            var e = entrevistas.OrderByDescending(ent => ent.FechaCreacion).FirstOrDefault();
            if (e == null) return null;

            // Mapea la entidad a un DTO y devuélvelo
            return new EntrevistaDto
            {
                Id = e.Id,
                UsuarioId = e.UsuarioId,
                ContextoId = e.ContextoId,
                Titulo = e.Titulo,
                Descripcion = e.Descripcion,
                DuracionMin = e.DuracionMin,
                NumeroTurnos = e.NumeroTurnos,
                FechaCreacion = e.FechaCreacion,
                FechaCierre = e.FechaCierre,
                ContextoSnapshot = e.ContextoSnapshot
            };
        }
    }
}