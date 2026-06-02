using System;

namespace Clinicks.Domain.Entities.States;

public class EstadoCamaOcupada : IEstadoCamaState
{
    public int IdEstado => 2;

    public void Ocupar(Cama context)
    {
        throw new InvalidOperationException("La cama ya está ocupada.");
    }

    public void Liberar(Cama context)
    {
        context.CambiarEstado(new EstadoCamaLibre());
    }

    public void PonerEnMantenimiento(Cama context)
    {
        throw new InvalidOperationException("No se puede poner en mantenimiento una cama ocupada.");
    }
}
