using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clinicks.Domain.Entities;
using Clinicks.Application.DTOs.Pacientes;
using Clinicks.Application.DTOs.Shared;

namespace Clinicks.Application.Interfaces
{
    public interface IPacienteService
    {
        Task<PaginatedResult<PacienteResponseDTO>> ListarPacientes(int page = 1, int pageSize = 15, bool fetchAll = false);
        Task<PaginatedResult<PacienteResponseDTO>> BuscarPacientes(string terminoBusqueda, int page = 1, int pageSize = 15);
        Task<PacienteResponseDTO?> BuscarPacientePorDni(int dni);
        Task<bool> ConsultarPaciente(int dni);
        Task RegistrarNuevoPaciente(PacienteCreateDTO pacienteDTO);
        Task<bool> ActualizarDatosPaciente(PacienteUpdateDTO pacienteDTO);
        Task EliminarPaciente(int dni);
    }
}
