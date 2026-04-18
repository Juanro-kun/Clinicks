using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Clinicks.Application.Interfaces;
using Clinicks.Domain.Entities;

namespace Clinicks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacientesController : ControllerBase
    {
        private readonly IPacienteService _pacienteService;

        // Inyectamos la Interfaz, no la clase directamente. 
        // Esto es "Desacoplamiento", clave para que el código sea pro.
        public PacientesController(IPacienteService pacienteService)
        {
            _pacienteService = pacienteService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var pacientes = await _pacienteService.GetAllAsync();
            return Ok(pacientes);
        }

        [HttpGet("{dni}")]
        public async Task<IActionResult> GetByDni(int dni)
        {
            var paciente = await _pacienteService.GetByDniAsync(dni);
            if (paciente == null) return NotFound("Paciente no encontrado, wacho.");

            return Ok(paciente);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Paciente paciente)
        {
            await _pacienteService.CreateAsync(paciente);
            return Ok("Paciente creado con éxito en la clinicks_bd!");
        }

        [HttpDelete("{dni}")]
        public async Task<IActionResult> Delete(int dni)
        {
            await _pacienteService.DeleteAsync(dni);
            return Ok("Paciente borrado.");
      
    }
}
