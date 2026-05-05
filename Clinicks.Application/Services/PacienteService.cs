using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinicks.Application.DTOs.Pacientes;
using Clinicks.Application.Interfaces;
using Clinicks.Application.Exceptions;
using Clinicks.Domain.Entities;

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

            if (!string.IsNullOrWhiteSpace(pacienteDto.Calle))
            {
                var ubicacionValida = await _repository.ExisteUbicacionAsync(
                    pacienteDto.CiudadNombre ?? "", 
                    pacienteDto.ProvinciaNombre ?? "", 
                    pacienteDto.PaisNombre ?? "");
                
                if (!ubicacionValida)
                    throw new ValidationException("La ciudad, provincia o país no son válidos o no existen.");
            }

            var nuevoPaciente = new Paciente
            {
                Dni = pacienteDto.Dni,
                Nombre = pacienteDto.Nombre,
                Apellido = pacienteDto.Apellido,
                Telefono = pacienteDto.Telefono
            };

            await _repository.RegistrarNuevoPaciente(nuevoPaciente, pacienteDto.Calle, pacienteDto.Altura, pacienteDto.CiudadNombre, pacienteDto.ProvinciaNombre, pacienteDto.PaisNombre);
        }

        public async Task<bool> ActualizarDatosPaciente(PacienteUpdateDTO pacienteDTO)
        {
            if (!string.IsNullOrWhiteSpace(pacienteDTO.Calle) && pacienteDTO.Altura.HasValue)
            {
                var existeUbicacion = await _repository.ExisteUbicacionAsync(
                    pacienteDTO.CiudadNombre ?? "", 
                    pacienteDTO.ProvinciaNombre ?? "", 
                    pacienteDTO.PaisNombre ?? "");
                    
                if (!existeUbicacion)
                {
                    throw new ArgumentException("La ciudad, provincia o país especificado no se encuentra registrado en el sistema.");
                }
            }

            var paciente = new Paciente
            {
                Dni = pacienteDTO.Dni,
                Nombre = pacienteDTO.Nombre,
                Apellido = pacienteDTO.Apellido,
                Telefono = pacienteDTO.Telefono
            };

            return await _repository.ActualizarDatosPaciente(paciente, pacienteDTO.Calle, pacienteDTO.Altura, pacienteDTO.CiudadNombre, pacienteDTO.ProvinciaNombre, pacienteDTO.PaisNombre);
        }

        public async Task EliminarPaciente(int dni)
        {
            await _repository.EliminarPaciente(dni);
        }
    }
}
