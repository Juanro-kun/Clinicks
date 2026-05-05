namespace Clinicks.Application.DTOs.Pacientes;

public class PacienteResponseDTO
{
    public int Dni { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string? Telefono { get; set; }
    public string? Calle { get; set; }
    public int? Altura { get; set; }
    public string? CiudadNombre { get; set; }
    public string? ProvinciaNombre { get; set; }
    public string? PaisNombre { get; set; }
    public bool EstaInternado { get; set; }
}
