using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinicks.Application.Interfaces
{
    public interface IUbicacionService
    {
        Task<IEnumerable<string>> ListarNombresDePaises();
        Task<IEnumerable<string>> ListarNombresDeProvincias();
        Task<IEnumerable<string>> ListarNombresDeCiudades();
        Task<IEnumerable<string>> ListarNombresDeCalles();
    }
}
