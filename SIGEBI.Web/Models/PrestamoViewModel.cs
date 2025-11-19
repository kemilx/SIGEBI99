using System;
using System.ComponentModel.DataAnnotations;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Web.Models;

public class PrestamoViewModel
{
    public Guid? Id { get; set; }

    [Required]
    [Display(Name = "Libro")]
    public Guid LibroId { get; set; }

    [Required]
    [Display(Name = "Usuario")]
    public Guid UsuarioId { get; set; }

    [Display(Name = "Estado")]
    public EstadoPrestamo? Estado { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha inicio")]
    public DateTime FechaInicioUtc { get; set; } = DateTime.UtcNow.Date;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha fin comprometida")]
    public DateTime FechaFinUtc { get; set; } = DateTime.UtcNow.Date.AddDays(7);

    [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres.")]
    public string? Observaciones { get; set; }
}
