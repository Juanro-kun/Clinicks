using System;
using System.Collections.Generic;
using Clinicks.Domain.Entities.States;

namespace Clinicks.Domain.Entities;

public partial class Cama
{
    public Cama()
    {
        // By default EF Core will set IdEstado which will trigger the initialization.
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

    private IEstadoCamaState _estadoActual;

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

    public void CambiarEstado(IEstadoCamaState nuevoEstado)
    {
        _estadoActual = nuevoEstado;
        _idEstado = nuevoEstado.IdEstado;
    }

    public void Ocupar() => _estadoActual?.Ocupar(this);
    public void Liberar() => _estadoActual?.Liberar(this);
    public void PonerEnMantenimiento() => _estadoActual?.PonerEnMantenimiento(this);

    public bool EstaLibre() => _estadoActual is EstadoCamaLibre;
    public bool EstaOcupada => _estadoActual is EstadoCamaOcupada;

    public virtual EstadoCama? EstadoNavigation { get; set; }
    public virtual Habitacion? HabitacionNavigation { get; set; }
    public virtual ICollection<MovimientoCama> MovimientosCama { get; set; } = new List<MovimientoCama>();
}
