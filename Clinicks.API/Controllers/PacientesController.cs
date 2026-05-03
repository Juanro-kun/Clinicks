using Clinicks.Application.Interfaces;
using Clinicks.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinicks.API.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> ObtenerTodosLosPacientes()
        {
            var pacientes = await _pacienteService.ListarPacientes();
            return Ok(pacientes);
        }

        [HttpGet("{dni}")]
        public async Task<IActionResult> ObtenerPacientePorDni(int dni)
        {
            var paciente = await _pacienteService.BuscarPacientePorDni(dni);
            if (paciente == null) return NotFound("Paciente no encontrado");

            return Ok(paciente);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarNuevoPaciente(Paciente paciente)
        {
            var existe = await _pacienteService.ConsultarPaciente(paciente.Dni);

            if (existe)
            {
                return Conflict("El DNI ya se encuentra registrado.");
            }

            try
            {
                await _pacienteService.RegistrarNuevoPaciente(
                    paciente.Dni, 
                    paciente.Nombre, 
                    paciente.Apellido, 
                    paciente.Telefono, 
                    paciente.Calle, 
                    paciente.Altura ?? 0, 
                    paciente.CiudadNombre, 
                    paciente.ProvinciaNombre, 
                    paciente.PaisNombre);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            return CreatedAtAction(nameof(RegistrarNuevoPaciente), new { id = paciente.Dni }, paciente);
        }

        [HttpDelete("{dni}")]
        public async Task<IActionResult> EliminarPaciente(int dni)
        {
            await _pacienteService.EliminarPaciente(dni);
            return Ok("Paciente borrado.");

        }

        [HttpPut("{dni}")]
        public async Task<IActionResult> ActualizarDatosPaciente(int dni, Paciente paciente)
        {
            if (dni != paciente.Dni) return BadRequest("El DNI no coincide");

            // El service hace el laburo sucio
            bool actualizado = await _pacienteService.ActualizarDatosPaciente(paciente);

            if (!actualizado)
            {
                return NotFound($"No se encontró ningún paciente con DNI {dni}");
            }

            return NoContent(); // 204: "Todo bien, no tengo nada más que decirte"
        }
    }
}
