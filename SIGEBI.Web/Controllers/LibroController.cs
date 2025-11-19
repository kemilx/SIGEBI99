using Microsoft.AspNetCore.Mvc;
using SIGEBI.Application.Interfaces;
using SIGEBI.Domain.Entities;
using SIGEBI.Web.Models;

namespace SIGEBI.Web.Controllers;

public class LibroController : Controller
{
    private readonly ILibroService _libroService;

    public LibroController(ILibroService libroService)
    {
        _libroService = libroService;
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
        var libros = await _libroService.BuscarAsync(titulo, autor, ct);
        var model = libros.Select(MapToViewModel).ToList();
        return View(model);
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

        var libro = await _libroService.CrearAsync(
            model.Titulo,
            model.Autor,
            model.EjemplaresTotales,
            model.Isbn,
            model.Ubicacion,
            model.FechaPublicacionUtc,
            ct);

        TempData["SuccessMessage"] = "Libro creado correctamente.";
        return RedirectToAction(nameof(Details), new { id = libro.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var libro = await _libroService.ObtenerPorIdAsync(id, ct);
        if (libro is null)
        {
            return NotFound();
        }

        return View(MapToViewModel(libro));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, LibroViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _libroService.ActualizarAsync(id, model.Titulo, model.Autor, model.Isbn, model.FechaPublicacionUtc, ct);
        await _libroService.ActualizarUbicacionAsync(id, model.Ubicacion, ct);

        TempData["SuccessMessage"] = "Libro actualizado correctamente.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var libro = await _libroService.ObtenerPorIdAsync(id, ct);
        if (libro is null)
        {
            return NotFound();
        }

        return View(MapToViewModel(libro));
    }

    private static LibroViewModel MapToViewModel(Libro libro) => new()
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
