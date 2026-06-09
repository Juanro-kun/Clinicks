using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinicks.Application.DTOs;
using Clinicks.Application.Interfaces;
using Clinicks.Application.Exceptions;
using Clinicks.Domain.Entities;

namespace Clinicks.Application.Services;

public class InternacionService : IInternacionService
{
    private readonly IInternacionRepository _internacionRepository;
    private readonly IHabitacionRepository _habitacionRepository;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IPacienteRepository _pacienteRepository;

    public InternacionService(
        IInternacionRepository internacionRepository,
        IHabitacionRepository habitacionRepository,
        IUnidadDeTrabajo unidadDeTrabajo,
        ICurrentUserProvider currentUserProvider,
        IPacienteRepository pacienteRepository)
    {
        _internacionRepository = internacionRepository;
        _habitacionRepository = habitacionRepository;
        _unidadDeTrabajo = unidadDeTrabajo;
        _currentUserProvider = currentUserProvider;
        _pacienteRepository = pacienteRepository;
    }

    public async Task<bool> ProcesarInternacionDePaciente(InternacionRequestDto request)
    {
        var paciente = await _pacienteRepository.ObtenerPacientePorDni(request.Dni);
        if (paciente == null)
        {
            throw new NotFoundException("El paciente no existe.");
        }

        if (paciente.TieneInternacionActiva())
        {
            throw new ConflictException("El paciente ya se encuentra internado actualmente.");
        }

        var cama = await _habitacionRepository.ObtenerCama(request.IdHabitacion, request.NCama);
        if (cama == null)
        {
            throw new NotFoundException("La cama seleccionada no existe.");
        }

        if (!cama.EstaLibre())
        {
            throw new ConflictException("La cama seleccionada no está libre.");
        }

        int? creadorId = _currentUserProvider.GetCurrentUserId();

        var nuevaInternacion = Internacion.CrearInternacion(request.Dni, cama);
        nuevaInternacion.CreadoPorUsuarioId = creadorId;

        _internacionRepository.Agregar(nuevaInternacion);

        await _unidadDeTrabajo.GuardarCambiosAsync();

        return true;
    }

    public async Task<IEnumerable<InternacionResponseDto>> ListarInternacionesActivas()
    {
        return await _internacionRepository.ListarInternacionesActivas();
    }

    public async Task<bool> ProcesarAltaMedica(int dni)
    {
        var paciente = await _pacienteRepository.ObtenerPacientePorDni(dni);
        if (paciente == null || !paciente.TieneInternacionActiva())
        {
            return false;
        }

        var internacion = paciente.Internaciones.FirstOrDefault(i => i.FechaEgreso == null);
        if (internacion == null)
        {
            return false;
        }

        internacion.RegistrarAlta();

        await _unidadDeTrabajo.GuardarCambiosAsync();
        return true;
    }

    public async Task<bool> TrasladarPaciente(TrasladoRequestDto request)
    {
        var paciente = await _pacienteRepository.ObtenerPacientePorDni(request.Dni);
        if (paciente == null || !paciente.TieneInternacionActiva())
        {
            return false;
        }

        var internacion = paciente.Internaciones.FirstOrDefault(i => i.FechaEgreso == null);
        if (internacion == null)
        {
            return false;
        }

        // Verifica que no estemos intentando trasladar al paciente a la misma cama donde ya está.
        var movimientoActual = internacion.MovimientosCama.FirstOrDefault(m => m.FechaFin == null);
        if (movimientoActual != null && movimientoActual.IdHabitacion == request.IdHabitacion && movimientoActual.NCama == request.NCama)
        {
            throw new ConflictException("El paciente ya se encuentra en la cama de destino.");
        }

        var nuevaCama = await _habitacionRepository.ObtenerCama(request.IdHabitacion, request.NCama);
        if (nuevaCama == null)
        {
            throw new NotFoundException("La cama de destino no existe.");
        }

        // Asegura que la nueva cama esté completamente libre antes de efectuar el traspaso.
        if (!nuevaCama.EstaLibre())
        {
            throw new ConflictException("La cama de destino no está libre.");
        }

        internacion.RegistrarTraslado(nuevaCama);

        await _unidadDeTrabajo.GuardarCambiosAsync();
        return true;
    }
}

