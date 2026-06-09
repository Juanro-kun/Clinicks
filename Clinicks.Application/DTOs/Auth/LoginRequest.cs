using System.ComponentModel.DataAnnotations;

namespace Clinicks.Application.DTOs.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 50 caracteres.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "La contraseña no es válida.")]
        public string Password { get; set; } = string.Empty;
    }
}
