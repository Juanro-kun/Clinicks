import { useState, useEffect, useMemo } from "react"
import { Bed, UserCheck, Search } from "lucide-react"
import api from "../api/api"

export default function Habitaciones() {
  const [habitaciones, setHabitaciones] = useState([])
  const [camas, setCamas] = useState([])
  const [selectedHabitacion, setSelectedHabitacion] = useState("all")

  useEffect(() => {
    // 1. Traer lista de habitaciones para el select
    const fetchHabitaciones = async () => {
      try {
        const res = await api.get('/Habitaciones')
        setHabitaciones(res.data)
      } catch (err) {
        console.error("Error al obtener habitaciones", err)
      }
    }

    // 2. Traer TODAS las camas
    const fetchCamas = async () => {
      try {
        const res = await api.get('/Habitaciones/camas')
        setCamas(res.data)
      } catch (err) {
        console.error("Error al obtener camas", err)
      }
    }

    fetchHabitaciones()
    fetchCamas()
  }, [])

  // Filtrar camas segun la habitacion seleccionada
  const camasFiltradas = useMemo(() => {
    if (selectedHabitacion === "all") {
      return camas
    }
    return camas.filter(c => c.idHabitacion.toString() === selectedHabitacion)
  }, [camas, selectedHabitacion])

  // Separar en Libres y Ocupadas
  const camasLibres = camasFiltradas.filter(c => !c.estaOcupada)
  const camasOcupadas = camasFiltradas.filter(c => c.estaOcupada)

  return (
    <div>
      <header className="flex justify-between items-center mb-10">
        <div>
          <h1 className="text-3xl font-bold text-slate-900">Gestión de Habitaciones</h1>
          <p className="text-slate-500 mt-1">Control de ocupación y asignación de camas</p>
        </div>
      </header>

      {/* Menu desplegable de Habitaciones */}
      <div className="mb-8 relative max-w-md">
        <select
          className="w-full pl-4 pr-10 py-3 bg-white border border-slate-200 rounded-2xl focus:outline-none focus:ring-4 focus:ring-emerald-500/10 shadow-sm appearance-none cursor-pointer font-medium text-slate-700"
          value={selectedHabitacion}
          onChange={(e) => setSelectedHabitacion(e.target.value)}
        >
          <option value="all">Todas las Habitaciones</option>
          {habitaciones.map(h => (
            <option key={h.idHabitacion} value={h.idHabitacion}>
              {h.nombre}
            </option>
          ))}
        </select>
        <div className="absolute right-4 top-1/2 -translate-y-1/2 pointer-events-none text-slate-400">
          ▼
        </div>
      </div>

      <div className="space-y-10">
        
        {/* SECCIÓN: CAMAS LIBRES */}
        <section>
          <div className="flex items-center gap-2 mb-4">
            <div className="w-3 h-3 rounded-full bg-emerald-500"></div>
            <h2 className="text-xl font-bold text-slate-800">Camas Libres ({camasLibres.length})</h2>
          </div>
          
          {camasLibres.length === 0 ? (
            <p className="text-slate-500 italic">No hay camas libres en este momento.</p>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
              {camasLibres.map((cama, idx) => (
                <div key={idx} className="bg-white border-2 border-emerald-100 rounded-2xl p-5 hover:shadow-md hover:border-emerald-200 transition-all">
                  <div className="flex justify-between items-start mb-2">
                    <div className="bg-emerald-50 text-emerald-600 p-2 rounded-xl">
                      <Bed className="w-6 h-6" />
                    </div>
                    <span className="px-3 py-1 bg-emerald-100 text-emerald-700 text-xs font-bold rounded-full">Disponible</span>
                  </div>
                  <h3 className="text-lg font-bold text-slate-800 mt-2">Cama {cama.nCama}</h3>
                  <p className="text-sm font-medium text-slate-500">{cama.habitacionNombre}</p>
                </div>
              ))}
            </div>
          )}
        </section>

        {/* SECCIÓN: CAMAS OCUPADAS */}
        <section>
          <div className="flex items-center gap-2 mb-4">
            <div className="w-3 h-3 rounded-full bg-rose-500"></div>
            <h2 className="text-xl font-bold text-slate-800">Camas Ocupadas ({camasOcupadas.length})</h2>
          </div>

          {camasOcupadas.length === 0 ? (
            <p className="text-slate-500 italic">No hay camas ocupadas en este momento.</p>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
              {camasOcupadas.map((cama, idx) => (
                <div key={idx} className="bg-white border-2 border-rose-100 rounded-2xl p-5 shadow-sm hover:shadow-md transition-all">
                  <div className="flex justify-between items-start mb-4">
                    <div className="bg-rose-50 text-rose-500 p-2 rounded-xl">
                      <Bed className="w-6 h-6" />
                    </div>
                    <span className="px-3 py-1 bg-rose-100 text-rose-700 text-xs font-bold rounded-full">Ocupada</span>
                  </div>
                  
                  <h3 className="text-lg font-bold text-slate-800">Cama {cama.nCama}</h3>
                  <p className="text-sm font-medium text-slate-500 mb-4">{cama.habitacionNombre}</p>
                  
                  <div className="pt-4 border-t border-slate-100">
                    <div className="flex items-center gap-2 text-slate-700">
                      <UserCheck className="w-4 h-4 text-slate-400" />
                      <span className="text-sm font-bold">
                        {cama.nombrePaciente} {cama.apellidoPaciente}
                      </span>
                    </div>
                    {cama.dniPaciente && (
                      <p className="text-xs text-slate-400 ml-6 mt-0.5">DNI: {cama.dniPaciente}</p>
                    )}
                  </div>
                </div>
              ))}
            </div>
          )}
        </section>

      </div>
    </div>
  )
}
