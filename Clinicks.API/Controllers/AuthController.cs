using Microsoft.AspNetCore.Mvc;
using Clinicks.Application.Services;

namespace Clinicks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
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
            string hash = BCrypt.Net.BCrypt.HashPassword(password);
            return Ok(hash);
        }
    }

    // Un DTO simple para recibir los datos del login
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}