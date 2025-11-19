using System.Net;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Api;
using SIGEBI.Web.Models;

namespace SIGEBI.Web.Controllers
{
    public class PrestamoController : Controller
    {
        private readonly IPrestamoApiClient _prestamoApiClient;

        public PrestamoController(IPrestamoApiClient prestamoApiClient)
        {
            _prestamoApiClient = prestamoApiClient;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new PrestamoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PrestamoViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var prestamo = await _prestamoApiClient.CrearAsync(
                    new CrearPrestamoRequest(
                        model.LibroId,
                        model.UsuarioId,
                        model.FechaInicioUtc,
                        model.FechaFinUtc),
                    ct);

                TempData["SuccessMessage"] = "Préstamo registrado correctamente.";
                return RedirectToAction(nameof(Details), new { id = prestamo.Id });
            }
            catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            catch (ApiException ex)
            {
                TempData["ErrorMessage"] = $"No se pudo crear el préstamo: {ex.Message}";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id, CancellationToken ct)
        {
            try
            {
                var prestamo = await _prestamoApiClient.ObtenerPorIdAsync(id, ct);
                if (prestamo is null)
                {
                    return NotFound();
                }

                return View(MapToViewModel(prestamo));
            }
            catch (ApiException ex)
            {
                TempData["ErrorMessage"] = $"No se pudo obtener el préstamo: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid? usuarioId, CancellationToken ct)
        {
            // Primera carga: sin filtro → lista vacía
            if (usuarioId is null || usuarioId == Guid.Empty)
            {
                ViewBag.UsuarioId = string.Empty;
                return View(new List<PrestamoViewModel>());
            }

            try
            {
                var prestamos = await _prestamoApiClient.ObtenerPorUsuarioAsync(usuarioId.Value, ct);
                var model = prestamos.Select(MapToViewModel).ToList();
                ViewBag.UsuarioId = usuarioId;
                return View(model);
            }
            catch (ApiException ex)
            {
                TempData["ErrorMessage"] = $"No se pudieron consultar los préstamos: {ex.Message}";
                ViewBag.UsuarioId = usuarioId;
                return View(new List<PrestamoViewModel>());
            }
        }

        private static PrestamoViewModel MapToViewModel(PrestamoDto prestamo) => new()
        {
            Id = prestamo.Id,
            LibroId = prestamo.LibroId,
            UsuarioId = prestamo.UsuarioId,
            Estado = prestamo.Estado,
            FechaInicioUtc = prestamo.FechaInicioUtc,
            FechaFinUtc = prestamo.FechaFinCompromisoUtc,
            Observaciones = prestamo.Observaciones
        };
    }
}
