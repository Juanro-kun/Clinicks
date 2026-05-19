using System;

namespace Clinicks.Application.DTOs;

public class InternacionResponseDto
{
    public int IdInternacion { get; set; }
    public int Dni { get; set; }
    public string NombrePaciente { get; set; } = string.Empty;
    public string ApellidoPaciente { get; set; } = string.Empty;
    public string HabitacionNombre { get; set; } = string.Empty;
    public int NCama { get; set; }
    public DateTime? FechaIngreso { get; set; }
    public DateTime? FechaEgreso { get; set; }
}
