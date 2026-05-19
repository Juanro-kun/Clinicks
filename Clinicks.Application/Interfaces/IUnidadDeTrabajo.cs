using System.Threading.Tasks;

namespace Clinicks.Application.Interfaces;

public interface IUnidadDeTrabajo
{
    Task<int> GuardarCambiosAsync();
}
