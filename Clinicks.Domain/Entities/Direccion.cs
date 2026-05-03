using System;
using System.Collections.Generic;

namespace Clinicks.Domain.Entities;

public partial class Direccion
{
    public int IdDireccion { get; set; }
    public string Calle { get; set; } = null!;
    public int Altura { get; set; }
    public int? IdCiudad { get; set; }
    public int? Dni { get; set; }

    public virtual Ciudad? CiudadNavigation { get; set; }
    public virtual Paciente? PacienteNavigation { get; set; }
}
