using System;
using System.Collections.Generic;

namespace Clinicks.Domain.Entities;

public partial class Usuario
{
    public int UsuarioId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public int Rol { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
}
