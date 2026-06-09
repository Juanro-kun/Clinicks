using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinicks.Application.DTOs.Pacientes;
using Clinicks.Application.Exceptions;
using Clinicks.Application.Interfaces;
using Clinicks.Application.Services;
using Clinicks.Domain.Entities;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Clinicks.Application.UnitTests;

public class PacienteServiceTests
{
    private readonly IPacienteRepository _repositoryMock;
    private readonly IUnidadDeTrabajo _unidadDeTrabajoMock;
    private readonly ICurrentUserProvider _currentUserProviderMock;
    private readonly PacienteService _sut; // System Under Test

    public PacienteServiceTests()
    {
        _repositoryMock = Substitute.For<IPacienteRepository>();
        _unidadDeTrabajoMock = Substitute.For<IUnidadDeTrabajo>();
        _currentUserProviderMock = Substitute.For<ICurrentUserProvider>();

        _sut = new PacienteService(_repositoryMock, _unidadDeTrabajoMock, _currentUserProviderMock);
    }

    [Fact]
    public async Task RegistrarNuevoPaciente_ConDniExistente_DebeLanzarConflictException()
    {
        // Arrange
        var dto = new PacienteCreateDTO { Dni = 12345678, Nombre = "Juan", Apellido = "Perez" };
        _repositoryMock.ConsultarPaciente(dto.Dni).Returns(true);

        // Act
        Func<Task> act = async () => await _sut.RegistrarNuevoPaciente(dto);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Ya existe un paciente con este DNI.");
        
        _repositoryMock.DidNotReceive().Agregar(Arg.Any<Paciente>());
        await _unidadDeTrabajoMock.DidNotReceive().GuardarCambiosAsync();
    }

    [Fact]
    public async Task RegistrarNuevoPaciente_DatosValidos_DebePersistirPaciente()
    {
        // Arrange
        var dto = new PacienteCreateDTO 
        { 
            Dni = 12345678, 
            Nombre = "Juan", 
            Apellido = "Perez",
            Calle = "Av Siempre Viva",
            Altura = 742,
            IdCiudad = 1
        };
        _repositoryMock.ConsultarPaciente(dto.Dni).Returns(false);
        _currentUserProviderMock.GetCurrentUserId().Returns(10);

        // Act
        await _sut.RegistrarNuevoPaciente(dto);

        // Assert
        _repositoryMock.Received(1).Agregar(Arg.Is<Paciente>(p => 
            p.Dni == dto.Dni && 
            p.Nombre == dto.Nombre && 
            p.CreadoPorUsuarioId == 10 &&
            p.Direcciones.Count == 1));
            
        await _unidadDeTrabajoMock.Received(1).GuardarCambiosAsync();
    }

    [Fact]
    public async Task RegistrarNuevoPaciente_ConUsuarioAnonimo_DebeAsignarCreadorNulo()
    {
        // Arrange
        var dto = new PacienteCreateDTO { Dni = 12345678, Nombre = "Juan", Apellido = "Perez" };
        _repositoryMock.ConsultarPaciente(dto.Dni).Returns(false);
        _currentUserProviderMock.GetCurrentUserId().Returns((int?)null);

        // Act
        await _sut.RegistrarNuevoPaciente(dto);

        // Assert
        _repositoryMock.Received(1).Agregar(Arg.Is<Paciente>(p => p.CreadoPorUsuarioId == null));
    }

    [Fact]
    public async Task RegistrarNuevoPaciente_ConCalleSinAltura_DebeLanzarValidationException()
    {
        // Arrange
        var dto = new PacienteCreateDTO 
        { 
            Dni = 12345678, Nombre = "Juan", Apellido = "Perez", 
            Calle = "   Av Falsa   ", 
            Altura = null 
        };
        _repositoryMock.ConsultarPaciente(dto.Dni).Returns(false);

        // Act
        Func<Task> act = async () => await _sut.RegistrarNuevoPaciente(dto);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Debe proporcionar la altura de la calle especificada.");
    }

    [Fact]
    public async Task ActualizarDatosPaciente_PacienteInexistente_DebeRetornarFalse()
    {
        // Arrange
        var dto = new PacienteUpdateDTO { Dni = 12345678, Nombre = "Juan" };
        _repositoryMock.ObtenerPacientePorDni(dto.Dni).Returns((Paciente)null);

        // Act
        var result = await _sut.ActualizarDatosPaciente(dto);

        // Assert
        result.Should().BeFalse();
        await _unidadDeTrabajoMock.DidNotReceive().GuardarCambiosAsync();
    }

    [Fact]
    public async Task ActualizarDatosPaciente_DatosValidos_DebeActualizarYGuardarCambios()
    {
        // Arrange
        var dto = new PacienteUpdateDTO { Dni = 12345678, Nombre = "Juan Modificado", Activo = true };
        var pacienteExistente = new Paciente { Dni = 12345678, Nombre = "Juan Viejo" };
        _repositoryMock.ObtenerPacientePorDni(dto.Dni).Returns(pacienteExistente);

        // Act
        var result = await _sut.ActualizarDatosPaciente(dto);

        // Assert
        result.Should().BeTrue();
        pacienteExistente.Nombre.Should().Be("Juan Modificado");
        await _unidadDeTrabajoMock.Received(1).GuardarCambiosAsync();
    }

