using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clinicks.Application.Context;
using Clinicks.Application.Interfaces;
using Clinicks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clinicks.Application.Services
{
    public class PacienteService : IPacienteService
    {
        private readonly ClinicksDbContext _context;

        public PacienteService(ClinicksDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Paciente>> ListarPacientes()
        {
            var pacientes = await _context.Pacientes
                .Include(p => p.Internaciones)
                .Include(p => p.DireccionNavigation)
                    .ThenInclude(d => d.CiudadNavigation)
                        .ThenInclude(c => c.ProvinciaNavigation)
                            .ThenInclude(pr => pr.PaisNavigation)
                .ToListAsync();

            foreach (var p in pacientes)
            {
                if (p.DireccionNavigation != null)
                {
                    p.Calle = p.DireccionNavigation.Calle;
                    p.Altura = p.DireccionNavigation.Altura;
                    p.CiudadNombre = p.DireccionNavigation.CiudadNavigation?.Nombre;
                    p.ProvinciaNombre = p.DireccionNavigation.CiudadNavigation?.ProvinciaNavigation?.Nombre;
                    p.PaisNombre = p.DireccionNavigation.CiudadNavigation?.ProvinciaNavigation?.PaisNavigation?.Nombre;
                }
                
                // Determinamos si está internado
                p.EstaInternado = p.Internaciones.Any(i => i.FechaFin == null);
            }

            return pacientes;
        }

        public async Task<Paciente?> BuscarPacientePorDni(int dni)
        {
            var p = await ObtenerDetallesCompletosDelPaciente(dni);

            if (p != null && p.DireccionNavigation != null)
            {
                p.Calle = p.DireccionNavigation.Calle;
                p.Altura = p.DireccionNavigation.Altura;
                p.CiudadNombre = p.DireccionNavigation.CiudadNavigation?.Nombre;
                p.ProvinciaNombre = p.DireccionNavigation.CiudadNavigation?.ProvinciaNavigation?.Nombre;
                p.PaisNombre = p.DireccionNavigation.CiudadNavigation?.ProvinciaNavigation?.PaisNavigation?.Nombre;
            }

            if (p != null)
            {
                p.EstaInternado = p.Internaciones.Any(i => i.FechaFin == null);
            }

            return p;
        }

        private async Task<Paciente?> ObtenerDetallesCompletosDelPaciente(int dni)
        {
            return await _context.Pacientes
                .Include(pac => pac.Internaciones)
                .Include(pac => pac.DireccionNavigation)
                    .ThenInclude(d => d.CiudadNavigation)
                        .ThenInclude(c => c.ProvinciaNavigation)
                            .ThenInclude(pr => pr.PaisNavigation)
                .FirstOrDefaultAsync(pac => pac.Dni == dni);
        }

        public async Task<bool> ConsultarPaciente(int dni)
        {
            return await _context.Pacientes.AnyAsync(e => e.Dni == dni);
        }

        public async Task RegistrarNuevoPaciente(int dni, string nombre, string apellido, string telefono, string calle, int altura, string ciudadNombre, string provinciaNombre, string paisNombre)
        {
            int? idDireccion = null;

            if (!string.IsNullOrWhiteSpace(calle))
            {
                await VerificarDireccion(calle, altura, ciudadNombre, provinciaNombre, paisNombre);
                idDireccion = await GuardarDireccion(calle, altura, ciudadNombre, provinciaNombre, paisNombre);
            }

            var paciente = new Paciente
            {
                Dni = dni,
                Nombre = nombre,
                Apellido = apellido,
                Telefono = telefono,
                IdDireccion = idDireccion
            };

            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ActualizarDatosPaciente(Paciente paciente)
        {
            if (!string.IsNullOrWhiteSpace(paciente.Calle) && paciente.Altura.HasValue)
            {
                await VerificarDireccion(paciente.Calle, paciente.Altura.Value, paciente.CiudadNombre, paciente.ProvinciaNombre, paciente.PaisNombre);
                paciente.IdDireccion = await GuardarDireccion(paciente.Calle, paciente.Altura.Value, paciente.CiudadNombre, paciente.ProvinciaNombre, paciente.PaisNombre);
            }
            else
            {
                paciente.IdDireccion = null;
            }

            _context.Entry(paciente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true; // Salió todo joya
            }
            catch (DbUpdateConcurrencyException)
            {
                // Si saltó este error, es muy probable que el paciente no exista
                if (!await ConsultarPaciente(paciente.Dni)) return false;
                throw; // Si es otro error raro, que explote
            }
        }

        private async Task VerificarDireccion(string calle, int altura, string ciudadNombre, string provinciaNombre, string paisNombre)
        {
            var ciudad = await _context.Ciudades
                .Include(c => c.ProvinciaNavigation)
                    .ThenInclude(p => p.PaisNavigation)
                .FirstOrDefaultAsync(c => 
                    c.Nombre == ciudadNombre && 
                    c.ProvinciaNavigation.Nombre == provinciaNombre && 
                    c.ProvinciaNavigation.PaisNavigation.Nombre == paisNombre);

            if (ciudad == null)
            {
                throw new ArgumentException("La ciudad, provincia o país especificado no se encuentra registrado en el sistema.");
            }
        }

        private async Task<int> GuardarDireccion(string calle, int altura, string ciudadNombre, string provinciaNombre, string paisNombre)
        {
            var ciudad = await _context.Ciudades
                .Include(c => c.ProvinciaNavigation)
                    .ThenInclude(p => p.PaisNavigation)
                .FirstOrDefaultAsync(c => 
                    c.Nombre == ciudadNombre && 
                    c.ProvinciaNavigation.Nombre == provinciaNombre && 
                    c.ProvinciaNavigation.PaisNavigation.Nombre == paisNombre);

            var direccion = new Direccion { 
                Calle = calle.Trim(), 
                Altura = altura, 
                IdCiudad = ciudad!.IdCiudad 
            };

            _context.Direcciones.Add(direccion);
            await _context.SaveChangesAsync();

            return direccion.IdDireccion;
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
    }
}
