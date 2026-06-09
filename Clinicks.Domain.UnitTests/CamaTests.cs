using Clinicks.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Clinicks.Domain.UnitTests;

public class CamaTests
{
    [Fact]
    public void NuevaCama_PorDefecto_DebeEstarLibre()
    {
        // Arrange & Act
        var cama = new Cama(); // Internamente setea _idEstado = 1

        // Assert
        cama.IdEstado.Should().Be(1);
        cama.EstaLibre().Should().BeTrue();
        cama.EstaOcupada.Should().BeFalse();
    }

    [Fact]
    public void Ocupar_CamaLibre_DebeCambiarAOcupada()
    {
        // Arrange
        var cama = new Cama();

        // Act
        cama.Ocupar();

        // Assert
        cama.EstaOcupada.Should().BeTrue();
        cama.EstaLibre().Should().BeFalse();
        cama.IdEstado.Should().Be(2); // Asumiendo que 2 es Ocupada
    }

    [Fact]
    public void PonerEnMantenimiento_CamaLibre_DebeCambiarAEnMantenimiento()
    {
        // Arrange
        var cama = new Cama();

        // Act
        cama.PonerEnMantenimiento();

        // Assert
        cama.EstaLibre().Should().BeFalse();
        cama.EstaOcupada.Should().BeFalse();
        cama.IdEstado.Should().Be(3); // Asumiendo que 3 es Mantenimiento
    }

    [Fact]
    public void Liberar_CamaOcupada_DebeCambiarALibre()
    {
        // Arrange
        var cama = new Cama();
        cama.Ocupar();

        // Act
        cama.Liberar();

        // Assert
        cama.EstaLibre().Should().BeTrue();
        cama.EstaOcupada.Should().BeFalse();
        cama.IdEstado.Should().Be(1);
    }
}
