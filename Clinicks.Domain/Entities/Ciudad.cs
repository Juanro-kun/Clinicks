using System;
using System.Collections.Generic;

namespace Clinicks.Domain.Entities;

public partial class Ciudad
{
    public int IdCiudad { get; set; }
    public string Nombre { get; set; } = null!;
    public int? IdProvincia { get; set; }

    public virtual Provincia? ProvinciaNavigation { get; set; }
    public virtual ICollection<Direccion> Direcciones { get; set; } = new List<Direccion>();
}
