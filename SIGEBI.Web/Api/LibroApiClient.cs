using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Web.Api;

public interface ILibroApiClient
{
    Task<IReadOnlyList<LibroDto>> BuscarAsync(string? titulo, string? autor, CancellationToken ct);
    Task<LibroDto?> ObtenerPorIdAsync(Guid id, CancellationToken ct);
    Task<LibroDto> CrearAsync(CrearLibroRequest request, CancellationToken ct);
    Task<LibroDto> ActualizarAsync(Guid id, ActualizarLibroRequest request, CancellationToken ct);
    Task<LibroDto> ActualizarUbicacionAsync(Guid id, ActualizarUbicacionRequest request, CancellationToken ct);
}

public class LibroApiClient : ApiClientBase, ILibroApiClient
{
    public LibroApiClient(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<IReadOnlyList<LibroDto>> BuscarAsync(string? titulo, string? autor, CancellationToken ct)
    {
        var query = new Dictionary<string, string>();

        if (!string.IsNullOrWhiteSpace(titulo))
        {
            query["titulo"] = titulo;
        }

        if (!string.IsNullOrWhiteSpace(autor))
        {
            query["autor"] = autor;
        }

        var url = QueryHelpers.AddQueryString("api/Libro/buscar", query);
        using var response = await HttpClient.GetAsync(url, ct);
        return await ReadAsAsync<List<LibroDto>>(response, ct);
    }

    public async Task<LibroDto?> ObtenerPorIdAsync(Guid id, CancellationToken ct)
    {
        using var response = await HttpClient.GetAsync($"api/Libro/{id}", ct);
        if (IsNotFound(response))
        {
            return null;
        }

        return await ReadAsAsync<LibroDto>(response, ct);
    }

    public async Task<LibroDto> CrearAsync(CrearLibroRequest request, CancellationToken ct)
    {
        using var response = await HttpClient.PostAsJsonAsync("api/Libro", request, SerializerOptions, ct);
        return await ReadAsAsync<LibroDto>(response, ct);
    }

    public async Task<LibroDto> ActualizarAsync(Guid id, ActualizarLibroRequest request, CancellationToken ct)
    {
        using var response = await HttpClient.PutAsJsonAsync($"api/Libro/{id}", request, SerializerOptions, ct);
        return await ReadAsAsync<LibroDto>(response, ct);
    }

    public async Task<LibroDto> ActualizarUbicacionAsync(Guid id, ActualizarUbicacionRequest request, CancellationToken ct)
    {
        using var response = await HttpClient.PutAsJsonAsync($"api/Libro/{id}/ubicacion", request, SerializerOptions, ct);
        return await ReadAsAsync<LibroDto>(response, ct);
    }
}

public record LibroDto(
    Guid Id,
    string Titulo,
    string Autor,
    string? Isbn,
    string? Ubicacion,
    int EjemplaresTotales,
    int EjemplaresDisponibles,
    EstadoLibro Estado,
    DateTime? FechaPublicacion,
    DateTime CreadoUtc,
    DateTime? ActualizadoUtc);

public record CrearLibroRequest(
    string Titulo,
    string Autor,
    int EjemplaresTotales,
    string? Isbn,
    string? Ubicacion,
    DateTime? FechaPublicacionUtc);

public record ActualizarLibroRequest(
    string? Titulo,
    string? Autor,
    string? Isbn,
    DateTime? FechaPublicacionUtc);

public record ActualizarUbicacionRequest(string? Ubicacion);
