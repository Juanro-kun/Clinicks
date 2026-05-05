using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<string>> ConsultarPais()
        {
            return await _repository.ConsultarPais();
        }

        public async Task<IEnumerable<string>> ConsultarProvincia()
        {
            return await _repository.ConsultarProvincia();
        }

        public async Task<IEnumerable<string>> ConsultarCiudad()
        {
            return await _repository.ConsultarCiudad();
        }

        public async Task<IEnumerable<string>> ConsultarCalle()
        {
            return await _repository.ConsultarCalle();
        }
    }
}
