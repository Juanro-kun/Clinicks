using System.Collections.Generic;
using System.Threading.Tasks;
using Clinicks.Application.DTOs.Ubicaciones;
using Clinicks.Application.Interfaces;

namespace Clinicks.Application.Services
{
    public class UbicacionService : IUbicacionService
    {
        private readonly IUbicacionRepository _repository;

        public UbicacionService(IUbicacionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<UbicacionDTO>> ConsultarPais()
        {
            return await _repository.ConsultarPais();
        }

        public async Task<IEnumerable<UbicacionDTO>> ConsultarProvincia(int? idPais = null)
        {
            return await _repository.ConsultarProvincia(idPais);
        }

        public async Task<IEnumerable<UbicacionDTO>> ConsultarCiudad(int? idProvincia = null)
        {
            return await _repository.ConsultarCiudad(idProvincia);
        }

        public async Task<IEnumerable<string>> ConsultarCalle()
        {
            return await _repository.ConsultarCalle();
        }
    }
}
