using System.Threading.Tasks;
using Clinicks.Application.Interfaces;
using Clinicks.Domain.Entities;
using Clinicks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clinicks.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ClinicksDbContext _context;

        public AuthRepository(ClinicksDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> BuscarUsuarioPorNombre(string username)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == username);
        }
    }
}
