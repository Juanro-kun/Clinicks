using System;
using Clinicks.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Clinicks.Domain.UnitTests;

public class PacienteTests
{
    [Fact]
    public void TieneInternacionActiva_ConInternacionSinFechaEgreso_DebeRetornarTrue()
    {
        // Arrange
        var paciente = new Paciente
        {
            Dni = 12345678,
            Nombre = "Juan",
            Apellido = "Perez"
        };
        var internacion = new Internacion
        {
            FechaIngreso = DateTime.Now,
            FechaEgreso = null
        };
        paciente.Internaciones.Add(internacion);

        // Act
        var result = paciente.TieneInternacionActiva();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void TieneInternacionActiva_SinInternacion_DebeRetornarFalse()
    {
        // Arrange
        var paciente = new Paciente
        {
            Dni = 12345678,
            Nombre = "Juan",
            Apellido = "Perez"
        };

        // Act
        var result = paciente.TieneInternacionActiva();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void TieneInternacionActiva_ConInternacionConFechaEgreso_DebeRetornarFalse()
    {
        // Arrange
        var paciente = new Paciente
        {
            Dni = 12345678,
            Nombre = "Juan",
            Apellido = "Perez"
        };
        var internacion = new Internacion
        {
            FechaIngreso = DateTime.Now.AddDays(-2),
            FechaEgreso = DateTime.Now
        };
        paciente.Internaciones.Add(internacion);

        // Act
        var result = paciente.TieneInternacionActiva();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Eliminar_SinInternacionActiva_DebeCambiarActivoAFalse()
    {
        // Arrange
        var paciente = new Paciente
        {
            Dni = 12345678,
            Nombre = "Juan",
            Apellido = "Perez",
            Activo = true
        };

        // Act
        paciente.Eliminar();

        // Assert
        paciente.Activo.Should().BeFalse();
    }

    [Fact]
    public void TieneInternacionActiva_ConVariasInternacionesCerradasYUnaAbierta_DebeRetornarTrue()
    {
        // Arrange
        var paciente = new Paciente { Dni = 12345678, Nombre = "Juan", Apellido = "Perez" };
        paciente.Internaciones.Add(new Internacion { FechaIngreso = DateTime.Now.AddDays(-10), FechaEgreso = DateTime.Now.AddDays(-8) });
        paciente.Internaciones.Add(new Internacion { FechaIngreso = DateTime.Now.AddDays(-5), FechaEgreso = DateTime.Now.AddDays(-2) });
        paciente.Internaciones.Add(new Internacion { FechaIngreso = DateTime.Now, FechaEgreso = null });

        // Act
        var result = paciente.TieneInternacionActiva();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void TieneInternacionActiva_ConVariasInternacionesTodasCerradas_DebeRetornarFalse()
    {
        // Arrange
        var paciente = new Paciente { Dni = 12345678, Nombre = "Juan", Apellido = "Perez" };
        paciente.Internaciones.Add(new Internacion { FechaIngreso = DateTime.Now.AddDays(-10), FechaEgreso = DateTime.Now.AddDays(-8) });
        paciente.Internaciones.Add(new Internacion { FechaIngreso = DateTime.Now.AddDays(-5), FechaEgreso = DateTime.Now.AddDays(-2) });

        // Act
        var result = paciente.TieneInternacionActiva();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Eliminar_ConInternacionActiva_DebeLanzarExcepcion()
    {
        // Arrange
        var paciente = new Paciente
        {
            Dni = 12345678,
            Nombre = "Juan",
            Apellido = "Perez",
            Activo = true
        };
        var internacion = new Internacion
        {
            FechaIngreso = DateTime.Now,
            FechaEgreso = null
        };
        paciente.Internaciones.Add(internacion);

        // Act
        Action act = () => paciente.Eliminar();

        // Assert
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("No se puede eliminar un paciente que se encuentra internado.");
    }
}
