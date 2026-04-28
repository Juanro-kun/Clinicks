using System;
using System.Collections.Generic;

namespace Clinicks.Domain.Entities;

public partial class Provincia
{
    public int IdProvincia { get; set; }
    public string Nombre { get; set; } = null!;
    public int? IdPais { get; set; }

    public virtual Pais? PaisNavigation { get; set; }
    public virtual ICollection<Ciudad> Ciudades { get; set; } = new List<Ciudad>();
}
