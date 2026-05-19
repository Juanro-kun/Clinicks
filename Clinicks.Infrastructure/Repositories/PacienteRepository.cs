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
            IdCiudad = p.Direcciones.Select(d => d.IdCiudad).FirstOrDefault(),
            IdProvincia = p.Direcciones.Select(d => d.CiudadNavigation.IdProvincia).FirstOrDefault(),
            IdPais = p.Direcciones.Select(d => d.CiudadNavigation.ProvinciaNavigation.IdPais).FirstOrDefault(),
            EstaInternado = p.Internaciones.Any(i => i.FechaEgreso == null),
            Activo = p.Activo
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

        public async Task<Paciente?> ObtenerPacienteParaModificar(int dni)
        {
            return await _context.Pacientes.Include(p => p.Internaciones).FirstOrDefaultAsync(p => p.Dni == dni);
        }

        public void Agregar(Paciente paciente)
        {
            _context.Pacientes.Add(paciente);
        }

        public void Modificar(Paciente paciente)
        {
            _context.Pacientes.Update(paciente);
        }

        public void AgregarDireccion(Direccion direccion)
        {
            _context.Direcciones.Add(direccion);
        }
    }
}
