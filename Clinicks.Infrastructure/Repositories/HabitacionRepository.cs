using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinicks.Application.DTOs;
using Clinicks.Application.Interfaces;
using Clinicks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Clinicks.Domain.Entities;
using Clinicks.Domain.Enums;

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
                .Include(c => c.MovimientosCama)
                    .ThenInclude(m => m.InternacionNavigation)
                        .ThenInclude(i => i.PacienteNavigation)
                .ToListAsync();

            return camas.Select(c => 
            {
                var movimientoActivo = c.MovimientosCama.FirstOrDefault(m => m.FechaFin == null);
                var internacionActiva = movimientoActivo?.InternacionNavigation;
                var estaOcupada = movimientoActivo != null;

                if (!estaOcupada && c.IdEstado == (int)EstadoCamaEnum.Ocupada) {
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
                    FechaInternacion = internacionActiva?.FechaIngreso
                };
            });
        }

        public async Task<Cama?> ObtenerCama(int idHabitacion, int nCama)
        {
            return await _context.Camas.FirstOrDefaultAsync(c => c.IdHabitacion == idHabitacion && c.NCama == nCama);
        }

        public void ModificarCama(Cama cama)
        {
            _context.Camas.Update(cama);
        }
    }
}
