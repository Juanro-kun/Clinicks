using System.Threading.Tasks;

namespace Clinicks.Application.Interfaces
{
    public interface IAuthService
    {
        Task<string?> IniciarSesion(string username, string password);
    }
}
