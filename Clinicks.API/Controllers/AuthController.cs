using Microsoft.AspNetCore.Mvc;
using Clinicks.Application.Services;
using Clinicks.Application.DTOs.Auth;
using Clinicks.Application.Interfaces;

namespace Clinicks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IPasswordHasher _passwordHasher;

        public AuthController(IAuthService authService, IPasswordHasher passwordHasher)
        {
            _authService = authService;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _authService.IniciarSesion(request.Username, request.Password);
            
            if (token == null)
                return Unauthorized("Usuario o contraseña incorrectos, wacho.");

            return Ok(new { Token = token });
        }

        [HttpGet("crear-hash/{password}")]
        public IActionResult GetHash(string password)
        {
            // Esto genera el hash exacto que tu sistema va a entender
            string hash = _passwordHasher.HashPassword(password);
            return Ok(hash);
        }
    }

}