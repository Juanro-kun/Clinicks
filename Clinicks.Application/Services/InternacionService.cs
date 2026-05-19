using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinicks.Application.DTOs;
using Clinicks.Application.Interfaces;
using Clinicks.Application.Exceptions;
using Clinicks.Domain.Entities;
using Clinicks.Domain.Enums;

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

        var movimientoActivoEnCama = await _internacionRepository.ObtenerMovimientoActivoEnCama(request.IdHabitacion, request.NCama);
        if (movimientoActivoEnCama != null || cama.IdEstado != (int)EstadoCamaEnum.Libre)
        {
            throw new ConflictException("La cama seleccionada no está libre.");
        }

        var nuevaInternacion = new Internacion
        {
            Dni = request.Dni,
            FechaIngreso = DateTime.Now,
            FechaEgreso = null
        };

        nuevaInternacion.MovimientosCama.Add(new MovimientoCama
        {
            IdHabitacion = request.IdHabitacion,
            NCama = request.NCama,
            FechaInicio = nuevaInternacion.FechaIngreso ?? DateTime.Now,
            FechaFin = null
        });

        _internacionRepository.Agregar(nuevaInternacion);

        cama.IdEstado = (int)EstadoCamaEnum.Ocupada;

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

        internacion.FechaEgreso = DateTime.Now;

        var movimiento = await _internacionRepository.ObtenerMovimientoActivo(internacion.IdInternacion);
        if (movimiento != null)
        {
            movimiento.FechaFin = DateTime.Now;

            var cama = await _habitacionRepository.ObtenerCama(movimiento.IdHabitacion, movimiento.NCama);
            if (cama != null)
            {
                cama.IdEstado = (int)EstadoCamaEnum.Libre;
            }
        }

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

        var ocupante = await _internacionRepository.ObtenerMovimientoActivoEnCama(request.IdHabitacion, request.NCama);
        if (ocupante != null || nuevaCama.IdEstado != (int)EstadoCamaEnum.Libre)
        {
            throw new ConflictException("La cama de destino no está libre.");
        }

        var movimientoActual = await _internacionRepository.ObtenerMovimientoActivo(internacion.IdInternacion);
        if (movimientoActual != null)
        {
            movimientoActual.FechaFin = DateTime.Now;

            var camaAnterior = await _habitacionRepository.ObtenerCama(movimientoActual.IdHabitacion, movimientoActual.NCama);
            if (camaAnterior != null)
            {
                camaAnterior.IdEstado = (int)EstadoCamaEnum.Libre;
            }
        }

        internacion.MovimientosCama.Add(new MovimientoCama
        {
            IdHabitacion = request.IdHabitacion,
            NCama = request.NCama,
            FechaInicio = DateTime.Now,
            FechaFin = null
        });

        nuevaCama.IdEstado = (int)EstadoCamaEnum.Ocupada;

        await _unidadDeTrabajo.GuardarCambiosAsync();
        return true;
    }
}
