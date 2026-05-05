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