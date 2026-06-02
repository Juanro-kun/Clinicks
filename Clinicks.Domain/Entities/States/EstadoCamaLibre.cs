using System;

namespace Clinicks.Domain.Entities.States;

public class EstadoCamaLibre : IEstadoCamaState
{
    public int IdEstado => 1;

    public void Ocupar(Cama context)
    {
        context.CambiarEstado(new EstadoCamaOcupada());
    }

    public void Liberar(Cama context)
    {
        throw new InvalidOperationException("La cama ya está libre.");
    }

    public void PonerEnMantenimiento(Cama context)
    {
        context.CambiarEstado(new EstadoCamaEnMantenimiento());
    }
}
