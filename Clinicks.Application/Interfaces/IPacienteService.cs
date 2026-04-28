using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clinicks.Domain.Entities;

namespace Clinicks.Application.Interfaces
{
    public interface IPacienteService
    {
        Task<IEnumerable<Paciente>> ListarPacientes();
        Task<Paciente?> BuscarPacientePorDni(int dni);
        Task<bool> ProcesarAltaDePaciente(Paciente paciente);
        Task<bool> ActualizarDatosPaciente(Paciente paciente);
        Task EliminarPaciente(int dni);
    }
}
