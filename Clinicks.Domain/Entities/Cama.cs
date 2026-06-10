using System;
using System.Collections.Generic;
using Clinicks.Domain.Entities.States;
using Stateless;

namespace Clinicks.Domain.Entities;

public partial class Cama
{
    public Cama()
    {
        // By default EF Core will set IdEstado which will trigger the initialization.
        // However, when created manually, we need to initialize the state explicitly.
        InicializarEstadoDesdeId();
    }

    public int NCama { get; set; }
    public int IdHabitacion { get; set; }

    private int _idEstado = 1;
    public int IdEstado
    {
        get => _idEstado;
        set
        {
            _idEstado = value;
            InicializarEstadoDesdeId();
        }
    }

    private IEstadoCamaState _estadoActual = null!;

    public void Ocupar() => _estadoActual?.Ocupar(this);
    public void Liberar() => _estadoActual?.Liberar(this);
    public void PonerEnMantenimiento() => _estadoActual?.PonerEnMantenimiento(this);

    // Resuelve dinámicamente la instancia del estado (State Pattern) según el ID en base de datos.
    private void InicializarEstadoDesdeId()
    {
        _estadoActual = _idEstado switch
        {
            1 => new EstadoCamaLibre(),
            2 => new EstadoCamaOcupada(),
            3 => new EstadoCamaEnMantenimiento(),
            _ => new EstadoCamaLibre()
        };
    }

    private StateMachine<int, Trigger>? _stateMachine;
    private StateMachine<int, Trigger> StateMachine
    {
        get
        {
            if (_stateMachine == null)
            {
                _stateMachine = new StateMachine<int, Trigger>(() => IdEstado, s => IdEstado = s);

                _stateMachine.Configure(1) // Libre
                    .Permit(Trigger.Ocupar, 2)
                    .Permit(Trigger.PonerEnMantenimiento, 3);

                _stateMachine.Configure(2) // Ocupada
                    .Permit(Trigger.Liberar, 1);

                _stateMachine.Configure(3) // En Mantenimiento
                    .Permit(Trigger.Liberar, 1);

                _stateMachine.OnUnhandledTrigger((state, trigger, unmetGuardConditions) =>
                {
                    string message = (state, trigger) switch
                    {
                        (1, Trigger.Liberar) => "La cama ya está libre.",
                        (2, Trigger.Ocupar) => "La cama ya está ocupada.",
                        (2, Trigger.PonerEnMantenimiento) => "No se puede poner en mantenimiento una cama ocupada.",
                        (3, Trigger.Ocupar) => "No se puede ocupar una cama en mantenimiento.",
                        (3, Trigger.PonerEnMantenimiento) => "La cama ya está en mantenimiento.",
                        _ => $"Transición inválida desde el estado {state} con el disparador {trigger}."
                    };
                    throw new InvalidOperationException(message);
                });
            }
            return _stateMachine;
        }
    }

    internal enum Trigger
    {
        Ocupar,
        Liberar,
        PonerEnMantenimiento
    }

    internal void Fire(Trigger trigger)
    {
        StateMachine.Fire(trigger);
    }

    

    public bool EstaLibre() => _estadoActual is EstadoCamaLibre;
    public bool EstaOcupada => _estadoActual is EstadoCamaOcupada;

    public virtual EstadoCama? EstadoNavigation { get; set; }
    public virtual Habitacion? HabitacionNavigation { get; set; }
    public virtual ICollection<MovimientoCama> MovimientosCama { get; set; } = new List<MovimientoCama>();
}
