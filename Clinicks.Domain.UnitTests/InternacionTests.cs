using System;
using System.Linq;
using Clinicks.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Clinicks.Domain.UnitTests;

public class InternacionTests
{
    [Fact]
    public void Crear_DebeInicializarInternacionYAgregarMovimientoCama()
    {
        // Arrange
        int dni = 12345678;
        var cama = new Cama { IdHabitacion = 1, NCama = 101 };

        // Act
        var internacion = Internacion.CrearInternacion(dni, cama);

        // Assert
        internacion.Dni.Should().Be(dni);
        internacion.FechaIngreso.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(2));
        internacion.FechaEgreso.Should().BeNull();
        
        internacion.MovimientosCama.Should().HaveCount(1);
        var movimiento = internacion.MovimientosCama.First();
        movimiento.IdHabitacion.Should().Be(1);
        movimiento.NCama.Should().Be(101);
        movimiento.FechaInicio.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(2));
        movimiento.FechaFin.Should().BeNull();
    }

    [Fact]
    public void RegistrarTraslado_DebeFinalizarMovimientoActualYCrearNuevo()
    {
        // Arrange
        var camaInicial = new Cama { IdHabitacion = 1, NCama = 101 };
        var internacion = Internacion.CrearInternacion(12345678, camaInicial);
        
        var camaNueva = new Cama { IdHabitacion = 2, NCama = 202 };

        // Act
        internacion.RegistrarTraslado(camaNueva);

        // Assert
        internacion.MovimientosCama.Should().HaveCount(2);
        
        var movimientoInicial = internacion.MovimientosCama.First(m => m.NCama == 101);
        movimientoInicial.FechaFin.Should().NotBeNull();
        movimientoInicial.FechaFin.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(2));

        var movimientoNuevo = internacion.MovimientosCama.First(m => m.NCama == 202);
        movimientoNuevo.FechaInicio.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(2));
        movimientoNuevo.FechaFin.Should().BeNull();
    }

    [Fact]
    public void RegistrarAlta_DebeEstablecerFechaEgresoYFinalizarMovimiento()
    {
        // Arrange
        var cama = new Cama { IdHabitacion = 1, NCama = 101 };
        var internacion = Internacion.CrearInternacion(12345678, cama);

        // Act
        internacion.RegistrarAlta();

        // Assert
        internacion.FechaEgreso.Should().NotBeNull();
        internacion.FechaEgreso.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(2));

        var movimiento = internacion.MovimientosCama.First();
        movimiento.FechaFin.Should().NotBeNull();
        movimiento.FechaFin.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(2));
    }
}
