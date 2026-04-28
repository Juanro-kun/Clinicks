using System.Collections.Generic;
using System.Threading.Tasks;
using Clinicks.Application.DTOs;
using Clinicks.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Clinicks.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HabitacionesController : ControllerBase
{
    private readonly IHabitacionService _habitacionService;

    public HabitacionesController(IHabitacionService habitacionService)
    {
        _habitacionService = habitacionService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HabitacionDto>>> GetHabitaciones()
    {
        var habitaciones = await _habitacionService.ListarHabitaciones();
        return Ok(habitaciones);
    }

    [HttpGet("camas")]
    public async Task<ActionResult<IEnumerable<CamaDto>>> GetCamas()
    {
        var camas = await _habitacionService.ListarTodasLasCamas();
        return Ok(camas);
    }
}
