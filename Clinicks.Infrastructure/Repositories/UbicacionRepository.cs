using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinicks.Application.Interfaces;
using Clinicks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clinicks.Infrastructure.Repositories
{
    public class UbicacionRepository : IUbicacionRepository
    {
        private readonly ClinicksDbContext _context;

        public UbicacionRepository(ClinicksDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<string>> ConsultarPais()
        {
            return await _context.Paises
                .Where(p => !string.IsNullOrEmpty(p.Nombre))
                .Select(p => p.Nombre)
                .Distinct()
                .OrderBy(nombre => nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> ConsultarProvincia()
        {
            return await _context.Provincias
                .Where(p => !string.IsNullOrEmpty(p.Nombre))
                .Select(p => p.Nombre)
                .Distinct()
                .OrderBy(nombre => nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> ConsultarCiudad()
        {
            return await _context.Ciudades
                .Where(c => !string.IsNullOrEmpty(c.Nombre))
                .Select(c => c.Nombre)
                .Distinct()
                .OrderBy(nombre => nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> ConsultarCalle()
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
