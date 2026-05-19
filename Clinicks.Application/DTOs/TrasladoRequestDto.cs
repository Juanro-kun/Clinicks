using System.ComponentModel.DataAnnotations;

namespace Clinicks.Application.DTOs;

public class TrasladoRequestDto
{
    [Required]
    public int Dni { get; set; }
    [Required]
    public int IdHabitacion { get; set; }

    [Required]
    public int NCama { get; set; }
}
