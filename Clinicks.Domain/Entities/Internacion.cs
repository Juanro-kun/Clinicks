using System;
using System.Collections.Generic;
using System.Linq;

namespace Clinicks.Domain.Entities;

public partial class Internacion
{
    public int IdInternacion { get; set; }
    public DateTime? FechaIngreso { get; set; }
    public DateTime? FechaEgreso { get; set; }
    public int? Dni { get; set; }
    public int? CreadoPorUsuarioId { get; set; }

    public virtual Paciente? PacienteNavigation { get; set; }
    public virtual Usuario? CreadoPorUsuario { get; set; }
    
    public virtual ICollection<MovimientoCama> MovimientosCama { get; set; } = new List<MovimientoCama>();

    public static Internacion Crear(int dni, Cama cama)
    {
        var internacion = new Internacion
        {
            Dni = dni,
            FechaIngreso = DateTime.Now,
            FechaEgreso = null
        };

        var movimiento = new MovimientoCama(cama);
        internacion.MovimientosCama.Add(movimiento);
        
        return internacion;
    }

    private MovimientoCama? ObtenerMovimientoActivo()
    {
        return MovimientosCama.FirstOrDefault(m => m.FechaFin == null);
    }

    public void RegistrarTraslado(Cama nuevaCama)
    {
        var movimientoActual = ObtenerMovimientoActivo();
        if (movimientoActual != null)
        {
            movimientoActual.FinalizarMovimiento();
        }

        var nuevoMovimiento = new MovimientoCama(nuevaCama);
        MovimientosCama.Add(nuevoMovimiento);
    }

    public void RegistrarAlta()
    {
        FechaEgreso = DateTime.Now;

        var movimientoActual = ObtenerMovimientoActivo();
        if (movimientoActual != null)
        {
            movimientoActual.FinalizarMovimiento();
        }
    }
}

