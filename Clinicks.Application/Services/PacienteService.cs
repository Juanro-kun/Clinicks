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
        private readonly IUnidadDeTrabajo _unidadDeTrabajo;

        public PacienteService(IPacienteRepository repository, IUnidadDeTrabajo unidadDeTrabajo)
        {
            _repository = repository;
            _unidadDeTrabajo = unidadDeTrabajo;
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

            // 1. Instanciamos el paciente
            var paciente = new Paciente
            {
                Dni = pacienteDto.Dni,
                Nombre = pacienteDto.Nombre,
                Apellido = pacienteDto.Apellido,
                Telefono = pacienteDto.Telefono
            };

            // 2. Armamos el grafo: si hay dirección, se la agregamos a la colección del paciente
            if (!string.IsNullOrWhiteSpace(pacienteDto.Calle))
            {
                paciente.Direcciones.Add(new Direccion
                {
                    Calle = pacienteDto.Calle.Trim(),
                    Altura = pacienteDto.Altura ?? 0,
                    IdCiudad = pacienteDto.IdCiudad
                });
            }

            _repository.Agregar(paciente);

            await _unidadDeTrabajo.GuardarCambiosAsync();
        }

        public async Task<bool> ActualizarDatosPaciente(PacienteUpdateDTO pacienteDto)
        {
            var paciente = await _repository.ObtenerPacienteParaModificar(pacienteDto.Dni);
            if (paciente == null)
            {
                return false;
            }

            paciente.Nombre = pacienteDto.Nombre;
            paciente.Apellido = pacienteDto.Apellido;
            paciente.Telefono = pacienteDto.Telefono;
            paciente.Activo = pacienteDto.Activo;


            //ESTO ANDA MAL, CREA UNA DIRECCION CADA VEZ QUE SE EJECUTA, HAY QUE ENCONTRAR UNA SOLUCION PERO POR AHORA QUEDA ASÍ
            if (!string.IsNullOrWhiteSpace(pacienteDto.Calle) && pacienteDto.Altura.HasValue)
            {
                var direccion = new Direccion
                {
                    Calle = pacienteDto.Calle.Trim(),
                    Altura = pacienteDto.Altura.Value,
                    IdCiudad = pacienteDto.IdCiudad,
                    Dni = paciente.Dni
                };
                _repository.AgregarDireccion(direccion);
            }

            await _unidadDeTrabajo.GuardarCambiosAsync();
            return true;
        }

        public async Task EliminarPaciente(int dni)
        {
            var paciente = await _repository.ObtenerPacienteParaModificar(dni);
            if (paciente == null)
            {
                return;
            }
            
            var estaInternado = paciente.Internaciones.Any(i => i.FechaEgreso == null);
            if (estaInternado)
            {
                throw new ConflictException("No se puede eliminar un paciente que se encuentra internado.");
            }
            
            paciente.Activo = false;
            
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
