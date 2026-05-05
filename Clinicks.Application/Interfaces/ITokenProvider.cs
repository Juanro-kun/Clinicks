using Clinicks.Domain.Entities;

namespace Clinicks.Application.Interfaces
{
    public interface ITokenProvider
    {
        string GenerarToken(Usuario usuario);
    }
}
