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

        public async Task<bool> ProcesarAltaDePaciente(Paciente paciente)
        {
           
            // Buscamos si ya existe alguien con ese DNI
            var existe = await VerificarPaciente(paciente.Dni);

            if (existe)
            {
                // Si ya existe, no hacemos nada y devolvemos false
                return false;
            }

            // Resolvemos o creamos la dirección primero
            await GestionarDireccionDelPaciente(paciente);

            // 2. Si llegamos acá, es porque el DNI está libre
            await GuardarPaciente(paciente);

            return true;    
        }

        public async Task<bool> ActualizarDatosPaciente(Paciente paciente)
        {
            // Resolvemos o creamos la dirección
            await GestionarDireccionDelPaciente(paciente);

            _context.Entry(paciente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true; // Salió todo joya
            }
            catch (DbUpdateConcurrencyException)
            {
                // Si saltó este error, es muy probable que el paciente no exista
                if (!await VerificarPaciente(paciente.Dni)) return false;
                throw; // Si es otro error raro, que explote
            }
        }

        private async Task<bool> VerificarPaciente(int dni)
        {
            return await _context.Pacientes.AnyAsync(e => e.Dni == dni);
        }

        private async Task GuardarPaciente(Paciente paciente)
        {
            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();
        }

        private async Task GestionarDireccionDelPaciente(Paciente paciente)
        {
            if (!string.IsNullOrWhiteSpace(paciente.Calle) && paciente.Altura.HasValue)
            {
                var paisNombre = string.IsNullOrWhiteSpace(paciente.PaisNombre) ? "Desconocido" : paciente.PaisNombre.Trim();
                var pais = await BuscarPaisPorNombre(paisNombre);
                if (pais == null)
                {
                    pais = new Pais { Nombre = paisNombre };
                    await RegistrarNuevoPais(pais);
                }

                var provNombre = string.IsNullOrWhiteSpace(paciente.ProvinciaNombre) ? "Desconocida" : paciente.ProvinciaNombre.Trim();
                var provincia = await BuscarProvinciaPorNombre(provNombre, pais.IdPais);
                if (provincia == null)
                {
                    provincia = new Provincia { Nombre = provNombre, IdPais = pais.IdPais };
                    await RegistrarNuevaProvincia(provincia);
                }

                var ciudadNombre = string.IsNullOrWhiteSpace(paciente.CiudadNombre) ? "Desconocida" : paciente.CiudadNombre.Trim();
                var ciudad = await BuscarCiudadPorNombre(ciudadNombre, provincia.IdProvincia);
                if (ciudad == null)
                {
                    ciudad = new Ciudad { Nombre = ciudadNombre, IdProvincia = provincia.IdProvincia };
                    await RegistrarNuevaCiudad(ciudad);
                }

                // Siempre creamos una nueva instancia de Direccion para el paciente
                // (Para evitar que si dos pacientes viven en la misma calle y uno la edita, le cambie al otro)
                var direccion = new Direccion { Calle = paciente.Calle.Trim(), Altura = paciente.Altura.Value, IdCiudad = ciudad.IdCiudad };
                await RegistrarNuevaDireccion(direccion);

                paciente.IdDireccion = direccion.IdDireccion;
            }
            else
            {
                paciente.IdDireccion = null;
            }
        }

        private async Task<Pais?> BuscarPaisPorNombre(string nombre)
        {
            return await _context.Paises.FirstOrDefaultAsync(p => p.Nombre == nombre);
        }

        private async Task RegistrarNuevoPais(Pais pais)
        {
            _context.Paises.Add(pais);
            await _context.SaveChangesAsync();
        }

        private async Task<Provincia?> BuscarProvinciaPorNombre(string nombre, int idPais)
        {
            return await _context.Provincias.FirstOrDefaultAsync(p => p.Nombre == nombre && p.IdPais == idPais);
        }

        private async Task RegistrarNuevaProvincia(Provincia provincia)
        {
            _context.Provincias.Add(provincia);
            await _context.SaveChangesAsync();
        }

        private async Task<Ciudad?> BuscarCiudadPorNombre(string nombre, int idProvincia)
        {
            return await _context.Ciudades.FirstOrDefaultAsync(c => c.Nombre == nombre && c.IdProvincia == idProvincia);
        }

        private async Task RegistrarNuevaCiudad(Ciudad ciudad)
        {
            _context.Ciudades.Add(ciudad);
            await _context.SaveChangesAsync();
        }

        private async Task RegistrarNuevaDireccion(Direccion direccion)
        {
            _context.Direcciones.Add(direccion);
            await _context.SaveChangesAsync();
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
