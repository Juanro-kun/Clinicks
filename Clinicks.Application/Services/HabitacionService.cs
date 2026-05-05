using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinicks.Application.DTOs;
using Clinicks.Application.Interfaces;

namespace Clinicks.Application.Services;

public class HabitacionService : IHabitacionService
{
    private readonly IHabitacionRepository _repository;

    public HabitacionService(IHabitacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<HabitacionDto>> ListarHabitaciones()
    {
        return await _repository.ListarHabitaciones();
    }

    public async Task<IEnumerable<CamaDto>> ListarTodasLasCamas()
    {
        return await _repository.ListarTodasLasCamas();
    }
}
