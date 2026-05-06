using System.Collections.Generic;
using System.Threading.Tasks;
using Clinicks.Application.DTOs.Ubicaciones;

namespace Clinicks.Application.Interfaces
{
    public interface IUbicacionRepository
    {
        Task<IEnumerable<UbicacionDTO>> ConsultarPais();
        Task<IEnumerable<UbicacionDTO>> ConsultarProvincia(int? idPais = null);
        Task<IEnumerable<UbicacionDTO>> ConsultarCiudad(int? idProvincia = null);
        Task<IEnumerable<string>> ConsultarCalle();
    }
}
