using System.Collections.Generic;
using System.Threading.Tasks;
using Clinicks.Application.DTOs.Pacientes;
using Clinicks.Application.DTOs.Shared;
using Clinicks.Application.Interfaces;
using Clinicks.Application.Exceptions;
using Clinicks.Domain.Entities;

namespace Clinicks.Application.Services
{
    public class PacienteService : IPacienteService
    {
        private readonly IPacienteRepository _repository;
        private readonly IUnidadDeTrabajo _unidadDeTrabajo;
        private readonly ICurrentUserProvider _currentUserProvider;

        public PacienteService(
            IPacienteRepository repository,
            IUnidadDeTrabajo unidadDeTrabajo,
            ICurrentUserProvider currentUserProvider)
        {
            _repository = repository;
            _unidadDeTrabajo = unidadDeTrabajo;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<PaginatedResult<PacienteResponseDTO>> ListarPacientes(int page = 1, int pageSize = 15, bool fetchAll = false)
        {
            return await _repository.ListarPacientes(page, pageSize, fetchAll);
        }

        public async Task<PaginatedResult<PacienteResponseDTO>> BuscarPacientes(string terminoBusqueda, int page = 1, int pageSize = 15)
        {
            return await _repository.BuscarPacientes(terminoBusqueda, page, pageSize);
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

            int? creadorId = _currentUserProvider.GetCurrentUserId();

            // 1. Instanciamos el paciente
            var paciente = new Paciente
            {
                Dni = pacienteDto.Dni,
                Nombre = pacienteDto.Nombre,
                Apellido = pacienteDto.Apellido,
                Telefono = pacienteDto.Telefono,
                CreadoPorUsuarioId = creadorId
            };

            // 2. Armamos el grafo: si hay dirección, se la agregamos a la colección del paciente
            if (!string.IsNullOrWhiteSpace(pacienteDto.Calle))
            {
                if (!pacienteDto.Altura.HasValue)
                {
                    throw new ValidationException("Debe proporcionar la altura de la calle especificada.");
                }

                paciente.Direcciones.Add(new Direccion
                {
                    Calle = pacienteDto.Calle.Trim(),
                    Altura = pacienteDto.Altura.Value,
                    IdCiudad = pacienteDto.IdCiudad
                });
            }

            _repository.Agregar(paciente);

            await _unidadDeTrabajo.GuardarCambiosAsync();
        }

        public async Task<bool> ActualizarDatosPaciente(PacienteUpdateDTO pacienteDto)
        {
            var paciente = await _repository.ObtenerPacientePorDni(pacienteDto.Dni);
            if (paciente == null)
            {
                return false;
            }

            paciente.Nombre = pacienteDto.Nombre;
            paciente.Apellido = pacienteDto.Apellido;
            paciente.Telefono = pacienteDto.Telefono;
            paciente.Activo = pacienteDto.Activo;


            if (!string.IsNullOrWhiteSpace(pacienteDto.Calle))
            {
                if (!pacienteDto.Altura.HasValue)
                {
                    throw new ValidationException("Debe proporcionar la altura de la calle especificada.");
                }

                var direccionExistente = paciente.Direcciones.FirstOrDefault();
                if (direccionExistente != null)
                {
                    direccionExistente.Calle = pacienteDto.Calle.Trim();
                    direccionExistente.Altura = pacienteDto.Altura.Value;
                    direccionExistente.IdCiudad = pacienteDto.IdCiudad;
                }
                else
                {
                    paciente.Direcciones.Add(new Direccion
                    {
                        Calle = pacienteDto.Calle.Trim(),
                        Altura = pacienteDto.Altura.Value,
                        IdCiudad = pacienteDto.IdCiudad
                    });
                }
            }

            await _unidadDeTrabajo.GuardarCambiosAsync();
            return true;
        }

        public async Task EliminarPaciente(int dni)
        {
            var paciente = await _repository.ObtenerPacientePorDni(dni);
            if (paciente == null)
            {
                return;
            }

            try
            {
                paciente.Eliminar();
            }
            catch (InvalidOperationException ex)
            {
                // Traduce el error de dominio (por ej. paciente internado) a un error HTTP 409 Conflict.
                throw new ConflictException(ex.Message);
            }

            await _unidadDeTrabajo.GuardarCambiosAsync();
        }
    }
}

/*
CODIGO ANTERIOR
namespace Clinicks.Application.Services
{
    public class PacienteService : IPacienteService
    {
        private readonly ClinicksDbContext _context;

        public PacienteService(ClinicksDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Paciente>> GetAllAsync()
        {
            return await _context.Pacientes.ToListAsync();
        }

        public async Task<Paciente?> GetByDniAsync(int dni)
        {
            return await _context.Pacientes.FindAsync(dni);
        }

        public async Task<bool> CreateAsync(Paciente paciente)
        {
            
            var existe = await _context.Pacientes.AnyAsync(p => p.Dni == paciente.Dni);

            if (existe)
            {
                return false;
            }

            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();

            return true; 
        }

        public async Task<bool> UpdateAsync(Paciente paciente)
        {
            _context.Entry(paciente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true; // Salió todo joya
            }
            catch (DbUpdateConcurrencyException)
            {
                // Si saltó este error, es muy probable que el paciente no exista
                if (!PacienteExists(paciente.Dni)) return false;
                throw; // Si es otro error raro, que explote
            }
        }

        // Un helper privado en el service para chequear si está
        private bool PacienteExists(int dni)
        {
            return _context.Pacientes.Any(e => e.Dni == dni);
        }

        public async Task DeleteAsync(int dni)
        {
            var paciente = await _context.Pacientes.FindAsync(dni);
            if (paciente != null)
            {
                _context.Pacientes.Remove(paciente);
                await _context.SaveChangesAsync();
            }
        }
    }
} 
*/
