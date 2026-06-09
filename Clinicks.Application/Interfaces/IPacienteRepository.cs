using System.Collections.Generic;
using System.Threading.Tasks;
using Clinicks.Application.DTOs.Pacientes;
using Clinicks.Application.DTOs.Shared;

namespace Clinicks.Application.Interfaces
{
    public interface IPacienteRepository
    {
        Task<PaginatedResult<PacienteResponseDTO>> ListarPacientes(int page = 1, int pageSize = 15, bool fetchAll = false);
        Task<PaginatedResult<PacienteResponseDTO>> BuscarPacientes(string terminoBusqueda, int page = 1, int pageSize = 15);
        Task<PacienteResponseDTO?> BuscarPacientePorDni(int dni);
        Task<bool> ConsultarPaciente(int dni);
        Task<Domain.Entities.Paciente?> ObtenerPacientePorDni(int dni);
        void Agregar(Domain.Entities.Paciente paciente);
        void Modificar(Domain.Entities.Paciente paciente);
    }
}
