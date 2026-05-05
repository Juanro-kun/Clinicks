using System.Threading.Tasks;
using Clinicks.Domain.Entities;

namespace Clinicks.Application.Interfaces
{
    public interface IAuthRepository
    {
        Task<Usuario?> BuscarUsuarioPorNombre(string username);
    }
}
