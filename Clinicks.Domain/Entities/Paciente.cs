using System;
using System.Collections.Generic;

namespace Clinicks.Domain.Entities;

public partial class Paciente
{
    public int Dni { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }
}
