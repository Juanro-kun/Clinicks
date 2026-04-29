using System.Threading.Tasks;
using Clinicks.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinicks.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UbicacionesController : ControllerBase
    {
        private readonly IUbicacionService _ubicacionService;

        public UbicacionesController(IUbicacionService ubicacionService)
        {
            _ubicacionService = ubicacionService;
        }

        [HttpGet("paises")]
        public async Task<IActionResult> ObtenerPaises()
        {
            var paises = await _ubicacionService.ListarNombresDePaises();
            return Ok(paises);
        }

        [HttpGet("provincias")]
        public async Task<IActionResult> ObtenerProvincias()
        {
            var provincias = await _ubicacionService.ListarNombresDeProvincias();
            return Ok(provincias);
        }

        [HttpGet("ciudades")]
        public async Task<IActionResult> ObtenerCiudades()
        {
            var ciudades = await _ubicacionService.ListarNombresDeCiudades();
            return Ok(ciudades);
        }

        [HttpGet("calles")]
        public async Task<IActionResult> ObtenerCalles()
        {
            var calles = await _ubicacionService.ListarNombresDeCalles();
            return Ok(calles);
        }
    }
}
