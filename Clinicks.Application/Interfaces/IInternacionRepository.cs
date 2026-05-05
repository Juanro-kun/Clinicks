using System.Collections.Generic;
using System.Threading.Tasks;
using Clinicks.Application.DTOs;
using Clinicks.Domain.Entities;

namespace Clinicks.Application.Interfaces
{
    public interface IInternacionRepository
    {
        Task<bool> VerificaInternacionActiva(int dni);
        Task<bool> VerificarCamaOcupada(int idHabitacion, int nCama);
        Task<bool> ExisteCama(int idHabitacion, int nCama);
        Task ProcesarNuevaInternacion(Internacion nuevaInternacion);
        Task<IEnumerable<InternacionResponseDto>> ListarInternacionesActivas();
        Task<bool> ProcesarAltaMedica(int idInternacion);
    }
}
