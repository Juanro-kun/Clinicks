using System.Collections.Generic;
using System.Threading.Tasks;
using Clinicks.Application.DTOs;
using Clinicks.Domain.Entities;

namespace Clinicks.Application.Interfaces
{
    public interface IInternacionRepository
    {
        Task<Internacion?> ObtenerInternacionActiva(int dni);
        Task<Internacion?> ObtenerInternacionPorId(int idInternacion);
        void Agregar(Internacion internacion);
        void Modificar(Internacion internacion);
        Task<MovimientoCama?> ObtenerMovimientoActivo(int idInternacion);
        Task<MovimientoCama?> ObtenerMovimientoActivoEnCama(int idHabitacion, int nCama);
        void ModificarMovimiento(MovimientoCama movimiento);
        Task<IEnumerable<InternacionResponseDto>> ListarInternacionesActivas();
    }
}
