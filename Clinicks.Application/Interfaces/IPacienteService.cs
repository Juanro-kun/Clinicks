using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clinicks.Domain.Entities;

namespace Clinicks.Application.Interfaces
{
    public interface IPacienteService
    {
        Task<IEnumerable<Paciente>> GetAllAsync();
        Task<Paciente?> GetByDniAsync(int dni);
        Task CreateAsync(Paciente paciente);
        Task<bool> UpdateAsync(Paciente paciente);
        Task DeleteAsync(int dni);
    }
}
