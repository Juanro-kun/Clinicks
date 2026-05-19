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
        Task<Domain.Entities.Paciente?> ObtenerPacienteParaModificar(int dni);
        void Agregar(Domain.Entities.Paciente paciente);
        void Modificar(Domain.Entities.Paciente paciente);
        void AgregarDireccion(Domain.Entities.Direccion direccion);
    }
}
