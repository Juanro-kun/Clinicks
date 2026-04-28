using System;
using System.Collections.Generic;

namespace Clinicks.Domain.Entities;

public partial class Internacion
{
    public int IdInternacion { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int? Dni { get; set; }
    public int? IdHabitacion { get; set; }
    public int? NCama { get; set; }

    public virtual Paciente? PacienteNavigation { get; set; }
    public virtual Cama? CamaNavigation { get; set; }
    
    public virtual ICollection<Ingreso> Ingresos { get; set; } = new List<Ingreso>();
    public virtual ICollection<Egreso> Egresos { get; set; } = new List<Egreso>();
    public virtual ICollection<Traslado> TrasladosOrigen { get; set; } = new List<Traslado>();
    public virtual ICollection<Traslado> TrasladosDestino { get; set; } = new List<Traslado>();
}
