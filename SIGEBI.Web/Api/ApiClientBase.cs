using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace SIGEBI.Web.Api;

public abstract class ApiClientBase
{
    protected ApiClientBase(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    protected HttpClient HttpClient { get; }

    protected JsonSerializerOptions SerializerOptions { get; } = new(JsonSerializerDefaults.Web);

    protected async Task<T> ReadAsAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        await EnsureSuccessAsync(response);
        var result = await response.Content.ReadFromJsonAsync<T>(SerializerOptions, ct);
        if (result is null)
        {
            throw new ApiException(response.StatusCode, "La API devolvió una respuesta vacía.");
        }

        return result;
    }

    protected async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var message = await ReadErrorMessageAsync(response);
        throw new ApiException(response.StatusCode, message);
    }

    protected async Task<string> ReadErrorMessageAsync(HttpResponseMessage response)
    {
        var content = response.Content is null ? null : await response.Content.ReadAsStringAsync();

        if (!string.IsNullOrWhiteSpace(content))
        {
            try
            {
                var problem = JsonSerializer.Deserialize<ProblemDetails>(content, SerializerOptions);
                if (!string.IsNullOrWhiteSpace(problem?.Title))
                {
                    return string.Join(" ", new[] { problem.Title, problem.Detail }.Where(s => !string.IsNullOrWhiteSpace(s)));
                }
            }
            catch (JsonException)
            {
                // Ignored: fallback to raw content
            }

            return content;
        }

        return $"La API respondió con código {(int)response.StatusCode} ({response.StatusCode}).";
    }

    protected static bool IsNotFound(HttpResponseMessage response) => response.StatusCode == HttpStatusCode.NotFound;
}
