using System;
using System.Threading.Tasks;
using Clinicks.Application.DTOs;
using Clinicks.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Clinicks.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class InternacionesController : ControllerBase
{
    private readonly IInternacionService _internacionService;

    public InternacionesController(IInternacionService internacionService)
    {
        _internacionService = internacionService;
    }

    [HttpPost("internar")]
    public async Task<IActionResult> InternarPaciente([FromBody] InternacionRequestDto request)
    {
        var exito = await _internacionService.ProcesarInternacionDePaciente(request);
        if (exito)
        {
            return Ok(new { message = "Paciente internado con éxito." });
        }
        return BadRequest("No se pudo internar al paciente.");
    }

    [HttpGet("activas")]
    public async Task<IActionResult> ObtenerInternacionesActivas()
    {
        var internaciones = await _internacionService.ListarInternacionesActivas();
        return Ok(internaciones);
    }

    [HttpPost("{id}/alta")]
    public async Task<IActionResult> DarDeAlta(int id)
    {
        var exito = await _internacionService.ProcesarAltaMedica(id);
        if (exito)
        {
            return Ok(new { message = "Paciente dado de alta exitosamente." });
        }
        return NotFound("Internación no encontrada.");
    }
}
