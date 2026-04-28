using System;
using System.Collections.Generic;

namespace Clinicks.Domain.Entities;

public partial class Egreso
{
    public int IdEgreso { get; set; }
    public DateTime? Fecha { get; set; }
    public int? IdInternacion { get; set; }

    public virtual Internacion? InternacionNavigation { get; set; }
}
