using System;

namespace Clinicks.Application.DTOs;

public class CamaDto
{
    public int NCama { get; set; }
    public int IdHabitacion { get; set; }
    public string HabitacionNombre { get; set; } = string.Empty;
    public bool EstaOcupada { get; set; }
    public int? DniPaciente { get; set; }
    public string? NombrePaciente { get; set; }
    public string? ApellidoPaciente { get; set; }
    public DateTime? FechaInternacion { get; set; }
}
