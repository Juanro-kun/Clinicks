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

        public async Task<bool> VerificaInternacionActiva(int dni)
        {
            return await _context.Internaciones.AnyAsync(i => i.Dni == dni && i.FechaFin == null);
        }

        public async Task<bool> VerificarCamaOcupada(int idHabitacion, int nCama)
        {
            return await _context.Internaciones.AnyAsync(i => i.IdHabitacion == idHabitacion && i.NCama == nCama && i.FechaFin == null);
        }

        public async Task<bool> ExisteCama(int idHabitacion, int nCama)
        {
            return await _context.Camas.AnyAsync(c => c.IdHabitacion == idHabitacion && c.NCama == nCama);
        }

        public async Task ProcesarNuevaInternacion(Internacion nuevaInternacion)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Internaciones.Add(nuevaInternacion);
                await _context.SaveChangesAsync();

                var nuevoIngreso = new Ingreso
                {
                    IdInternacion = nuevaInternacion.IdInternacion,
                    Fecha = DateTime.Now
                };
                _context.Ingresos.Add(nuevoIngreso);

                var cama = await _context.Camas.FirstOrDefaultAsync(c => c.IdHabitacion == nuevaInternacion.IdHabitacion && c.NCama == nuevaInternacion.NCama);
                if (cama != null)
                {
                    cama.Ocupado = "Si";
                    _context.Camas.Update(cama);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<InternacionResponseDto>> ListarInternacionesActivas()
        {
            var internaciones = await _context.Internaciones
                .Include(i => i.PacienteNavigation)
                .Include(i => i.CamaNavigation)
                    .ThenInclude(c => c.HabitacionNavigation)
                .Where(i => i.FechaFin == null)
                .OrderByDescending(i => i.FechaInicio)
                .ToListAsync();

            return internaciones.Select(i => new InternacionResponseDto
            {
                IdInternacion = i.IdInternacion,
                Dni = i.Dni ?? 0,
                NombrePaciente = i.PacienteNavigation?.Nombre ?? "Desconocido",
                ApellidoPaciente = i.PacienteNavigation?.Apellido ?? "",
                HabitacionNombre = i.CamaNavigation?.HabitacionNavigation?.Nombre ?? "Desconocida",
                NCama = i.NCama ?? 0,
                FechaInicio = i.FechaInicio,
                FechaFin = i.FechaFin
            });
        }

        public async Task<bool> ProcesarAltaMedica(int idInternacion)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var internacion = await _context.Internaciones
                    .FirstOrDefaultAsync(i => i.IdInternacion == idInternacion);

                if (internacion == null || internacion.FechaFin != null) 
                {
                    return false;
                }

                internacion.FechaFin = DateTime.Now;
                _context.Internaciones.Update(internacion);

                var egreso = new Egreso
                {
                    IdInternacion = internacion.IdInternacion,
                    Fecha = DateTime.Now
                };
                _context.Egresos.Add(egreso);

                if (internacion.IdHabitacion.HasValue && internacion.NCama.HasValue)
                {
                    var cama = await _context.Camas
                        .FirstOrDefaultAsync(c => c.IdHabitacion == internacion.IdHabitacion && c.NCama == internacion.NCama);
                    
                    if (cama != null)
                    {
                        cama.Ocupado = "No";
                        _context.Camas.Update(cama);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
