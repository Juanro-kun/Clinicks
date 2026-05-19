using System;

namespace Clinicks.Domain.Entities;

public partial class MovimientoCama
{
    public int IdMovimiento { get; set; }
    public int IdInternacion { get; set; }
    public int IdHabitacion { get; set; }
    public int NCama { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }

    public virtual Internacion? InternacionNavigation { get; set; }
    public virtual Habitacion? HabitacionNavigation { get; set; }
    public virtual Cama? CamaNavigation { get; set; }
}
