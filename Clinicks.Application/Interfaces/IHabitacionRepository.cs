using System.Collections.Generic;
using System.Threading.Tasks;
using Clinicks.Application.DTOs;

namespace Clinicks.Application.Interfaces
{
    public interface IHabitacionRepository
    {
        Task<IEnumerable<HabitacionDto>> ListarHabitaciones();
        Task<IEnumerable<CamaDto>> ListarTodasLasCamas();
        Task<Domain.Entities.Cama?> ObtenerCama(int idHabitacion, int nCama);
        void ModificarCama(Domain.Entities.Cama cama);
    }
}
