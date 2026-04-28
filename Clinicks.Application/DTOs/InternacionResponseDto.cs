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
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}
