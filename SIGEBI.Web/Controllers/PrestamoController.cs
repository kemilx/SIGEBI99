using Microsoft.AspNetCore.Mvc;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Prestamos.Commands;
using SIGEBI.Domain.Entities;
using SIGEBI.Web.Models;

namespace SIGEBI.Web.Controllers
{
    public class PrestamoController : Controller
    {
        private readonly IPrestamoService _prestamoService;

        public PrestamoController(IPrestamoService prestamoService)
        {
            _prestamoService = prestamoService;
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

            var prestamo = await _prestamoService.CrearAsync(
                new CrearPrestamoCommand(
                    model.LibroId,
                    model.UsuarioId,
                    model.FechaInicioUtc,
                    model.FechaFinUtc),
                ct);

            TempData["SuccessMessage"] = "Préstamo registrado correctamente.";
            return RedirectToAction(nameof(Details), new { id = prestamo.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id, CancellationToken ct)
        {
            var prestamo = await _prestamoService.ObtenerPorIdAsync(id, ct);
            return View(MapToViewModel(prestamo));
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

            var prestamos = await _prestamoService.ObtenerPorUsuarioAsync(usuarioId.Value, ct);
            var model = prestamos.Select(MapToViewModel).ToList();
            ViewBag.UsuarioId = usuarioId;
            return View(model);
        }

        private static PrestamoViewModel MapToViewModel(Prestamo prestamo) => new()
        {
            Id = prestamo.Id,
            LibroId = prestamo.LibroId,
            UsuarioId = prestamo.UsuarioId,
            Estado = prestamo.Estado,
            FechaInicioUtc = prestamo.Periodo.FechaInicioUtc,
            FechaFinUtc = prestamo.Periodo.FechaFinCompromisoUtc,
            Observaciones = prestamo.Observaciones
        };
    }
}
