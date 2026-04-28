using System;
using System.Collections.Generic;

namespace Clinicks.Domain.Entities;

public partial class Pais
{
    public int IdPais { get; set; }
    public string Nombre { get; set; } = null!;

    public virtual ICollection<Provincia> Provincias { get; set; } = new List<Provincia>();
}
