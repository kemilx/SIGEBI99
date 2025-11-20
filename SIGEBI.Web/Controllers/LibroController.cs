using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.ApiClients;
using SIGEBI.Web.ApiDtos;
using SIGEBI.Web.Models;

namespace SIGEBI.Web.Controllers
{
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
            if (string.IsNullOrWhiteSpace(titulo) && string.IsNullOrWhiteSpace(autor))
            {
                return View(new List<LibroViewModel>());
            }

            try
            {
                var libros = await _libroApiClient.BuscarAsync(titulo, autor, ct);
                var model = libros.Select(MapToViewModel).ToList();
                return View(model);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
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
                return View(model);

            var dto = new CreateUpdateLibroDto
            {
                Titulo = model.Titulo,
                Autor = model.Autor,
                Isbn = model.Isbn,
                Ubicacion = model.Ubicacion,
                EjemplaresTotales = model.EjemplaresTotales,
                FechaPublicacionUtc = model.FechaPublicacionUtc
            };

            try
            {
                var libro = await _libroApiClient.CrearAsync(dto, ct);
                return RedirectToAction(nameof(Details), new { id = libro.Id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
        {
            var libro = await _libroApiClient.ObtenerPorIdAsync(id, ct);
            return View(MapToViewModel(libro));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, LibroViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new CreateUpdateLibroDto
            {
                Titulo = model.Titulo,
                Autor = model.Autor,
                Isbn = model.Isbn,
                Ubicacion = model.Ubicacion,
                EjemplaresTotales = model.EjemplaresTotales,
                FechaPublicacionUtc = model.FechaPublicacionUtc
            };

            try
            {
                await _libroApiClient.ActualizarAsync(id, dto, ct);
                await _libroApiClient.ActualizarUbicacionAsync(id, model.Ubicacion, ct);
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id, CancellationToken ct)
        {
            var libro = await _libroApiClient.ObtenerPorIdAsync(id, ct);
            return View(MapToViewModel(libro));
        }

        private static LibroViewModel MapToViewModel(LibroDto libro) => new()
        {
            Id = libro.Id,
            Titulo = libro.Titulo,
            Autor = libro.Autor,
            Isbn = libro.Isbn,
            Ubicacion = libro.Ubicacion,
            EjemplaresTotales = libro.EjemplaresTotales,
            EjemplaresDisponibles = libro.EjemplaresDisponibles,
            FechaPublicacionUtc = libro.FechaPublicacionUtc,
            Estado = libro.Estado.ToString()
        };

    }
}
