using System;
using System.Threading.Tasks;
using Clinicks.Application.DTOs;
using Clinicks.Application.Exceptions;
using Clinicks.Application.Interfaces;
using Clinicks.Application.Services;
using Clinicks.Domain.Entities;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Clinicks.Application.UnitTests;

public class InternacionServiceTests
{
    private readonly IInternacionRepository _internacionRepoMock;
    private readonly IHabitacionRepository _habitacionRepoMock;
    private readonly IUnidadDeTrabajo _unidadDeTrabajoMock;
    private readonly ICurrentUserProvider _currentUserProviderMock;
    private readonly IPacienteRepository _pacienteRepoMock;
    private readonly InternacionService _sut;

    public InternacionServiceTests()
    {
        _internacionRepoMock = Substitute.For<IInternacionRepository>();
        _habitacionRepoMock = Substitute.For<IHabitacionRepository>();
        _unidadDeTrabajoMock = Substitute.For<IUnidadDeTrabajo>();
        _currentUserProviderMock = Substitute.For<ICurrentUserProvider>();
        _pacienteRepoMock = Substitute.For<IPacienteRepository>();

        _sut = new InternacionService(
            _internacionRepoMock, 
            _habitacionRepoMock, 
            _unidadDeTrabajoMock, 
            _currentUserProviderMock,
            _pacienteRepoMock);
    }

    [Fact]
    public async Task ProcesarInternacionDePaciente_PacienteYaInternado_DebeLanzarConflictException()
    {
        // Arrange
        var request = new InternacionRequestDto { Dni = 12345678, IdHabitacion = 1, NCama = 101 };
        var paciente = new Paciente { Dni = request.Dni };
        var cama = new Cama();
        var internacion = Internacion.CrearInternacion(request.Dni, cama);
        paciente.Internaciones.Add(internacion);
        _pacienteRepoMock.ObtenerPacientePorDni(request.Dni).Returns(paciente);

        // Act
        Func<Task> act = async () => await _sut.ProcesarInternacionDePaciente(request);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("El paciente ya se encuentra internado actualmente.");
    }

    [Fact]
    public async Task ProcesarInternacionDePaciente_CamaInexistente_DebeLanzarNotFoundException()
    {
        // Arrange
        var request = new InternacionRequestDto { Dni = 12345678, IdHabitacion = 1, NCama = 101 };
        var paciente = new Paciente { Dni = request.Dni };
        _pacienteRepoMock.ObtenerPacientePorDni(request.Dni).Returns(paciente);
        _habitacionRepoMock.ObtenerCama(request.IdHabitacion, request.NCama).Returns((Cama)null);

        // Act
        Func<Task> act = async () => await _sut.ProcesarInternacionDePaciente(request);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("La cama seleccionada no existe.");
    }

    [Fact]
    public async Task ProcesarInternacionDePaciente_CamaOcupada_DebeLanzarConflictException()
    {
        // Arrange
        var request = new InternacionRequestDto { Dni = 12345678, IdHabitacion = 1, NCama = 101 };
        var paciente = new Paciente { Dni = request.Dni };
        _pacienteRepoMock.ObtenerPacientePorDni(request.Dni).Returns(paciente);
        
        var cama = new Cama();
        cama.Ocupar(); // La cama ya no esta libre

        _habitacionRepoMock.ObtenerCama(request.IdHabitacion, request.NCama).Returns(cama);

        // Act
        Func<Task> act = async () => await _sut.ProcesarInternacionDePaciente(request);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("La cama seleccionada no está libre.");
    }

    [Fact]
    public async Task ProcesarInternacionDePaciente_DatosValidos_DebeCrearInternacion()
    {
        // Arrange
        var request = new InternacionRequestDto { Dni = 12345678, IdHabitacion = 1, NCama = 101 };
        var paciente = new Paciente { Dni = request.Dni };
        _pacienteRepoMock.ObtenerPacientePorDni(request.Dni).Returns(paciente);
        
        var cama = new Cama(); // Libre por defecto
        _habitacionRepoMock.ObtenerCama(request.IdHabitacion, request.NCama).Returns(cama);
        _currentUserProviderMock.GetCurrentUserId().Returns(1);

        // Act
        var result = await _sut.ProcesarInternacionDePaciente(request);

        // Assert
        result.Should().BeTrue();
        _internacionRepoMock.Received(1).Agregar(Arg.Is<Internacion>(i => i.Dni == request.Dni && i.CreadoPorUsuarioId == 1));
        await _unidadDeTrabajoMock.Received(1).GuardarCambiosAsync();
    }

