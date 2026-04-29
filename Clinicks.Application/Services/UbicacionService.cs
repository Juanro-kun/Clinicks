using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinicks.Application.Context;
using Clinicks.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Clinicks.Application.Services
{
    public class UbicacionService : IUbicacionService
    {
        private readonly ClinicksDbContext _context;

        public UbicacionService(ClinicksDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<string>> ListarNombresDePaises()
        {
            return await _context.Paises
                .Where(p => !string.IsNullOrEmpty(p.Nombre))
                .Select(p => p.Nombre)
                .Distinct()
                .OrderBy(nombre => nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> ListarNombresDeProvincias()
        {
            return await _context.Provincias
                .Where(p => !string.IsNullOrEmpty(p.Nombre))
                .Select(p => p.Nombre)
                .Distinct()
                .OrderBy(nombre => nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> ListarNombresDeCiudades()
        {
            return await _context.Ciudades
                .Where(c => !string.IsNullOrEmpty(c.Nombre))
                .Select(c => c.Nombre)
                .Distinct()
                .OrderBy(nombre => nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> ListarNombresDeCalles()
        {
            return await _context.Direcciones
                .Where(d => !string.IsNullOrEmpty(d.Calle))
                .Select(d => d.Calle)
                .Distinct()
                .OrderBy(calle => calle)
                .ToListAsync();
        }
    }
}
