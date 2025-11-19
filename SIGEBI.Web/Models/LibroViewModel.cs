using System;
using System.ComponentModel.DataAnnotations;

namespace SIGEBI.Web.Models;

public class LibroViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(250, ErrorMessage = "El título no puede exceder 250 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El autor es obligatorio.")]
    [StringLength(200, ErrorMessage = "El autor no puede exceder 200 caracteres.")]
    public string Autor { get; set; } = string.Empty;

    [Display(Name = "ISBN")]
    [StringLength(40, ErrorMessage = "El ISBN no puede exceder 40 caracteres.")]
    public string? Isbn { get; set; }

    [Display(Name = "Ubicación")]
    [StringLength(100, ErrorMessage = "La ubicación no puede exceder 100 caracteres.")]
    public string? Ubicacion { get; set; }

    [Display(Name = "Fecha de publicación")]
    [DataType(DataType.Date)]
    public DateTime? FechaPublicacionUtc { get; set; }

    [Display(Name = "Ejemplares totales")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe haber al menos un ejemplar.")]
    public int EjemplaresTotales { get; set; }

    [Display(Name = "Ejemplares disponibles")]
    public int EjemplaresDisponibles { get; set; }

    public string? Estado { get; set; }
}
