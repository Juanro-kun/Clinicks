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

        public async Task<IEnumerable<Paciente>> GetAllAsync()
        {
            return await _context.Pacientes.ToListAsync();
        }

        public async Task<Paciente?> GetByDniAsync(int dni)
        {
            return await _context.Pacientes.FindAsync(dni);
        }

        public async Task CreateAsync(Paciente paciente)
        {
            // Acá podrías meter una validación: si el DNI ya existe, tirar un error.
            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Paciente paciente)
        {
            _context.Entry(paciente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true; // Salió todo joya
            }
            catch (DbUpdateConcurrencyException)
            {
                // Si saltó este error, es muy probable que el paciente no exista
                if (!PacienteExists(paciente.Dni)) return false;
                throw; // Si es otro error raro, que explote
            }
        }

        // Un helper privado en el service para chequear si está
        private bool PacienteExists(int dni)
        {
            return _context.Pacientes.Any(e => e.Dni == dni);
        }

        public async Task DeleteAsync(int dni)
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
