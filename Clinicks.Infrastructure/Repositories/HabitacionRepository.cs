using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinicks.Application.DTOs;
using Clinicks.Application.Interfaces;
using Clinicks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clinicks.Infrastructure.Repositories
{
    public class HabitacionRepository : IHabitacionRepository
    {
        private readonly ClinicksDbContext _context;

        public HabitacionRepository(ClinicksDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HabitacionDto>> ListarHabitaciones()
        {
            return await _context.Habitaciones
                .Select(h => new HabitacionDto
                {
                    IdHabitacion = h.IdHabitacion,
                    Nombre = h.Nombre ?? "Sin nombre"
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CamaDto>> ListarTodasLasCamas()
        {
            var camas = await _context.Camas
                .Include(c => c.HabitacionNavigation)
                .Include(c => c.Internaciones)
                    .ThenInclude(i => i.PacienteNavigation)
                .ToListAsync();

            return camas.Select(c => 
            {
                var internacionActiva = c.Internaciones.FirstOrDefault(i => i.FechaFin == null);
                var estaOcupada = internacionActiva != null;

                if (!estaOcupada && c.Ocupado == "Si") {
                    estaOcupada = true;
                }

                return new CamaDto
                {
                    NCama = c.NCama,
                    IdHabitacion = c.IdHabitacion,
                    HabitacionNombre = c.HabitacionNavigation?.Nombre ?? "Desconocida",
                    EstaOcupada = estaOcupada,
                    DniPaciente = internacionActiva?.Dni,
                    NombrePaciente = internacionActiva?.PacienteNavigation?.Nombre,
                    ApellidoPaciente = internacionActiva?.PacienteNavigation?.Apellido,
                    FechaInternacion = internacionActiva?.FechaInicio
                };
            });
        }
    }
}
