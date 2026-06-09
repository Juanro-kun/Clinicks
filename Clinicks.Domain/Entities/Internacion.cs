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

    public static Internacion CrearInternacion(int dni, Cama cama)
    {
        var internacion = new Internacion
        {
            Dni = dni,
            FechaIngreso = DateTime.Now,
            FechaEgreso = null
        };

        var movimiento = MovimientoCama.CrearMovimientoCama(cama);
        internacion.MovimientosCama.Add(movimiento);

        return internacion;
    }

    // Busca el movimiento que no tiene fecha de fin (es decir, la cama donde se encuentra actualmente).
    private MovimientoCama? ObtenerMovimientoActivo()
    {
        return MovimientosCama.FirstOrDefault(m => m.FechaFin == null);
    }

    // Cierra el uso de la cama actual e inicia uno nuevo en la cama de destino.
    public void RegistrarTraslado(Cama nuevaCama)
    {
        var movimientoActual = ObtenerMovimientoActivo();
        if (movimientoActual != null)
        {
            movimientoActual.FinalizarMovimiento();
        }

        var nuevoMovimiento = MovimientoCama.CrearMovimientoCama(nuevaCama);
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

