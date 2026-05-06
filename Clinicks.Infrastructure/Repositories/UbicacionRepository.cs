using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinicks.Application.DTOs.Ubicaciones;
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

        public async Task<IEnumerable<UbicacionDTO>> ConsultarPais()
        {
            return await _context.Paises
                .Where(p => !string.IsNullOrEmpty(p.Nombre))
                .Select(p => new UbicacionDTO { Id = p.IdPais, Nombre = p.Nombre })
                .Distinct()
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<UbicacionDTO>> ConsultarProvincia(int? idPais = null)
        {
            return await _context.Provincias
                .Where(p => !string.IsNullOrEmpty(p.Nombre) && (!idPais.HasValue || p.IdPais == idPais))
                .Select(p => new UbicacionDTO { Id = p.IdProvincia, Nombre = p.Nombre })
                .Distinct()
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<UbicacionDTO>> ConsultarCiudad(int? idProvincia = null)
        {
            return await _context.Ciudades
                .Where(c => !string.IsNullOrEmpty(c.Nombre) && (!idProvincia.HasValue || c.IdProvincia == idProvincia))
                .Select(c => new UbicacionDTO { Id = c.IdCiudad, Nombre = c.Nombre })
                .Distinct()
                .OrderBy(c => c.Nombre)
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
