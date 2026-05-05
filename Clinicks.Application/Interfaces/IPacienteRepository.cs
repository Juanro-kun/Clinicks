using System.Collections.Generic;
using System.Threading.Tasks;
using Clinicks.Application.DTOs.Pacientes;
using Clinicks.Domain.Entities;

namespace Clinicks.Application.Interfaces
{
    public interface IPacienteRepository
    {
        Task<IEnumerable<PacienteResponseDTO>> ListarPacientes();
        Task<PacienteResponseDTO?> BuscarPacientePorDni(int dni);
        Task<bool> ConsultarPaciente(int dni);
        Task<bool> ExisteUbicacionAsync(string ciudadNombre, string provinciaNombre, string paisNombre);
        Task RegistrarNuevoPaciente(Paciente paciente, string? calle, int? altura, string? ciudadNombre, string? provinciaNombre, string? paisNombre);
        Task<bool> ActualizarDatosPaciente(Paciente paciente, string? calle, int? altura, string? ciudadNombre, string? provinciaNombre, string? paisNombre);
        Task EliminarPaciente(int dni);
    }
}
