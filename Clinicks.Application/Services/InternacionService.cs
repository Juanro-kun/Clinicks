using System;
using System.Linq;
using System.Threading.Tasks;
using Clinicks.Application.Context;
using Clinicks.Application.DTOs;
using Clinicks.Application.Interfaces;
using Clinicks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clinicks.Application.Services;

public class InternacionService : IInternacionService
{
    private readonly ClinicksDbContext _context;

    public InternacionService(ClinicksDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ProcesarInternacionDePaciente(InternacionRequestDto request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Verificar si el paciente ya tiene una internación activa
            var internacionActiva = await VerificaInternacionActiva(request.Dni);

            if (internacionActiva)
            {
                throw new InvalidOperationException("El paciente ya se encuentra internado actualmente.");
            }

            // 2. Verificar si la cama seleccionada está libre
            var cama = await BuscarCama(request.IdHabitacion, request.NCama);

            if (cama == null)
            {
                throw new InvalidOperationException("La cama seleccionada no existe.");
            }

            var camaOcupadaPorOtro = await VerificarCamaOcupada(request.IdHabitacion, request.NCama);

            if (camaOcupadaPorOtro || cama.Ocupado == "Si")
            {
                throw new InvalidOperationException("La cama seleccionada ya está ocupada.");
            }

            // 3. Crear la nueva Internación
            var nuevaInternacion = new Internacion
            {
                Dni = request.Dni,
                IdHabitacion = request.IdHabitacion,
                NCama = request.NCama,
                FechaInicio = DateTime.Now,
                FechaFin = null
            };

            await GuardarNuevaInternacion(nuevaInternacion, cama);

            // 6. Confirmar transacción
            await transaction.CommitAsync();
            return true;
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

            if (internacion == null) return false;
            if (internacion.FechaFin != null) throw new InvalidOperationException("Esta internación ya finalizó.");

            // 1. Dar fecha de fin
            internacion.FechaFin = DateTime.Now;
            _context.Internaciones.Update(internacion);

            // 2. Crear Egreso
            var egreso = new Egreso
            {
                IdInternacion = internacion.IdInternacion,
                Fecha = DateTime.Now
            };
            _context.Egresos.Add(egreso);

            // 3. Liberar la Cama
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

    private async Task<bool> VerificaInternacionActiva(int dni)
    {
        return await _context.Internaciones.AnyAsync(i => i.Dni == dni && i.FechaFin == null);
    }

    private async Task<Cama?> BuscarCama(int idHabitacion, int nCama)
    {
        return await _context.Camas.FirstOrDefaultAsync(c => c.IdHabitacion == idHabitacion && c.NCama == nCama);
    }

    private async Task<bool> VerificarCamaOcupada(int idHabitacion, int nCama)
    {
        return await _context.Internaciones.AnyAsync(i => i.IdHabitacion == idHabitacion && i.NCama == nCama && i.FechaFin == null);
    }

    private async Task GuardarNuevaInternacion(Internacion nuevaInternacion, Cama cama)
    {
        await RegistrarInternacion(nuevaInternacion);

        var nuevoIngreso = new Ingreso
        {
            IdInternacion = nuevaInternacion.IdInternacion,
            Fecha = DateTime.Now
        };
        await RegistrarIngreso(nuevoIngreso);

        await MarcarCamaComoOcupada(cama);
    }

    private async Task RegistrarInternacion(Internacion internacion)
    {
        _context.Internaciones.Add(internacion);
        await _context.SaveChangesAsync();
    }

    private async Task RegistrarIngreso(Ingreso ingreso)
    {
        _context.Ingresos.Add(ingreso);
        await _context.SaveChangesAsync();
    }

    private async Task MarcarCamaComoOcupada(Cama cama)
    {
        cama.Ocupado = "Si";
        _context.Camas.Update(cama);
        await _context.SaveChangesAsync();
    }
}
