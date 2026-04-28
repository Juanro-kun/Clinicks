using System;
using System.Collections.Generic;

namespace Clinicks.Domain.Entities;

public partial class Traslado
{
    public int IdTraslado { get; set; }
    public int? IdInternacionOrigen { get; set; }
    public int? IdInternacionDestino { get; set; }
    public DateTime? Fecha { get; set; }

    public virtual Internacion? InternacionOrigenNavigation { get; set; }
    public virtual Internacion? InternacionDestinoNavigation { get; set; }
}
