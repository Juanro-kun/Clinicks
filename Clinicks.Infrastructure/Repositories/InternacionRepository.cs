using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinicks.Application.DTOs;
using Clinicks.Application.Interfaces;
using Clinicks.Domain.Entities;
using Clinicks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clinicks.Infrastructure.Repositories
{
    public class InternacionRepository : IInternacionRepository
    {
        private readonly ClinicksDbContext _context;

        public InternacionRepository(ClinicksDbContext context)
        {
            _context = context;
        }

        public async Task<Internacion?> ObtenerInternacionActiva(int dni)
        {
            return await _context.Internaciones.FirstOrDefaultAsync(i => i.Dni == dni && i.FechaEgreso == null);
        }

        public async Task<Internacion?> ObtenerInternacionPorId(int idInternacion)
        {
            return await _context.Internaciones.FirstOrDefaultAsync(i => i.IdInternacion == idInternacion);
        }

        public void Agregar(Internacion internacion)
        {
            _context.Internaciones.Add(internacion);
        }

        public void Modificar(Internacion internacion)
        {
            _context.Internaciones.Update(internacion);
        }

        public async Task<MovimientoCama?> ObtenerMovimientoActivo(int idInternacion)
        {
            return await _context.MovimientosCama.FirstOrDefaultAsync(m => m.IdInternacion == idInternacion && m.FechaFin == null);
        }

        public async Task<MovimientoCama?> ObtenerMovimientoActivoEnCama(int idHabitacion, int nCama)
        {
            return await _context.MovimientosCama.FirstOrDefaultAsync(m => m.IdHabitacion == idHabitacion && m.NCama == nCama && m.FechaFin == null);
        }

        public void ModificarMovimiento(MovimientoCama movimiento)
        {
            _context.MovimientosCama.Update(movimiento);
        }

        public async Task<IEnumerable<InternacionResponseDto>> ListarInternacionesActivas()
        {
            var internaciones = await _context.Internaciones
                .Include(i => i.PacienteNavigation)
                .Where(i => i.FechaEgreso == null)
                .OrderByDescending(i => i.FechaIngreso)
                .ToListAsync();

            var internacionesIds = internaciones.Select(i => i.IdInternacion).ToList();
            
            var movimientosActivos = await _context.MovimientosCama
                .Include(m => m.HabitacionNavigation)
                .Where(m => internacionesIds.Contains(m.IdInternacion) && m.FechaFin == null)
                .ToDictionaryAsync(m => m.IdInternacion);

            return internaciones.Select(i => 
            {
                var mov = movimientosActivos.GetValueOrDefault(i.IdInternacion);
                return new InternacionResponseDto
                {
                    IdInternacion = i.IdInternacion,
                    Dni = i.Dni ?? 0,
                    NombrePaciente = i.PacienteNavigation?.Nombre ?? "Desconocido",
                    ApellidoPaciente = i.PacienteNavigation?.Apellido ?? "",
                    HabitacionNombre = mov?.HabitacionNavigation?.Nombre ?? "Desconocida",
                    NCama = mov?.NCama ?? 0,
                    FechaIngreso = i.FechaIngreso,
                    FechaEgreso = i.FechaEgreso
                };
            });
        }
    }
}
