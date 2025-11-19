using System.Net;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Api;
using SIGEBI.Web.Models;

namespace SIGEBI.Web.Controllers;

public class LibroController : Controller
{
    private readonly ILibroApiClient _libroApiClient;

    public LibroController(ILibroApiClient libroApiClient)
    {
        _libroApiClient = libroApiClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? titulo, string? autor, CancellationToken ct)
    {
        // Si no hay criterios de búsqueda, primera carga => vista vacía
        if (string.IsNullOrWhiteSpace(titulo) && string.IsNullOrWhiteSpace(autor))
        {
            return View(new List<LibroViewModel>());
        }

        // Si hay título o autor, entonces sí buscamos
        try
        {
            var libros = await _libroApiClient.BuscarAsync(titulo, autor, ct);
            var model = libros.Select(MapToViewModel).ToList();
            return View(model);
        }
        catch (ApiException ex)
        {
            TempData["ErrorMessage"] = $"No se pudo consultar los libros: {ex.Message}";
            return View(new List<LibroViewModel>());
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new LibroViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LibroViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var libro = await _libroApiClient.CrearAsync(
                new CrearLibroRequest(
                    model.Titulo,
                    model.Autor,
                    model.EjemplaresTotales,
                    model.Isbn,
                    model.Ubicacion,
                    model.FechaPublicacionUtc),
                ct);

            TempData["SuccessMessage"] = "Libro creado correctamente.";
            return RedirectToAction(nameof(Details), new { id = libro.Id });
        }
        catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
        catch (ApiException ex)
        {
            TempData["ErrorMessage"] = $"No se pudo crear el libro: {ex.Message}";
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        try
        {
            var libro = await _libroApiClient.ObtenerPorIdAsync(id, ct);
            if (libro is null)
            {
                return NotFound();
            }

            return View(MapToViewModel(libro));
        }
        catch (ApiException ex)
        {
            TempData["ErrorMessage"] = $"No se pudo cargar el libro: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, LibroViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _libroApiClient.ActualizarAsync(
                id,
                new ActualizarLibroRequest(model.Titulo, model.Autor, model.Isbn, model.FechaPublicacionUtc),
                ct);

            await _libroApiClient.ActualizarUbicacionAsync(id, new ActualizarUbicacionRequest(model.Ubicacion), ct);

            TempData["SuccessMessage"] = "Libro actualizado correctamente.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
        catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }
        catch (ApiException ex)
        {
            TempData["ErrorMessage"] = $"No se pudo actualizar el libro: {ex.Message}";
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        try
        {
            var libro = await _libroApiClient.ObtenerPorIdAsync(id, ct);
            if (libro is null)
            {
                return NotFound();
            }

            return View(MapToViewModel(libro));
        }
        catch (ApiException ex)
        {
            TempData["ErrorMessage"] = $"No se pudo obtener la información del libro: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    private static LibroViewModel MapToViewModel(LibroDto libro) => new()
    {
        Id = libro.Id,
        Titulo = libro.Titulo,
        Autor = libro.Autor,
        Isbn = libro.Isbn,
        Ubicacion = libro.Ubicacion,
        FechaPublicacionUtc = libro.FechaPublicacion,
        EjemplaresTotales = libro.EjemplaresTotales,
        EjemplaresDisponibles = libro.EjemplaresDisponibles,
        Estado = libro.Estado.ToString()
    };
}
