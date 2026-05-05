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
    private readonly IInternacionRepository _repository;

    public InternacionService(IInternacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> ProcesarInternacionDePaciente(InternacionRequestDto request)
    {
        // 1. Verificar si el paciente ya tiene una internación activa
        var internacionActiva = await VerificaInternacionActiva(request.Dni);

        if (internacionActiva)
        {
            throw new ConflictException("El paciente ya se encuentra internado actualmente.");
        }

        // 2. Verificar si la cama seleccionada está libre
        var cama = await BuscarCama(request.IdHabitacion, request.NCama);

        if (!cama)
        {
            throw new NotFoundException("La cama seleccionada no existe.");
        }

        var camaOcupadaPorOtro = await VerificarCamaOcupada(request.IdHabitacion, request.NCama);

        if (camaOcupadaPorOtro)
        {
            throw new ConflictException("La cama seleccionada ya está ocupada.");
        }

        // 3. Crear la nueva Internación
        var nuevaInternacion = new Internacion
        {
            Dni = request.Dni,
            IdHabitacion = request.IdHabitacion,
            NCama = request.NCama,
            FechaInicio = DateTime.Now,
            FechaFin = null
        };

        await _repository.ProcesarNuevaInternacion(nuevaInternacion);

        return true;
    }

    public async Task<IEnumerable<InternacionResponseDto>> ListarInternacionesActivas()
    {
        return await _repository.ListarInternacionesActivas();
    }

    public async Task<bool> ProcesarAltaMedica(int idInternacion)
    {
        return await _repository.ProcesarAltaMedica(idInternacion);
    }

    private async Task<bool> VerificaInternacionActiva(int dni)
    {
        return await _repository.VerificaInternacionActiva(dni);
    }

    private async Task<bool> BuscarCama(int idHabitacion, int nCama)
    {
        return await _repository.ExisteCama(idHabitacion, nCama);
    }

    private async Task<bool> VerificarCamaOcupada(int idHabitacion, int nCama)
    {
        return await _repository.VerificarCamaOcupada(idHabitacion, nCama);
    }
}
