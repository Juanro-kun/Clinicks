using System.Collections.Generic;
using System.Threading.Tasks;
using Clinicks.Application.DTOs;

namespace Clinicks.Application.Interfaces;

public interface IHabitacionService
{
    Task<IEnumerable<HabitacionDto>> ListarHabitaciones();
    Task<IEnumerable<CamaDto>> ListarTodasLasCamas();
}
