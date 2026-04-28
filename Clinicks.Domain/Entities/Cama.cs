using System;
using System.Collections.Generic;

namespace Clinicks.Domain.Entities;

public partial class Cama
{
    public int NCama { get; set; }
    public int IdHabitacion { get; set; }
    
    // El usuario especificó VARCHAR(10) ('Si'/'No') o BIT.
    // Lo mapearemos como string porque SQL Server lo tiene como VARCHAR.
    public string? Ocupado { get; set; }

    public virtual Habitacion? HabitacionNavigation { get; set; }
    public virtual ICollection<Internacion> Internaciones { get; set; } = new List<Internacion>();
}
