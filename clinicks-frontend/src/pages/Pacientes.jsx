import { useState, useEffect, useMemo } from "react"
import { Plus, Search, LogOut, Users, ClipboardList, X, Trash2, Pencil } from "lucide-react"
import api from "../api/api"
import { useNavigate } from "react-router-dom"

export default function Pacientes() {
  const [patients, setPatients] = useState([])
  const [searchTerm, setSearchTerm] = useState("")
  const [isModalOpen, setIsModalOpen] = useState(false)
  
  // Estado para saber si estamos editando o creando uno nuevo
  const [isEditing, setIsEditing] = useState(false)
  
  const [patientForm, setPatientForm] = useState({ dni: '', nombre: '', apellido: '', direccion: '', telefono: '' })
  
  const navigate = useNavigate()

  const fetchPacientes = async () => {
    try {
      const res = await api.get('/Pacientes')
      setPatients(res.data)
    } catch (err) {
      if (err.response?.status === 401) navigate('/login')
    }
  }

  useEffect(() => { fetchPacientes() }, [])

  // 1. FUNCIÓN PARA PREPARAR LA EDICIÓN
  const handleEditRequest = (p) => {
    setPatientForm(p) // Cargamos los datos del paciente en el form
    setIsEditing(true) // Activamos modo edición
    setIsModalOpen(true) // Abrimos el modal
  }

  // 2. FUNCIÓN PARA CERRAR Y LIMPIAR
  const closeModal = () => {
    setIsModalOpen(false)
    setIsEditing(false)
    setPatientForm({ dni: '', nombre: '', apellido: '', direccion: '', telefono: '' })
  }

  // 3. GUARDAR (POST o PUT)
  const handleSave = async (e) => {
        e.preventDefault();

        // 1. Validaciones manuales rápidas
        if (patientForm.dni.toString().length < 7 || patientForm.dni.toString().length > 8) {
            alert("DNI invalido");
            return;
        }

        if (patientForm.nombre.trim().length < 2 || patientForm.apellido.trim().length < 1) {
            alert("Nombre o Apellido muy corto");
            return;
        }

        // 2. Si pasó los filtros, recién ahí vamos a la API
        try {
            if (isEditing) {
                await api.put(`/Pacientes/${patientForm.dni}`, patientForm);
            } else {
                await api.post('/Pacientes', patientForm);
            }
            closeModal();
            fetchPacientes();
        } catch (err) {
            console.log("Error completo:", err.response); // Esto miralo en la consola (F12)
            
            // Si la API mandó un objeto, lo convertimos a texto para el alert
            const mensajeError = err.response?.data 
                ? JSON.stringify(err.response.data) 
                : err.message;
                
            alert("Error detallado: " + mensajeError);
        }
    };

  const handleDelete = async (dni) => {
    if (window.confirm("¿Esta seguro de eliminar a este paciente? Sus datos no se podrán recuperar")) {
      try {
        await api.delete(`/Pacientes/${dni}`)
        setPatients(patients.filter(p => p.dni !== dni))
      } catch (err) {
        alert("No se pudo eliminar.")
      }
    }
  }

  const filteredPatients = useMemo(() => {
    const term = searchTerm.toLowerCase()
    return patients.filter(p => 
      p.dni.toString().includes(term) || 
      p.nombre.toLowerCase().includes(term) || 
      p.apellido.toLowerCase().includes(term)
    )
  }, [patients, searchTerm])

  return (
    <>
      <header className="flex justify-between items-center mb-10">
        <div>
          <h1 className="text-3xl font-bold text-slate-900">Gestión de Pacientes</h1>
          <p className="text-slate-500 mt-1">Administrá los registros de la clínica</p>
        </div>
        <button 
          onClick={() => setIsModalOpen(true)}
          className="bg-emerald-500 hover:bg-emerald-600 text-white px-5 py-2.5 rounded-xl font-semibold flex items-center gap-2 shadow-lg shadow-emerald-200"
        >
          <Plus className="w-5 h-5" /> Nuevo Paciente
        </button>
      </header>

      {/* Buscador */}
      <div className="mb-8 relative max-w-md">
        <Search className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
        <input
          type="text"
          placeholder="Buscar..."
          className="w-full pl-12 pr-4 py-3 bg-white border border-slate-200 rounded-2xl focus:outline-none focus:ring-4 focus:ring-emerald-500/10 shadow-sm"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
        />
      </div>

      {/* Tabla */}
      <div className="bg-white rounded-3xl border border-slate-200 shadow-sm overflow-hidden">
        <table className="w-full text-left">
          <thead>
            <tr className="bg-slate-50/50 border-b border-slate-200">
              <th className="px-8 py-5 text-xs font-bold text-slate-400 uppercase">DNI</th>
              <th className="px-8 py-5 text-xs font-bold text-slate-400 uppercase">Nombre</th>
              <th className="px-8 py-5 text-xs font-bold text-slate-400 uppercase text-right">Acciones</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100">
            {filteredPatients.map((p) => (
              <tr key={p.dni} className="hover:bg-slate-50/50 transition-colors">
                <td className="px-8 py-5 text-sm font-bold text-slate-700">{p.dni}</td>
                <td className="px-8 py-5 text-sm text-slate-600 font-medium">{p.nombre} {p.apellido}</td>
                <td className="px-8 py-5 text-right">
                  <div className="flex justify-end gap-2">
                    {/* BOTÓN EDITAR */}
                    <button 
                      onClick={() => handleEditRequest(p)}
                      className="p-2 text-slate-400 hover:text-emerald-500 hover:bg-emerald-50 rounded-lg"
                    >
                      <Pencil className="w-4 h-4" />
                    </button>
                    <button 
                      onClick={() => handleDelete(p.dni)}
                      className="p-2 text-slate-400 hover:text-red-500 hover:bg-red-50 rounded-lg"
                    >
                      <Trash2 className="w-4 h-4" />
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* MODAL REUTILIZABLE */}
      {isModalOpen && (
        <div className="fixed inset-0 bg-slate-900/50 backdrop-blur-sm flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-3xl w-full max-w-md p-8 shadow-2xl">
            <div className="flex justify-between items-center mb-6">
              <h2 className="text-2xl font-bold text-slate-800">
                {isEditing ? 'Editar Paciente' : 'Nuevo Paciente'}
              </h2>
              <button onClick={closeModal}><X className="text-slate-400" /></button>
            </div>
            
            <form onSubmit={handleSave} className="space-y-4">
              {/* Si estamos editando, el DNI suele ser fijo (PK) */}
              <input 
                type="number" placeholder="DNI" 
                className={`w-full p-3 border border-slate-200 rounded-xl ${isEditing ? 'bg-slate-50 text-slate-400' : ''}`}
                value={patientForm.dni}
                disabled={isEditing}
                required
                onChange={e => setPatientForm({...patientForm, dni: e.target.value})} 
              />
              
              <div className="flex gap-4">
                <input type="text" placeholder="Nombre" className="w-1/2 p-3 border border-slate-200 rounded-xl" required
                  value={patientForm.nombre}
                  onChange={e => setPatientForm({...patientForm, nombre: e.target.value})} />
                <input type="text" placeholder="Apellido" className="w-1/2 p-3 border border-slate-200 rounded-xl" required
                  value={patientForm.apellido}
                  onChange={e => setPatientForm({...patientForm, apellido: e.target.value})} />
              </div>

              <input type="text" placeholder="Dirección" className="w-full p-3 border border-slate-200 rounded-xl"
                value={patientForm.direccion || ''}
                onChange={e => setPatientForm({...patientForm, direccion: e.target.value})} />
              
              <input type="text" placeholder="Teléfono" className="w-full p-3 border border-slate-200 rounded-xl"
                value={patientForm.telefono || ''}
                onChange={e => setPatientForm({...patientForm, telefono: e.target.value})} />

              <button type="submit" className="w-full bg-emerald-500 text-white py-3 rounded-xl font-bold mt-4 shadow-lg shadow-emerald-200">
                {isEditing ? 'Actualizar Datos' : 'Guardar Paciente'}
              </button>
            </form>
          </div>
        </div>
      )}
    </>
  )
}