using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Clinicks.Domain.Entities;

public partial class Paciente
{
    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [Range(1_000_000, 99_999_999, ErrorMessage = "El DNI debe ser un número válido de 7 u 8 cifras.")]
    public int Dni { get; set; }

    [Required(ErrorMessage = "El nombre no puede estar vacío.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "El apellido debe tener entre 1 y 50 caracteres.")]
    public string Apellido { get; set; } = null!;

    [StringLength(255, ErrorMessage = "La dirección es demasiado larga.")]
    public string? Direccion { get; set; }

    [Phone(ErrorMessage = "El formato del teléfono no es válido.")]
    public string? Telefono { get; set; }
}
