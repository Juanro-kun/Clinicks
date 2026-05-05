using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clinicks.Domain.Entities;
using Clinicks.Application.DTOs.Pacientes;

namespace Clinicks.Application.Interfaces
{
    public interface IPacienteService
    {
        Task<IEnumerable<PacienteResponseDTO>> ListarPacientes();
        Task<PacienteResponseDTO?> BuscarPacientePorDni(int dni);
        Task<bool> ConsultarPaciente(int dni);
        Task RegistrarNuevoPaciente(PacienteCreateDTO pacienteDTO);
        Task<bool> ActualizarDatosPaciente(PacienteUpdateDTO pacienteDTO);
        Task EliminarPaciente(int dni);
    }
}
