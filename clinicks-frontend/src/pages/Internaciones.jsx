import { useState, useEffect, useMemo } from "react"
import { Search, LogOut, CheckCircle, Activity, ArrowLeftRight, X, ClipboardList } from "lucide-react"
import api from "../api/api"

export default function Internaciones() {
  const [internaciones, setInternaciones] = useState([])
  const [searchTerm, setSearchTerm] = useState("")
  
  const [notification, setNotification] = useState(null)

  const showNotification = (type, message) => {
    setNotification({ type, message })
    setTimeout(() => setNotification(null), 3000)
  }
  
  // Estados para Traslado
  const [isTrasladarModalOpen, setIsTrasladarModalOpen] = useState(false)
  const [pacienteATrasladar, setPacienteATrasladar] = useState(null)
  const [habitacionesList, setHabitacionesList] = useState([])
  const [camasList, setCamasList] = useState([])
  const [trasladoForm, setTrasladoForm] = useState({ idHabitacion: '', nCama: '' })

  // Estados para Dar de Alta (Modal de Confirmación)
  const [isAltaModalOpen, setIsAltaModalOpen] = useState(false)
  const [pacienteAAlta, setPacienteAAlta] = useState(null)

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

  const prepararAlta = (internacion) => {
    setPacienteAAlta(internacion)
    setIsAltaModalOpen(true)
  }

  const confirmarAlta = async () => {
    if (!pacienteAAlta) return;
    try {
      await api.post(`/Internaciones/${pacienteAAlta.dni}/alta`)
      showNotification('success', "Alta medica registrada con exito")
      setIsAltaModalOpen(false)
      setPacienteAAlta(null)
      obtenerInternaciones() // Refrescar la lista
    } catch (err) {
      showNotification('error', err.response?.data || "No se pudo dar de alta al paciente.")
      setIsAltaModalOpen(false)
      setPacienteAAlta(null)
    }
  }

  const abrirModalTraslado = async (internacion) => {
    setPacienteATrasladar(internacion)
    setIsTrasladarModalOpen(true)
    setTrasladoForm({ idHabitacion: '', nCama: '' })
    
    try {
      const resHab = await api.get('/Habitaciones')
      setHabitacionesList(resHab.data)
      const resCam = await api.get('/Habitaciones/camas')
      // Solo guardamos camas libres
      setCamasList(resCam.data.filter(c => !c.estaOcupada))
    } catch (err) {
      console.error("Error al cargar habitaciones y camas para traslado", err)
    }
  }

  const confirmarTraslado = async (e) => {
    e.preventDefault()

    const idHab = parseInt(trasladoForm.idHabitacion);
    const nCama = parseInt(trasladoForm.nCama);

    if (isNaN(idHab) || idHab <= 0) {
      showNotification('error', "Seleccione una habitación de destino válida.");
      return;
    }

    if (isNaN(nCama) || nCama <= 0) {
      showNotification('error', "Seleccione una cama de destino válida.");
      return;
    }
    
    try {
      await api.post('/Internaciones/trasladar', {
        dni: pacienteATrasladar.dni,
        idHabitacion: parseInt(trasladoForm.idHabitacion),
        nCama: parseInt(trasladoForm.nCama)
      })
      showNotification('success', "Traslado realizado con exito")
      setIsTrasladarModalOpen(false)
      obtenerInternaciones()
    } catch (err) {
      showNotification('error', err.response?.data || "Error al trasladar")
    }
  }

  const filteredInternaciones = useMemo(() => {
    if (!searchTerm) return internaciones;
    
    // Función auxiliar para quitar acentos
    const removerAcentos = (str) => {
      return str.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
    }

    const term = removerAcentos(searchTerm.toLowerCase());

    return internaciones.filter(i => {
      const dniStr = i.dni.toString();
      // Concatenar de ambas formas para soportar "Juan Perez" y "Perez Juan"
      const nombreCompleto = removerAcentos(`${i.nombrePaciente} ${i.apellidoPaciente}`.toLowerCase());
      const apellidoNombre = removerAcentos(`${i.apellidoPaciente} ${i.nombrePaciente}`.toLowerCase());
      
      return dniStr.includes(term) || 
             nombreCompleto.includes(term) || 
             apellidoNombre.includes(term)
    })
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
                            {new Date(i.fechaIngreso).toLocaleDateString('es-AR', {
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
                            <div className="flex justify-end gap-2">
                                <button 
                                    onClick={() => abrirModalTraslado(i)}
                                    className="inline-flex items-center gap-2 px-4 py-2 bg-blue-500 hover:bg-blue-600 text-white text-sm font-bold rounded-xl transition-colors shadow-sm"
                                >
                                    <ArrowLeftRight className="w-4 h-4" />
                                    Trasladar
                                </button>
                                <button 
                                    onClick={() => prepararAlta(i)}
                                    className="inline-flex items-center gap-2 px-4 py-2 bg-slate-800 hover:bg-slate-900 text-white text-sm font-bold rounded-xl transition-colors shadow-sm"
                                >
                                    <CheckCircle className="w-4 h-4" />
                                    Dar de Alta
                                </button>
                            </div>
                        </td>
                    </tr>
                ))
            )}
          </tbody>
        </table>
      </div>

      {/* MODAL DE TRASLADO */}
      {isTrasladarModalOpen && (
        <div className="fixed inset-0 bg-slate-900/50 backdrop-blur-sm flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-3xl w-full max-w-md p-8 shadow-2xl">
            <div className="flex justify-between items-center mb-6">
              <h2 className="text-2xl font-bold text-slate-800">
                Trasladar Paciente
              </h2>
              <button onClick={() => setIsTrasladarModalOpen(false)}><X className="text-slate-400" /></button>
            </div>
            
            <p className="text-slate-600 mb-6">
              Trasladar a: <strong>{pacienteATrasladar?.nombrePaciente} {pacienteATrasladar?.apellidoPaciente}</strong> (DNI: {pacienteATrasladar?.dni})
              <br />
              Ubicación actual: <strong>{pacienteATrasladar?.habitacionNombre} - Cama {pacienteATrasladar?.nCama}</strong>
            </p>

            <form onSubmit={confirmarTraslado} className="space-y-4">
              
              <div>
                <label className="block text-sm font-bold text-slate-700 mb-2">Nueva Habitación</label>
                <select 
                  className="w-full p-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none"
                  value={trasladoForm.idHabitacion}
                  onChange={(e) => setTrasladoForm({ idHabitacion: e.target.value, nCama: '' })}
                  required
                >
                  <option value="">Seleccione una habitación</option>
                  {habitacionesList.map(h => (
                    <option key={h.idHabitacion} value={h.idHabitacion}>{h.nombre}</option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-bold text-slate-700 mb-2">Nueva Cama Disponible</label>
                <select 
                  className="w-full p-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none disabled:bg-slate-50"
                  value={trasladoForm.nCama}
                  onChange={(e) => setTrasladoForm({ ...trasladoForm, nCama: e.target.value })}
                  disabled={!trasladoForm.idHabitacion}
                  required
                >
                  <option value="">Seleccione una cama libre</option>
                  {camasList
                    .filter(c => c.idHabitacion.toString() === trasladoForm.idHabitacion)
                    .map(c => (
                      <option key={`${c.idHabitacion}-${c.nCama}`} value={c.nCama}>
                        Cama {c.nCama}
                      </option>
                  ))}
                </select>
                {trasladoForm.idHabitacion && camasList.filter(c => c.idHabitacion.toString() === trasladoForm.idHabitacion).length === 0 && (
                    <p className="text-rose-500 text-sm mt-2">No hay camas libres en esta habitación.</p>
                )}
              </div>

              <button type="submit" className="w-full bg-blue-500 hover:bg-blue-600 transition-colors text-white py-3 rounded-xl font-bold mt-4 shadow-lg shadow-blue-200">
                Confirmar Traslado
              </button>
            </form>
          </div>
        </div>
      )}

      {/* MODAL CONFIRMAR ALTA */}
      {isAltaModalOpen && (
        <div className="fixed inset-0 bg-slate-900/50 backdrop-blur-sm flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-3xl w-full max-w-sm p-8 shadow-2xl text-center">
            <div className="mx-auto w-16 h-16 bg-emerald-100 text-emerald-500 rounded-full flex items-center justify-center mb-6">
              <CheckCircle className="w-8 h-8" />
            </div>
            <h2 className="text-2xl font-bold text-slate-800 mb-2">
              ¿Seguro que desea registrar el Alta Medica?
            </h2>
            <p className="text-slate-500 mb-8">
              Está a punto de dar de alta a <strong>{pacienteAAlta?.nombrePaciente} {pacienteAAlta?.apellidoPaciente}</strong>. Esta acción registrará un egreso y liberará la cama actual.
            </p>
            <div className="flex gap-4">
              <button 
                onClick={() => {
                    setIsAltaModalOpen(false);
                    setPacienteAAlta(null);
                }}
                className="flex-1 py-3 px-4 rounded-xl font-bold text-slate-600 bg-slate-100 hover:bg-slate-200 transition-colors"
              >
                Cancelar
              </button>
              <button 
                onClick={confirmarAlta}
                className="flex-1 py-3 px-4 rounded-xl font-bold text-white bg-emerald-500 hover:bg-emerald-600 transition-colors shadow-lg shadow-emerald-200"
              >
                Confirmar Alta
              </button>
            </div>
          </div>
        </div>
      )}

      {/* NOTIFICACIONES TOAST */}
      {notification && (
        <div className={`fixed bottom-6 right-6 p-4 rounded-2xl shadow-xl flex items-center gap-3 z-50 transition-all duration-300
          ${notification.type === 'success' ? 'bg-emerald-50 text-emerald-600 border border-emerald-200' : 'bg-red-50 text-red-600 border border-red-200'}`}>
          {notification.type === 'success' ? <ClipboardList className="w-5 h-5" /> : <X className="w-5 h-5" />}
          <span className="font-semibold">{notification.message}</span>
        </div>
      )}
    </>
  )
}
