using System;
using System.Collections.Generic;

namespace Clinicks.Domain.Entities;

public partial class EstadoCama
{
    public int IdEstado { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Cama> Camas { get; set; } = new List<Cama>();
}
