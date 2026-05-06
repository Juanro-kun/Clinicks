using System.Threading.Tasks;
using Clinicks.Application.Interfaces;

namespace Clinicks.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;
        private readonly ITokenProvider _tokenProvider;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(
            IAuthRepository repository, 
            ITokenProvider tokenProvider,
            IPasswordHasher passwordHasher)
        {
            _repository = repository;
            _tokenProvider = tokenProvider;
            _passwordHasher = passwordHasher;
        }

        public async Task<string?> IniciarSesion(string username, string password)
        {
            var usuario = await _repository.BuscarUsuarioPorNombre(username);

            if (usuario == null || !_passwordHasher.VerifyPassword(password, usuario.Password))
                return null;

            return _tokenProvider.GenerarToken(usuario);
        }
    }
}

/*
CODIGO ANTERIOR
namespace Clinicks.Application.Services
{
    public class AuthService
    {
        private readonly ClinicksDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(ClinicksDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<string?> LoginAsync(string username, string password)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == username);

            // Verificamos si existe y si el hash de la clave coincide
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(password, usuario.Password))
                return null;

            return GenerarToken(usuario);
        }

        private string GenerarToken(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim("UsuarioId", usuario.UsuarioId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
*/