    [Fact]
    public async Task ActualizarDatosPaciente_ConDireccionNueva_DebeAgregarDireccionALaColeccion()
    {
        // Arrange
        var dto = new PacienteUpdateDTO 
        { 
            Dni = 12345678, 
            Nombre = "Juan", 
            Calle = "Av Siempre Viva", 
            Altura = 742,
            IdCiudad = 1
        };
        var pacienteExistente = new Paciente { Dni = 12345678, Nombre = "Juan" };
        _repositoryMock.ObtenerPacientePorDni(dto.Dni).Returns(pacienteExistente);

        // Act
        var result = await _sut.ActualizarDatosPaciente(dto);

        // Assert
        result.Should().BeTrue();
        pacienteExistente.Direcciones.Should().HaveCount(1);
        var dir = pacienteExistente.Direcciones.First();
        dir.Calle.Should().Be("Av Siempre Viva");
        dir.Altura.Should().Be(742);
        dir.IdCiudad.Should().Be(1);
        await _unidadDeTrabajoMock.Received(1).GuardarCambiosAsync();
    }

    [Fact]
    public async Task ActualizarDatosPaciente_ConDireccionExistente_DebeModificarDireccionExistente()
    {
        // Arrange
        var dto = new PacienteUpdateDTO 
        { 
            Dni = 12345678, 
            Nombre = "Juan", 
            Calle = "Av Siempre Viva Nueva", 
            Altura = 743,
            IdCiudad = 2
        };
        var pacienteExistente = new Paciente { Dni = 12345678, Nombre = "Juan" };
        pacienteExistente.Direcciones.Add(new Direccion 
        { 
            Calle = "Av Siempre Viva Antigua", 
            Altura = 742, 
            IdCiudad = 1 
        });
        _repositoryMock.ObtenerPacientePorDni(dto.Dni).Returns(pacienteExistente);

        // Act
        var result = await _sut.ActualizarDatosPaciente(dto);

        // Assert
        result.Should().BeTrue();
        pacienteExistente.Direcciones.Should().HaveCount(1);
        var dir = pacienteExistente.Direcciones.First();
        dir.Calle.Should().Be("Av Siempre Viva Nueva");
        dir.Altura.Should().Be(743);
        dir.IdCiudad.Should().Be(2);
        await _unidadDeTrabajoMock.Received(1).GuardarCambiosAsync();
    }

    [Fact]
    public async Task ActualizarDatosPaciente_ConCalleVacia_NoDebeModificarNiAgregarDirecciones()
    {
        // Arrange
        var dto = new PacienteUpdateDTO 
        { 
            Dni = 12345678, 
            Nombre = "Juan", 
            Calle = "", 
            Altura = null 
        };
        var pacienteExistente = new Paciente { Dni = 12345678, Nombre = "Juan" };
        pacienteExistente.Direcciones.Add(new Direccion 
        { 
            Calle = "Calle Existente", 
            Altura = 123, 
            IdCiudad = 1 
        });
        _repositoryMock.ObtenerPacientePorDni(dto.Dni).Returns(pacienteExistente);

        // Act
        var result = await _sut.ActualizarDatosPaciente(dto);

        // Assert
        result.Should().BeTrue();
        pacienteExistente.Direcciones.Should().HaveCount(1);
        var dir = pacienteExistente.Direcciones.First();
        dir.Calle.Should().Be("Calle Existente");
        await _unidadDeTrabajoMock.Received(1).GuardarCambiosAsync();
    }

    [Fact]
    public async Task ActualizarDatosPaciente_ConCallePeroSinAltura_DebeLanzarValidationException()
    {
        // Arrange
        var dto = new PacienteUpdateDTO { Dni = 12345678, Nombre = "Juan", Calle = "Av Falsa", Altura = null };
        var pacienteExistente = new Paciente { Dni = 12345678, Nombre = "Juan Viejo" };
        _repositoryMock.ObtenerPacientePorDni(dto.Dni).Returns(pacienteExistente);

        // Act
        Func<Task> act = async () => await _sut.ActualizarDatosPaciente(dto);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Debe proporcionar la altura de la calle especificada.");
    }

    [Fact]
    public async Task EliminarPaciente_PacienteInexistente_NoDebeHacerNada()
    {
        // Arrange
        _repositoryMock.ObtenerPacientePorDni(12345678).Returns((Paciente)null);

        // Act
        await _sut.EliminarPaciente(12345678);

        // Assert
        await _unidadDeTrabajoMock.DidNotReceive().GuardarCambiosAsync();
    }

    [Fact]
    public async Task EliminarPaciente_PacienteExistenteYLibre_DebeEliminarYGuardar()
    {
        // Arrange
        var paciente = new Paciente { Dni = 12345678, Activo = true };
        _repositoryMock.ObtenerPacientePorDni(12345678).Returns(paciente);

        // Act
        await _sut.EliminarPaciente(12345678);

        // Assert
        paciente.Activo.Should().BeFalse();
        await _unidadDeTrabajoMock.Received(1).GuardarCambiosAsync();
    }
}
