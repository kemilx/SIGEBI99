using System.Net;
using System.Net.Http.Json;
using SIGEBI.Web.ApiDtos;

namespace SIGEBI.Web.ApiClients
{
    public class LibroApiClient : ILibroApiClient
    {
        private readonly HttpClient _httpClient;

        public LibroApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET /api/Libro/buscar?titulo=...&autor=...
        public async Task<IReadOnlyList<LibroDto>> BuscarAsync(string? titulo, string? autor, CancellationToken ct = default)
        {
            var url = $"api/Libro/buscar?titulo={Uri.EscapeDataString(titulo ?? string.Empty)}" +
                      $"&autor={Uri.EscapeDataString(autor ?? string.Empty)}";

            var response = await _httpClient.GetAsync(url, ct);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                // Por ejemplo, cuando la API exige un criterio de búsqueda
                throw new InvalidOperationException("Debe indicar un texto de búsqueda por título o autor.");
            }

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadFromJsonAsync<List<LibroDto>>(cancellationToken: ct);
            return data ?? new List<LibroDto>();
        }

        // GET /api/Libro/{id}
        public async Task<LibroDto> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
        {
            var response = await _httpClient.GetAsync($"api/Libro/{id}", ct);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException("El libro no existe.");
            }

            response.EnsureSuccessStatusCode();

            var libro = await response.Content.ReadFromJsonAsync<LibroDto>(cancellationToken: ct);
            return libro!;
        }

        // POST /api/Libro
        public async Task<LibroDto> CrearAsync(CreateUpdateLibroDto dto, CancellationToken ct = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Libro", dto, ct);

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                throw new InvalidOperationException("El ISBN indicado ya se encuentra registrado.");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Error al crear libro ({(int)response.StatusCode}).");
            }

            var libro = await response.Content.ReadFromJsonAsync<LibroDto>(cancellationToken: ct);
            return libro!;
        }

        // PUT /api/Libro/{id}
        public async Task ActualizarAsync(Guid id, CreateUpdateLibroDto dto, CancellationToken ct = default)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Libro/{id}", dto, ct);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException("El libro no existe.");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Error al actualizar libro ({(int)response.StatusCode}).");
            }
        }

        // PUT /api/Libro/{id}/ubicacion
        public async Task ActualizarUbicacionAsync(Guid id, string? ubicacion, CancellationToken ct = default)
        {
            var payload = new
            {
                ubicacion = ubicacion
            };

            var response = await _httpClient.PutAsJsonAsync($"api/Libro/{id}/ubicacion", payload, ct);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException("El libro no existe.");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Error al actualizar la ubicación ({(int)response.StatusCode}).");
            }
        }
    }
}
