using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

    // Propiedades para recibir los datos desde el frontend sin guardarlos directamente en la tabla Paciente
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string? Calle { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public int? Altura { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string? CiudadNombre { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string? ProvinciaNombre { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string? PaisNombre { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public bool EstaInternado { get; set; }
}
