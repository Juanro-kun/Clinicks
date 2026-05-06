using System.Collections.Generic;
using System.Threading.Tasks;
using Clinicks.Application.DTOs.Pacientes;
using Clinicks.Application.Interfaces;
using Clinicks.Application.Exceptions;

namespace Clinicks.Application.Services
{
    public class PacienteService : IPacienteService
    {
        private readonly IPacienteRepository _repository;

        public PacienteService(IPacienteRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PacienteResponseDTO>> ListarPacientes()
        {
            return await _repository.ListarPacientes();
        }

        public async Task<PacienteResponseDTO?> BuscarPacientePorDni(int dni)
        {
            return await _repository.BuscarPacientePorDni(dni);
        }

        public async Task<bool> ConsultarPaciente(int dni)
        {
            return await _repository.ConsultarPaciente(dni);
        }

        public async Task RegistrarNuevoPaciente(PacienteCreateDTO pacienteDto)
        {
            var existe = await _repository.ConsultarPaciente(pacienteDto.Dni);
            if (existe)
                throw new ConflictException("Ya existe un paciente con este DNI.");

            await _repository.RegistrarNuevoPaciente(pacienteDto);
        }

        public async Task<bool> ActualizarDatosPaciente(PacienteUpdateDTO pacienteDTO)
        {
            return await _repository.ActualizarDatosPaciente(pacienteDTO);
        }

        public async Task EliminarPaciente(int dni)
        {
            await _repository.EliminarPaciente(dni);
        }
    }
}
