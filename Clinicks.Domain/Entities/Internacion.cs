using System;
using System.Collections.Generic;

namespace Clinicks.Domain.Entities;

public partial class Internacion
{
    public int IdInternacion { get; set; }
    public DateTime? FechaIngreso { get; set; }
    public DateTime? FechaEgreso { get; set; }
    public int? Dni { get; set; }

    public virtual Paciente? PacienteNavigation { get; set; }
    
    public virtual ICollection<MovimientoCama> MovimientosCama { get; set; } = new List<MovimientoCama>();
}
