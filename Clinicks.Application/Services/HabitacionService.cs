using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinicks.Application.Context;
using Clinicks.Application.DTOs;
using Clinicks.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Clinicks.Application.Services;

public class HabitacionService : IHabitacionService
{
    private readonly ClinicksDbContext _context;

    public HabitacionService(ClinicksDbContext context)
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
            // Buscamos si tiene una internación activa (sin fecha de fin)
            var internacionActiva = c.Internaciones.FirstOrDefault(i => i.FechaFin == null);
            var estaOcupada = internacionActiva != null;
            // Si el campo Ocupado dice "Si", o si tiene internación activa, la marcamos como ocupada.
            // Para mantener consistencia con la lógica del usuario, priorizamos la internación activa.
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
