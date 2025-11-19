using System.Net.Http.Json;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Web.Api;

public interface IPrestamoApiClient
{
    Task<PrestamoDto?> ObtenerPorIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<PrestamoDto>> ObtenerPorUsuarioAsync(Guid usuarioId, CancellationToken ct);
    Task<PrestamoDto> CrearAsync(CrearPrestamoRequest request, CancellationToken ct);
}

public class PrestamoApiClient : ApiClientBase, IPrestamoApiClient
{
    public PrestamoApiClient(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<PrestamoDto?> ObtenerPorIdAsync(Guid id, CancellationToken ct)
    {
        using var response = await HttpClient.GetAsync($"api/Prestamo/{id}", ct);
        if (IsNotFound(response))
        {
            return null;
        }

        return await ReadAsAsync<PrestamoDto>(response, ct);
    }

    public async Task<IReadOnlyList<PrestamoDto>> ObtenerPorUsuarioAsync(Guid usuarioId, CancellationToken ct)
    {
        using var response = await HttpClient.GetAsync($"api/Prestamo/usuario/{usuarioId}", ct);
        return await ReadAsAsync<List<PrestamoDto>>(response, ct);
    }

    public async Task<PrestamoDto> CrearAsync(CrearPrestamoRequest request, CancellationToken ct)
    {
        using var response = await HttpClient.PostAsJsonAsync("api/Prestamo", request, SerializerOptions, ct);
        return await ReadAsAsync<PrestamoDto>(response, ct);
    }
}

public record PrestamoDto(
    Guid Id,
    Guid LibroId,
    Guid UsuarioId,
    EstadoPrestamo Estado,
    DateTime FechaInicioUtc,
    DateTime FechaFinCompromisoUtc,
    DateTime? FechaEntregaRealUtc,
    string? Observaciones,
    DateTime CreadoUtc,
    DateTime? ActualizadoUtc);

public record CrearPrestamoRequest(
    Guid LibroId,
    Guid UsuarioId,
    DateTime FechaInicioUtc,
    DateTime FechaFinUtc);
