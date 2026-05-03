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
        Task<bool> ConsultarPaciente(int dni);
        Task RegistrarNuevoPaciente(int dni, string nombre, string apellido, string telefono, string calle, int altura, string ciudadNombre, string provinciaNombre, string paisNombre);
        Task<bool> ActualizarDatosPaciente(Paciente paciente);
        Task EliminarPaciente(int dni);
    }
}
