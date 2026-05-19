using System;
using System.Collections.Generic;

namespace Clinicks.Domain.Entities;

public partial class Cama
{
    public int NCama { get; set; }
    public int IdHabitacion { get; set; }
    
    public int IdEstado { get; set; }
    public virtual EstadoCama? EstadoNavigation { get; set; }
    public virtual Habitacion? HabitacionNavigation { get; set; }
    public virtual ICollection<MovimientoCama> MovimientosCama { get; set; } = new List<MovimientoCama>();
}
