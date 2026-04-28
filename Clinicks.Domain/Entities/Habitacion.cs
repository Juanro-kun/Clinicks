using System;
using System.Collections.Generic;

namespace Clinicks.Domain.Entities;

public partial class Habitacion
{
    public int IdHabitacion { get; set; }
    public string? Nombre { get; set; }

    public virtual ICollection<Cama> Camas { get; set; } = new List<Cama>();
}
