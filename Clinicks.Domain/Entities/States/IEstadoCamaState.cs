namespace Clinicks.Domain.Entities.States;

public interface IEstadoCamaState
{
    void Ocupar(Cama context);
    void Liberar(Cama context);
    void PonerEnMantenimiento(Cama context);
    int IdEstado { get; }
}
