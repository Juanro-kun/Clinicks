using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Clinicks.Application.DTOs.Pacientes;
using Clinicks.Application.Interfaces;
using Clinicks.Domain.Entities;
using Clinicks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clinicks.Infrastructure.Repositories
{
    public class PacienteRepository : IPacienteRepository
    {
        private readonly ClinicksDbContext _context;

        public PacienteRepository(ClinicksDbContext context)
        {
            _context = context;
        }

        private static Expression<Func<Paciente, PacienteResponseDTO>> MapToResponseDTO => p => new PacienteResponseDTO
        {
            Dni = p.Dni,
            Nombre = p.Nombre,
            Apellido = p.Apellido,
            Telefono = p.Telefono,
            Calle = p.Direcciones.Select(d => d.Calle).FirstOrDefault(),
            Altura = p.Direcciones.Select(d => (int?)d.Altura).FirstOrDefault(),
            CiudadNombre = p.Direcciones.Select(d => d.CiudadNavigation.Nombre).FirstOrDefault(),
            ProvinciaNombre = p.Direcciones.Select(d => d.CiudadNavigation.ProvinciaNavigation.Nombre).FirstOrDefault(),
            PaisNombre = p.Direcciones.Select(d => d.CiudadNavigation.ProvinciaNavigation.PaisNavigation.Nombre).FirstOrDefault(),
            IdCiudad = p.Direcciones.Select(d => d.IdCiudad).FirstOrDefault(),
            IdProvincia = p.Direcciones.Select(d => d.CiudadNavigation.IdProvincia).FirstOrDefault(),
            IdPais = p.Direcciones.Select(d => d.CiudadNavigation.ProvinciaNavigation.IdPais).FirstOrDefault(),
            EstaInternado = p.Internaciones.Any(i => i.FechaEgreso == null),
            Activo = p.Activo
        };

        public async Task<Application.DTOs.Shared.PaginatedResult<PacienteResponseDTO>> ListarPacientes(int page = 1, int pageSize = 15, bool fetchAll = false)
        {
            var query = _context.Pacientes.AsQueryable();

            var totalCount = await query.CountAsync();

            var resultQuery = query.Select(MapToResponseDTO);

            if (!fetchAll)
            {
                resultQuery = resultQuery.Skip((page - 1) * pageSize).Take(pageSize);
            }
            else
            {
                page = 1;
                pageSize = totalCount > 0 ? totalCount : 1;
            }

            var items = await resultQuery.ToListAsync();

            return new Application.DTOs.Shared.PaginatedResult<PacienteResponseDTO>
            {
                Items = items,
                TotalCount = totalCount,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<Application.DTOs.Shared.PaginatedResult<PacienteResponseDTO>> BuscarPacientes(string terminoBusqueda, int page = 1, int pageSize = 15)
        {
            var query = _context.Pacientes.AsQueryable();
            var terminoLimpio = terminoBusqueda.Trim().ToLower();

            if (int.TryParse(terminoLimpio, out _))
            {
                query = query.Where(p => p.Dni.ToString().Contains(terminoLimpio));
            }
            else
            {
                var palabras = terminoLimpio.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                
                string collation = "SQL_Latin1_General_CP1_CI_AI";

                if (palabras.Length == 1)
                {
                    query = query.Where(p => 
                        EF.Functions.Collate(p.Nombre, collation).Contains(terminoLimpio) || 
                        EF.Functions.Collate(p.Apellido, collation).Contains(terminoLimpio));
                }
                else if (palabras.Length >= 2)
                {
                    query = query.Where(p => 
                        EF.Functions.Collate(p.Nombre + " " + p.Apellido, collation).Contains(terminoLimpio) ||
                        EF.Functions.Collate(p.Apellido + " " + p.Nombre, collation).Contains(terminoLimpio)
                    );
                }
            }

            var totalCount = await query.CountAsync();
            var resultQuery = query.Select(MapToResponseDTO)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize);

            var items = await resultQuery.ToListAsync();

            return new Application.DTOs.Shared.PaginatedResult<PacienteResponseDTO>
            {
                Items = items,
                TotalCount = totalCount,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<PacienteResponseDTO?> BuscarPacientePorDni(int dni)
        {
            return await _context.Pacientes
                .Where(p => p.Dni == dni)
                .Select(MapToResponseDTO)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ConsultarPaciente(int dni)
        {
            var result = await _context.Database
                .SqlQuery<int>($"EXEC sp_ConsultarPaciente_Count @Dni={dni}")
                .ToListAsync();
                
            return result.FirstOrDefault() > 0;
        }

        public async Task<Paciente?> ObtenerPacientePorDni(int dni)
        {
            return await _context.Pacientes
                .Include(p => p.Internaciones)
                    .ThenInclude(i => i.MovimientosCama)
                .Include(p => p.Direcciones)
                .FirstOrDefaultAsync(p => p.Dni == dni);
        }

        public void Agregar(Paciente paciente)
        {
            _context.Pacientes.Add(paciente);
        }

        public void Modificar(Paciente paciente)
        {
            _context.Database.ExecuteSqlRaw(
                "EXEC sp_ActualizarPaciente @Dni={0}, @Nombre={1}, @Apellido={2}, @Telefono={3}, @Activo={4}",
                paciente.Dni, paciente.Nombre, paciente.Apellido, paciente.Telefono, paciente.Activo
            );
        }
    }
}