    [Fact]
    public async Task ProcesarAltaMedica_InternacionInexistente_DebeRetornarFalse()
    {
        // Arrange
        _pacienteRepoMock.ObtenerPacientePorDni(12345678).Returns((Paciente)null);

        // Act
        var result = await _sut.ProcesarAltaMedica(12345678);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ProcesarAltaMedica_DatosValidos_DebeRegistrarAltaYGuardar()
    {
        // Arrange
        var paciente = new Paciente { Dni = 12345678 };
        var cama = new Cama();
        var internacion = Internacion.CrearInternacion(12345678, cama);
        paciente.Internaciones.Add(internacion);
        _pacienteRepoMock.ObtenerPacientePorDni(12345678).Returns(paciente);

        // Act
        var result = await _sut.ProcesarAltaMedica(12345678);

        // Assert
        result.Should().BeTrue();
        internacion.FechaEgreso.Should().NotBeNull();
        await _unidadDeTrabajoMock.Received(1).GuardarCambiosAsync();
    }

    [Fact]
    public async Task TrasladarPaciente_MismaCamaOcupada_DebeLanzarConflictException()
    {
        // Arrange
        var request = new TrasladoRequestDto { Dni = 12345678, IdHabitacion = 1, NCama = 101 };
        var camaDestino = new Cama { IdHabitacion = 1, NCama = 101 };
        
        var paciente = new Paciente { Dni = request.Dni };
        var internacion = Internacion.CrearInternacion(request.Dni, camaDestino); // Al crear la internación, la cama se ocupa automáticamente
        paciente.Internaciones.Add(internacion);

        _pacienteRepoMock.ObtenerPacientePorDni(request.Dni).Returns(paciente);
        _habitacionRepoMock.ObtenerCama(request.IdHabitacion, request.NCama).Returns(camaDestino);

        // Act
        Func<Task> act = async () => await _sut.TrasladarPaciente(request);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("El paciente ya se encuentra en la cama de destino.");
    }

    [Fact]
    public async Task TrasladarPaciente_PacienteNoInternado_DebeRetornarFalse()
    {
        // Arrange
        var request = new TrasladoRequestDto { Dni = 12345678, IdHabitacion = 2, NCama = 202 };
        var paciente = new Paciente { Dni = request.Dni }; 
        // No agregamos ninguna internación activa al paciente
        
        _pacienteRepoMock.ObtenerPacientePorDni(request.Dni).Returns(paciente);

        // Act
        var result = await _sut.TrasladarPaciente(request);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task TrasladarPaciente_DatosValidos_DebeTrasladarYGuardar()
    {
        // Arrange
        var request = new TrasladoRequestDto { Dni = 12345678, IdHabitacion = 2, NCama = 202 };
        
        var camaOrigen = new Cama { IdHabitacion = 1, NCama = 101 };
        var paciente = new Paciente { Dni = request.Dni };
        
        // Creamos la internación activa en la cama origen
        var internacion = Internacion.CrearInternacion(request.Dni, camaOrigen);
        
        // Dado que en el Unit Test no estamos usando Entity Framework, 
        // debemos enlazar manualmente la propiedad de navegación para que `FinalizarMovimiento()` funcione.
        var movimiento = internacion.MovimientosCama.First();
        movimiento.CamaNavigation = camaOrigen; 
        
        paciente.Internaciones.Add(internacion);

        var camaDestino = new Cama { IdHabitacion = 2, NCama = 202 }; // Libre por defecto

        _pacienteRepoMock.ObtenerPacientePorDni(request.Dni).Returns(paciente);
        _habitacionRepoMock.ObtenerCama(request.IdHabitacion, request.NCama).Returns(camaDestino);

        // Act
        var result = await _sut.TrasladarPaciente(request);

        // Assert
        result.Should().BeTrue();
        camaOrigen.EstaLibre().Should().BeTrue(); // Verifica que la cama de origen se liberó
        camaDestino.EstaLibre().Should().BeFalse(); // Verifica que la cama destino se ocupó
        await _unidadDeTrabajoMock.Received(1).GuardarCambiosAsync(); // Verifica que se persistió
    }
}
