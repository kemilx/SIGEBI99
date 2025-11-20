using SIGEBI.Web.ApiDtos;

namespace SIGEBI.Web.ApiClients
{
    public interface ILibroApiClient
    {
        Task<IReadOnlyList<LibroDto>> BuscarAsync(string? titulo, string? autor, CancellationToken ct = default);
        Task<LibroDto> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
        Task<LibroDto> CrearAsync(CreateUpdateLibroDto dto, CancellationToken ct = default);
        Task ActualizarAsync(Guid id, CreateUpdateLibroDto dto, CancellationToken ct = default);
        Task ActualizarUbicacionAsync(Guid id, string? ubicacion, CancellationToken ct = default);
        // Si más adelante quieres, aquí podrías poner también CambiarEstadoAsync(...)
    }
}
