import { useState, useEffect, useMemo } from "react"
import { Plus, Search, LogOut, Users, Bed, ArrowLeftRight, ClipboardList, X, Trash2, Pencil } from "lucide-react"
import api from "../api/api"
import { useNavigate } from "react-router-dom"

export default function Pacientes() {
  const [patients, setPatients] = useState([])
  const [searchTerm, setSearchTerm] = useState("")
  const [isModalOpen, setIsModalOpen] = useState(false) // Estado para el Modal
  const [newPatient, setNewPatient] = useState({ dni: '', nombre: '', apellido: '', direccion: '', telefono: '' })
  
  const navigate = useNavigate()

  // Cargar pacientes
  const fetchPacientes = async () => {
    try {
      const res = await api.get('/Pacientes')
      setPatients(res.data)
    } catch (err) {
      if (err.response?.status === 401) navigate('/login')
    }
  }

  useEffect(() => { fetchPacientes() }, [])

  // Guardar nuevo paciente
  const handleSave = async (e) => {
    e.preventDefault()
    try {
      await api.post('/Pacientes', newPatient)
      setIsModalOpen(false) // Cerramos el modal
      setNewPatient({ dni: '', nombre: '', apellido: '', direccion: '', telefono: '' }) // Limpiamos
      fetchPacientes() // Recargamos la lista
    } catch (err) {
      alert("Error al guardar. Revisá que el DNI no esté duplicado.")
    }
  }

  const filteredPatients = useMemo(() => {
    if (!searchTerm.trim()) return patients
    const term = searchTerm.toLowerCase()
    return patients.filter(p => 
      p.dni.toString().includes(term) || 
      p.nombre.toLowerCase().includes(term) || 
      p.apellido.toLowerCase().includes(term)
    )
  }, [patients, searchTerm])

  const handleDelete = async (dni) => {
        if (window.confirm("¿Seguro que querés borrar a este paciente?")) {
            try {
                await api.delete(`/Pacientes/${dni}`);
                setPatients(patients.filter(p => p.dni !== dni));
            } catch (err) {
                alert("No se pudo eliminar.");
            }
        }
    };

  return (
    <div className="flex min-h-screen bg-[#f8fafc]">
      
      
      <main className="flex-1 p-8">
        <div className="max-w-6xl mx-auto">
          <header className="flex justify-between items-center mb-10">
            <div>
              <h1 className="text-3xl font-bold text-slate-900">Gestión de Pacientes</h1>
            </div>
            {/* AQUÍ CONECTAMOS EL BOTÓN */}
            <button 
              onClick={() => setIsModalOpen(true)}
              className="bg-emerald-500 hover:bg-emerald-600 text-white px-5 py-2.5 rounded-xl font-semibold flex items-center gap-2 shadow-lg shadow-emerald-200"
            >
              <Plus className="w-5 h-5" /> Nuevo Paciente
            </button>
          </header>

          {/* TABLA ESTILO CARD */}
        <div className="bg-white rounded-3xl border border-slate-200 shadow-sm overflow-hidden">
          <table className="w-full">
            <thead>
              <tr className="bg-slate-50/50 border-b border-slate-200">
                <th className="px-8 py-5 text-left text-xs font-bold text-slate-400 uppercase tracking-wider">DNI</th>
                <th className="px-8 py-5 text-left text-xs font-bold text-slate-400 uppercase tracking-wider">Nombre</th>
                <th className="px-8 py-5 text-left text-xs font-bold text-slate-400 uppercase tracking-wider">Dirección</th>
                <th className="px-8 py-5 text-left text-xs font-bold text-slate-400 uppercase tracking-wider">Teléfono</th>
                <th className="px-8 py-5 text-right text-xs font-bold text-slate-400 uppercase tracking-wider">Acciones</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
            {filteredPatients.map((p) => (
                <tr key={p.dni} className="hover:bg-slate-50/50 transition-colors group">
                <td className="px-8 py-5 text-sm font-bold text-slate-700">{p.dni}</td>
                <td className="px-8 py-5 text-sm text-slate-600 font-medium">{p.nombre} {p.apellido}</td>
                <td className="px-8 py-5 text-sm text-slate-500">{p.direccion || '-'}</td>
                <td className="px-8 py-5 text-sm text-slate-500">{p.telefono || '-'}</td>
                <td className="px-8 py-5 text-right">
                  <div className="flex justify-end gap-2">
                    {/* Botón Editar */}
                    <button 
                        onClick={() => console.log("Editar", p.dni)} 
                        className="p-2 text-slate-400 hover:text-emerald-500 hover:bg-emerald-50 rounded-lg transition-all"
                    >
                        <Pencil className="w-4 h-4" /> 
                    </button>
                    
                    {/* Botón Eliminar */}
                    <button 
                        onClick={() => handleDelete(p.dni)} 
                        className="p-2 text-slate-400 hover:text-red-500 hover:bg-red-50 rounded-lg transition-all"
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
        </div>
      </main>

      {/* --- MODAL DE NUEVO PACIENTE --- */}
      {isModalOpen && (
        <div className="fixed inset-0 bg-slate-900/50 backdrop-blur-sm flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-3xl w-full max-w-md p-8 shadow-2xl">
            <div className="flex justify-between items-center mb-6">
              <h2 className="text-2xl font-bold text-slate-800">Nuevo Paciente</h2>
              <button onClick={() => setIsModalOpen(false)}><X className="text-slate-400" /></button>
            </div>
            
            <form onSubmit={handleSave} className="space-y-4">
              <input type="number" placeholder="DNI" className="w-full p-3 border border-slate-200 rounded-xl" required
                onChange={e => setNewPatient({...newPatient, dni: e.target.value})} />
              
              <div className="flex gap-4">
                <input type="text" placeholder="Nombre" className="w-1/2 p-3 border border-slate-200 rounded-xl" required
                  onChange={e => setNewPatient({...newPatient, nombre: e.target.value})} />
                <input type="text" placeholder="Apellido" className="w-1/2 p-3 border border-slate-200 rounded-xl" required
                  onChange={e => setNewPatient({...newPatient, apellido: e.target.value})} />
              </div>

              <input type="text" placeholder="Dirección" className="w-full p-3 border border-slate-200 rounded-xl"
                onChange={e => setNewPatient({...newPatient, direccion: e.target.value})} />
              
              <input type="text" placeholder="Teléfono" className="w-full p-3 border border-slate-200 rounded-xl"
                onChange={e => setNewPatient({...newPatient, telefono: e.target.value})} />

              <button type="submit" className="w-full bg-emerald-500 text-white py-3 rounded-xl font-bold mt-4">
                Guardar Paciente
              </button>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}