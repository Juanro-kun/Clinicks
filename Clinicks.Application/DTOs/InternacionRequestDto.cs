using System.ComponentModel.DataAnnotations;

namespace Clinicks.Application.DTOs;

public class InternacionRequestDto
{
    [Range(1_000_000, 99_999_999, ErrorMessage = "El DNI debe ser válido.")]
    public int Dni { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El Id de Habitación debe ser mayor a 0.")]
    public int IdHabitacion { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El número de cama debe ser mayor a 0.")]
    public int NCama { get; set; }
}
