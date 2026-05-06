using System.Collections.Generic;
using System.Threading.Tasks;
using Clinicks.Application.DTOs.Pacientes;

namespace Clinicks.Application.Interfaces
{
    public interface IPacienteRepository
    {
        Task<IEnumerable<PacienteResponseDTO>> ListarPacientes();
        Task<PacienteResponseDTO?> BuscarPacientePorDni(int dni);
        Task<bool> ConsultarPaciente(int dni);
        Task RegistrarNuevoPaciente(PacienteCreateDTO pacienteDto);
        Task<bool> ActualizarDatosPaciente(PacienteUpdateDTO pacienteDto);
        Task EliminarPaciente(int dni);
    }
}
