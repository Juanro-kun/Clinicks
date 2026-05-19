using System.Threading.Tasks;
using Clinicks.Application.Interfaces;

namespace Clinicks.Infrastructure.Persistence;

public class UnidadDeTrabajo : IUnidadDeTrabajo
{
    private readonly ClinicksDbContext _context;

    public UnidadDeTrabajo(ClinicksDbContext context)
    {
        _context = context;
    }

    public async Task<int> GuardarCambiosAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
