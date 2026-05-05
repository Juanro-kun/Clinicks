using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinicks.Application.Interfaces
{
    public interface IUbicacionRepository
    {
        Task<IEnumerable<string>> ConsultarPais();
        Task<IEnumerable<string>> ConsultarProvincia();
        Task<IEnumerable<string>> ConsultarCiudad();
        Task<IEnumerable<string>> ConsultarCalle();
    }
}
