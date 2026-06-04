using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Clinicks.Domain.Entities;

public partial class Paciente
{
    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [Range(1_000_000, 99_999_999, ErrorMessage = "El DNI debe ser un número válido de 7 u 8 cifras.")]
    public int Dni { get; set; }

    [Required(ErrorMessage = "El nombre no puede estar vacío.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "El apellido debe tener entre 1 y 50 caracteres.")]
    public string Apellido { get; set; } = null!;

    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<Direccion> Direcciones { get; set; } = new List<Direccion>();

    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<Internacion> Internaciones { get; set; } = new List<Internacion>();

    [Phone(ErrorMessage = "El formato del teléfono no es válido.")]
    public string? Telefono { get; set; }

    public bool Activo { get; set; } = true;

    public int? CreadoPorUsuarioId { get; set; }
    public virtual Usuario? CreadoPorUsuario { get; set; }

    public bool TieneInternacionActiva()
    {
        return Internaciones.Any(i => i.FechaEgreso == null);
    }

    public void Eliminar()
    {
        if (TieneInternacionActiva())
        {
            throw new InvalidOperationException("No se puede eliminar un paciente que se encuentra internado.");
        }
        Activo = false;
    }
}
