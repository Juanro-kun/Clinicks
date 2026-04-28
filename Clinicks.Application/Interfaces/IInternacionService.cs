using System.Threading.Tasks;
using Clinicks.Application.DTOs;

namespace Clinicks.Application.Interfaces;

public interface IInternacionService
{
    Task<bool> ProcesarInternacionDePaciente(InternacionRequestDto request);
    Task<IEnumerable<InternacionResponseDto>> ListarInternacionesActivas();
    Task<bool> ProcesarAltaMedica(int idInternacion);
}
