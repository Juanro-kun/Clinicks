using System;
using System.Collections.Generic;

namespace Clinicks.Domain.Entities;

public partial class Ingreso
{
    public int IdIngreso { get; set; }
    public DateTime? Fecha { get; set; }
    public int? IdInternacion { get; set; }

    public virtual Internacion? InternacionNavigation { get; set; }
}
