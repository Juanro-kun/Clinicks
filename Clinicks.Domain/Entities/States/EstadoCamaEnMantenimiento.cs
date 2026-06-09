using System;

namespace Clinicks.Domain.Entities.States;

public class EstadoCamaEnMantenimiento : IEstadoCamaState
{
    public int IdEstado => 3;

    public void Ocupar(Cama context)
    {
        context.Fire(Cama.Trigger.Ocupar);
    }

    public void Liberar(Cama context)
    {
        context.Fire(Cama.Trigger.Liberar);
    }

    public void PonerEnMantenimiento(Cama context)
    {
        context.Fire(Cama.Trigger.PonerEnMantenimiento);
    }
}
