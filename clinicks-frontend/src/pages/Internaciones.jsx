import { useState, useEffect, useMemo } from "react"
import { Search, LogOut, CheckCircle, Activity } from "lucide-react"
import api from "../api/api"

export default function Internaciones() {
  const [internaciones, setInternaciones] = useState([])
  const [searchTerm, setSearchTerm] = useState("")

  const obtenerInternaciones = async () => {
    try {
      const res = await api.get('/Internaciones/activas')
      setInternaciones(res.data)
    } catch (err) {
      console.error("Error al obtener internaciones", err)
    }
  }

  useEffect(() => {
    obtenerInternaciones()
  }, [])

  const darDeAlta = async (idInternacion) => {
    if (window.confirm("¿Está seguro de que desea dar de alta a este paciente? Esta acción registrará un egreso y liberará la cama.")) {
      try {
        await api.post(`/Internaciones/${idInternacion}/alta`)
        alert("Paciente dado de alta exitosamente.")
        obtenerInternaciones() // Refrescar la lista
      } catch (err) {
        alert(err.response?.data || "No se pudo dar de alta al paciente.")
      }
    }
  }

  const filteredInternaciones = useMemo(() => {
    const term = searchTerm.toLowerCase()
    return internaciones.filter(i => 
      i.dni.toString().includes(term) || 
      i.nombrePaciente.toLowerCase().includes(term) || 
      i.apellidoPaciente.toLowerCase().includes(term)
    )
  }, [internaciones, searchTerm])

  return (
    <>
      <header className="flex justify-between items-center mb-10">
        <div>
          <h1 className="text-3xl font-bold text-slate-900">Internaciones Activas</h1>
          <p className="text-slate-500 mt-1">Control de pacientes internados actualmente</p>
        </div>
      </header>

      {/* Buscador */}
      <div className="mb-8 relative max-w-md">
        <Search className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
        <input
          type="text"
          placeholder="Buscar por DNI o Nombre..."
          className="w-full pl-12 pr-4 py-3 bg-white border border-slate-200 rounded-2xl focus:outline-none focus:ring-4 focus:ring-emerald-500/10 shadow-sm"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
        />
      </div>

      {/* Tabla */}
      <div className="bg-white rounded-3xl border border-slate-200 shadow-sm overflow-hidden overflow-x-auto">
        <table className="w-full text-left">
          <thead>
            <tr className="bg-slate-50/50 border-b border-slate-200">
              <th className="px-8 py-5 text-xs font-bold text-slate-400 uppercase">Paciente</th>
              <th className="px-8 py-5 text-xs font-bold text-slate-400 uppercase">Ubicación</th>
              <th className="px-8 py-5 text-xs font-bold text-slate-400 uppercase">Ingreso</th>
              <th className="px-8 py-5 text-xs font-bold text-slate-400 uppercase">Estado</th>
              <th className="px-8 py-5 text-xs font-bold text-slate-400 uppercase text-right">Acciones</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100">
            {filteredInternaciones.length === 0 ? (
                <tr>
                    <td colSpan="5" className="px-8 py-10 text-center text-slate-500 font-medium">
                        No hay pacientes internados actualmente.
                    </td>
                </tr>
            ) : (
                filteredInternaciones.map((i) => (
                    <tr key={i.idInternacion} className="hover:bg-slate-50/50 transition-colors">
                        <td className="px-8 py-5">
                            <p className="text-sm font-bold text-slate-700">{i.nombrePaciente} {i.apellidoPaciente}</p>
                            <p className="text-xs text-slate-500 font-mono mt-0.5">DNI: {i.dni}</p>
                        </td>
                        <td className="px-8 py-5 text-sm font-medium text-slate-600">
                            {i.habitacionNombre} - Cama {i.nCama}
                        </td>
                        <td className="px-8 py-5 text-sm text-slate-500 font-medium">
                            {new Date(i.fechaInicio).toLocaleDateString('es-AR', {
                                day: '2-digit', month: '2-digit', year: 'numeric',
                                hour: '2-digit', minute: '2-digit'
                            })}
                        </td>
                        <td className="px-8 py-5">
                            <span className="inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-bold bg-emerald-100 text-emerald-700">
                                <Activity className="w-3.5 h-3.5" /> Activo
                            </span>
                        </td>
                        <td className="px-8 py-5 text-right">
                            <button 
                                onClick={() => darDeAlta(i.idInternacion)}
                                className="inline-flex items-center gap-2 px-4 py-2 bg-slate-800 hover:bg-slate-900 text-white text-sm font-bold rounded-xl transition-colors shadow-sm"
                            >
                                <CheckCircle className="w-4 h-4" />
                                Dar de Alta
                            </button>
                        </td>
                    </tr>
                ))
            )}
          </tbody>
        </table>
      </div>
    </>
  )
}
