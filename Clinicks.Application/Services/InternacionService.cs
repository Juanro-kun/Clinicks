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

    public InternacionService(
        IInternacionRepository internacionRepository, 
        IHabitacionRepository habitacionRepository,
        IUnidadDeTrabajo unidadDeTrabajo)
    {
        _internacionRepository = internacionRepository;
        _habitacionRepository = habitacionRepository;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public async Task<bool> ProcesarInternacionDePaciente(InternacionRequestDto request)
    {
        var internacionActiva = await _internacionRepository.ObtenerInternacionActiva(request.Dni);
        if (internacionActiva != null)
        {
            throw new ConflictException("El paciente ya se encuentra internado actualmente.");
        }

        var cama = await _habitacionRepository.ObtenerCama(request.IdHabitacion, request.NCama);
        if (cama == null)
        {
            throw new NotFoundException("La cama seleccionada no existe.");
        }

        var existeMovimientoActivoEnCama = await _internacionRepository.ExisteMovimientoActivoEnCama(request.IdHabitacion, request.NCama);
        if (existeMovimientoActivoEnCama || !cama.EstaLibre())
        {
            throw new ConflictException("La cama seleccionada no está libre.");
        }

        var nuevaInternacion = Internacion.Crear(request.Dni, cama);

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
        var internacion = await _internacionRepository.ObtenerInternacionActiva(dni);
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
        var internacion = await _internacionRepository.ObtenerInternacionActiva(request.Dni);
        if (internacion == null)
        {
            return false;
        }

        var nuevaCama = await _habitacionRepository.ObtenerCama(request.IdHabitacion, request.NCama);
        if (nuevaCama == null)
        {
            throw new NotFoundException("La cama de destino no existe.");
        }

        var existeMovimientoActivoEnCama = await _internacionRepository.ExisteMovimientoActivoEnCama(request.IdHabitacion, request.NCama);
        if (existeMovimientoActivoEnCama || !nuevaCama.EstaLibre())
        {
            throw new ConflictException("La cama de destino no está libre.");
        }

        internacion.RegistrarTraslado(nuevaCama);

        await _unidadDeTrabajo.GuardarCambiosAsync();
        return true;
    }
}

