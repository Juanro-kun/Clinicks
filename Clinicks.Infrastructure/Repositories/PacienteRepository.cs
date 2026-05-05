using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Clinicks.Application.DTOs.Pacientes;
using Clinicks.Application.Interfaces;
using Clinicks.Domain.Entities;
using Clinicks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clinicks.Infrastructure.Repositories
{
    public class PacienteRepository : IPacienteRepository
    {
        private readonly ClinicksDbContext _context;

        public PacienteRepository(ClinicksDbContext context)
        {
            _context = context;
        }

        private static Expression<Func<Paciente, PacienteResponseDTO>> MapToResponseDTO => p => new PacienteResponseDTO
        {
            Dni = p.Dni,
            Nombre = p.Nombre,
            Apellido = p.Apellido,
            Telefono = p.Telefono,
            Calle = p.Direcciones.Select(d => d.Calle).FirstOrDefault(),
            Altura = p.Direcciones.Select(d => (int?)d.Altura).FirstOrDefault(),
            CiudadNombre = p.Direcciones.Select(d => d.CiudadNavigation.Nombre).FirstOrDefault(),
            ProvinciaNombre = p.Direcciones.Select(d => d.CiudadNavigation.ProvinciaNavigation.Nombre).FirstOrDefault(),
            PaisNombre = p.Direcciones.Select(d => d.CiudadNavigation.ProvinciaNavigation.PaisNavigation.Nombre).FirstOrDefault(),
            EstaInternado = p.Internaciones.Any(i => i.FechaFin == null)
        };

        public async Task<IEnumerable<PacienteResponseDTO>> ListarPacientes()
        {
            return await _context.Pacientes
                .Select(MapToResponseDTO)
                .ToListAsync();
        }

        public async Task<PacienteResponseDTO?> BuscarPacientePorDni(int dni)
        {
            return await _context.Pacientes
                .Where(p => p.Dni == dni)
                .Select(MapToResponseDTO)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ConsultarPaciente(int dni)
        {
            return await _context.Pacientes.AnyAsync(e => e.Dni == dni);
        }

        public async Task RegistrarNuevoPaciente(Paciente paciente, string? calle, int? altura, string? ciudadNombre, string? provinciaNombre, string? paisNombre)
        {
            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(calle))
            {
                await GuardarDireccion(paciente.Dni, calle, altura ?? 0, ciudadNombre, provinciaNombre, paisNombre);
            }
        }

        public async Task<bool> ActualizarDatosPaciente(Paciente paciente, string? calle, int? altura, string? ciudadNombre, string? provinciaNombre, string? paisNombre)
        {
            if (!string.IsNullOrWhiteSpace(calle) && altura.HasValue)
            {
                await GuardarDireccion(paciente.Dni, calle, altura.Value, ciudadNombre, provinciaNombre, paisNombre);
            }

            _context.Entry(paciente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true; 
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ConsultarPaciente(paciente.Dni)) return false;
                throw; 
            }
        }

        public async Task EliminarPaciente(int dni)
        {
            var paciente = await _context.Pacientes.FindAsync(dni);
            if (paciente != null)
            {
                _context.Pacientes.Remove(paciente);
                await _context.SaveChangesAsync();
            }
        }

        private IQueryable<Ciudad> ObtenerConsultaCiudad(string ciudadNombre, string provinciaNombre, string paisNombre)
        {
            return _context.Ciudades
                .Include(c => c.ProvinciaNavigation)
                    .ThenInclude(p => p.PaisNavigation)
                .Where(c => 
                    c.Nombre == ciudadNombre && 
                    c.ProvinciaNavigation.Nombre == provinciaNombre && 
                    c.ProvinciaNavigation.PaisNavigation.Nombre == paisNombre);
        }

        public async Task<bool> ExisteUbicacionAsync(string ciudadNombre, string provinciaNombre, string paisNombre)
        {
            return await ObtenerConsultaCiudad(ciudadNombre, provinciaNombre, paisNombre).AnyAsync();
        }

        private async Task<int> GuardarDireccion(int dni, string calle, int altura, string? ciudadNombre, string? provinciaNombre, string? paisNombre)
        {
            var ciudad = await ObtenerConsultaCiudad(ciudadNombre ?? "", provinciaNombre ?? "", paisNombre ?? "").FirstOrDefaultAsync();

            var direccion = new Direccion { 
                Calle = calle.Trim(), 
                Altura = altura, 
                IdCiudad = ciudad!.IdCiudad,
                Dni = dni
            };

            _context.Direcciones.Add(direccion);
            await _context.SaveChangesAsync();

            return direccion.IdDireccion;
        }
    }
}
