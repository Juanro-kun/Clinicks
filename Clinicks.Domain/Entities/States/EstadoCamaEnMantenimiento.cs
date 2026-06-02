using System;

namespace Clinicks.Domain.Entities.States;

public class EstadoCamaEnMantenimiento : IEstadoCamaState
{
    public int IdEstado => 3;

    public void Ocupar(Cama context)
    {
        throw new InvalidOperationException("No se puede ocupar una cama en mantenimiento.");
    }

    public void Liberar(Cama context)
    {
        context.CambiarEstado(new EstadoCamaLibre());
    }

    public void PonerEnMantenimiento(Cama context)
    {
        throw new InvalidOperationException("La cama ya está en mantenimiento.");
    }
}
