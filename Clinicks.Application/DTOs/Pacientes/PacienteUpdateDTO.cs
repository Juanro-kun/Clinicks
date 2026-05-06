using System.ComponentModel.DataAnnotations;

namespace Clinicks.Application.DTOs.Pacientes;

public class PacienteUpdateDTO
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

    [Phone(ErrorMessage = "El formato del teléfono no es válido.")]
    public string? Telefono { get; set; }

    public string? Calle { get; set; }
    public int? Altura { get; set; }
    public int? IdCiudad { get; set; }
    public int? IdProvincia { get; set; }
    public int? IdPais { get; set; }
}
