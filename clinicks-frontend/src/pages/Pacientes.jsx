import { useState, useEffect, useMemo } from "react"
import { Plus, Search, LogOut, Users, ClipboardList, X, Trash2, Pencil, BedDouble } from "lucide-react"
import api from "../api/api"
import { useNavigate } from "react-router-dom"

export default function Pacientes() {
  const [patients, setPatients] = useState([])
  const [searchTerm, setSearchTerm] = useState("")
  const [isModalOpen, setIsModalOpen] = useState(false)
  
  // Estado para saber si estamos editando o creando uno nuevo
  const [isEditing, setIsEditing] = useState(false)
  
  const [patientForm, setPatientForm] = useState({ 
    dni: '', nombre: '', apellido: '', telefono: '',
    calle: '', altura: '', ciudadNombre: '', provinciaNombre: '', paisNombre: '' 
  })

  // Listas para autocompletado
  const [callesList, setCallesList] = useState([])
  const [ciudadesList, setCiudadesList] = useState([])
  const [provinciasList, setProvinciasList] = useState([])
  const [paisesList, setPaisesList] = useState([])
  
  // Estados para Internación
  const [isInternarModalOpen, setIsInternarModalOpen] = useState(false)
  const [pacienteAInternar, setPacienteAInternar] = useState(null)
  const [habitacionesList, setHabitacionesList] = useState([])
  const [camasList, setCamasList] = useState([])
  const [internarForm, setInternarForm] = useState({ idHabitacion: '', nCama: '' })
  
  const navigate = useNavigate()

  const obtenerListaDePacientes = async () => {
    try {
      const res = await api.get('/Pacientes')
      setPatients(res.data)
    } catch (err) {
      if (err.response?.status === 401) navigate('/login')
    }
  }

  const obtenerListasDeUbicaciones = async () => {
    try {
      const [resCalles, resCiudades, resProvincias, resPaises] = await Promise.all([
        api.get('/Ubicaciones/calles'),
        api.get('/Ubicaciones/ciudades'),
        api.get('/Ubicaciones/provincias'),
        api.get('/Ubicaciones/paises')
      ])
      setCallesList(resCalles.data)
      setCiudadesList(resCiudades.data)
      setProvinciasList(resProvincias.data)
      setPaisesList(resPaises.data)
    } catch (err) {
      console.error("Error al obtener ubicaciones", err)
    }
  }

  useEffect(() => { 
    obtenerListaDePacientes();
    obtenerListasDeUbicaciones();
  }, [])

  // 1. FUNCIÓN PARA PREPARAR LA EDICIÓN
  const prepararEdicionDePaciente = (p) => {
    setPatientForm(p) // Cargamos los datos del paciente en el form
    setIsEditing(true) // Activamos modo edición
    setIsModalOpen(true) // Abrimos el modal
  }

  // 2. FUNCIÓN PARA CERRAR Y LIMPIAR
  const cerrarFormularioPaciente = () => {
    setIsModalOpen(false)
    setIsEditing(false)
    setPatientForm({ 
      dni: '', nombre: '', apellido: '', telefono: '',
      calle: '', altura: '', ciudadNombre: '', provinciaNombre: '', paisNombre: '' 
    })
  }

  // LÓGICA DE INTERNACIÓN
  const abrirModalInternacion = async (p) => {
    setPacienteAInternar(p)
    setIsInternarModalOpen(true)
    setInternarForm({ idHabitacion: '', nCama: '' })
    
    try {
      const resHab = await api.get('/Habitaciones')
      setHabitacionesList(resHab.data)
      const resCam = await api.get('/Habitaciones/camas')
      // Solo guardamos camas libres
      setCamasList(resCam.data.filter(c => !c.estaOcupada))
    } catch (err) {
      console.error(err)
    }
  }

  const confirmarInternacion = async (e) => {
    e.preventDefault()
    if(!internarForm.idHabitacion || !internarForm.nCama) {
        alert("Seleccione habitación y cama")
        return
    }
    
    try {
        await api.post('/Internaciones/internar', {
            dni: pacienteAInternar.dni,
            idHabitacion: parseInt(internarForm.idHabitacion),
            nCama: parseInt(internarForm.nCama)
        })
        alert("¡Paciente internado con éxito!")
        setIsInternarModalOpen(false)
    } catch(err) {
        alert(err.response?.data || "Error al internar")
    }
  }

  // Capitaliza la primera letra de cada palabra
  const capitalizarPalabras = (texto) => {
    if (!texto) return '';
    return texto.toLowerCase().split(' ').map(palabra => 
        palabra.charAt(0).toUpperCase() + palabra.slice(1)
    ).join(' ');
  }

  // 3. GUARDAR (POST o PUT)
  const registrarOActualizarPaciente = async (e) => {
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

        // Formatear ubicaciones para que tengan la primera letra mayúscula
        const formAEnviar = {
            ...patientForm,
            calle: capitalizarPalabras(patientForm.calle),
            ciudadNombre: capitalizarPalabras(patientForm.ciudadNombre),
            provinciaNombre: capitalizarPalabras(patientForm.provinciaNombre),
            paisNombre: capitalizarPalabras(patientForm.paisNombre)
        };

        // 2. Si pasó los filtros, recién ahí vamos a la API
        try {
            if (isEditing) {
                await api.put(`/Pacientes/${patientForm.dni}`, formAEnviar);
            } else {
                await api.post('/Pacientes', formAEnviar);
            }
            cerrarFormularioPaciente();
            obtenerListaDePacientes();
            obtenerListasDeUbicaciones(); // Refrescar las listas por si se agregó algo nuevo
        } catch (err) {
            console.log("Error completo:", err.response); // Esto miralo en la consola (F12)
            
            // Si la API mandó un objeto, lo convertimos a texto para el alert
            const mensajeError = err.response?.data 
                ? JSON.stringify(err.response.data) 
                : err.message;
                
            alert("Error detallado: " + mensajeError);
        }
    };

  const eliminarRegistroPaciente = async (dni) => {
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
        <div className="bg-white rounded-3xl border border-slate-200 shadow-sm overflow-hidden overflow-x-auto">
        <table className="w-full text-left">
            <thead>
            <tr className="bg-slate-50/50 border-b border-slate-200">
                <th className="px-8 py-5 text-xs font-bold text-slate-400 uppercase">DNI</th>
                <th className="px-8 py-5 text-xs font-bold text-slate-400 uppercase">Nombre</th>
                <th className="px-8 py-5 text-xs font-bold text-slate-400 uppercase">Teléfono</th>
                <th className="px-8 py-5 text-xs font-bold text-slate-400 uppercase text-right">Acciones</th>
            </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
            {filteredPatients.map((p) => (
                <tr key={p.dni} className="hover:bg-slate-50/50 transition-colors">
                <td className="px-8 py-5 text-sm font-bold text-slate-700">{p.dni}</td>
                <td className="px-8 py-5">
                    <div className="flex items-center gap-3">
                        <span className="text-sm text-slate-600 font-medium">
                            {p.nombre} {p.apellido}
                        </span>
                        {p.estaInternado && (
                            <span className="px-2 py-0.5 rounded-md bg-rose-100 text-rose-600 text-[10px] font-bold uppercase tracking-wider">
                                Internado
                            </span>
                        )}
                    </div>
                </td>
                {/* Columna de Teléfono */}
                <td className="px-8 py-5 text-sm text-slate-500 font-mono">
                    {p.telefono || <span className="text-slate-300">---</span>}
                </td>
                <td className="px-8 py-5 text-right">
                    <div className="flex justify-end gap-2">
                    <button 
                        onClick={() => abrirModalInternacion(p)}
                        className="p-2 text-slate-400 hover:text-blue-500 hover:bg-blue-50 rounded-lg transition-colors"
                        title="Internar"
                    >
                        <BedDouble className="w-4 h-4" />
                    </button>
                    <button 
                        onClick={() => prepararEdicionDePaciente(p)}
                        className="p-2 text-slate-400 hover:text-emerald-500 hover:bg-emerald-50 rounded-lg transition-colors"
                        title="Editar"
                    >
                        <Pencil className="w-4 h-4" />
                    </button>
                    <button 
                        onClick={() => eliminarRegistroPaciente(p.dni)}
                        className="p-2 text-slate-400 hover:text-red-500 hover:bg-red-50 rounded-lg transition-colors"
                        title="Eliminar"
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
              <button onClick={cerrarFormularioPaciente}><X className="text-slate-400" /></button>
            </div>
            
            <form onSubmit={registrarOActualizarPaciente} className="space-y-4">
              {/* Si estamos editando, el DNI suele ser fijo (PK) */}
              <input 
                type="number" placeholder="DNI" 
                className={`w-full p-3 border border-slate-200 rounded-xl ${isEditing ? 'bg-slate-50 text-slate-400' : ''}`}
                value={patientForm.dni}
                disabled={isEditing}
                required
                maxLength={8}
                onChange={e =>{
                    const valorLimpio = e.target.value.replace(/\D/g, '');
                    setPatientForm({...patientForm, dni: valorLimpio}) 
                }}/>
              
              <div className="flex gap-4">
                <input type="text" placeholder="Nombre" maxLength={50} className="w-1/2 p-3 border border-slate-200 rounded-xl" required
                  value={patientForm.nombre}
                  onChange={e => setPatientForm({...patientForm, nombre: e.target.value})} />
                <input type="text" placeholder="Apellido" maxLength={50} className="w-1/2 p-3 border border-slate-200 rounded-xl" required
                  value={patientForm.apellido}
                  onChange={e => setPatientForm({...patientForm, apellido: e.target.value})} />
              </div>

              <div className="flex gap-4">
                <input type="text" placeholder="Calle" list="calles-datalist" className="w-2/3 p-3 border border-slate-200 rounded-xl"
                  value={patientForm.calle || ''}
                  onChange={e => setPatientForm({...patientForm, calle: e.target.value})} />
                <datalist id="calles-datalist">
                  {callesList.map(c => <option key={c} value={c} />)}
                </datalist>

                <input type="number" placeholder="Altura" className="w-1/3 p-3 border border-slate-200 rounded-xl"
                  value={patientForm.altura || ''}
                  onChange={e => setPatientForm({...patientForm, altura: e.target.value})} />
              </div>

              <div className="flex gap-4">
                <input type="text" placeholder="Ciudad" list="ciudades-datalist" className="w-1/3 p-3 border border-slate-200 rounded-xl"
                  value={patientForm.ciudadNombre || ''}
                  onChange={e => setPatientForm({...patientForm, ciudadNombre: e.target.value})} />
                <datalist id="ciudades-datalist">
                  {ciudadesList.map(c => <option key={c} value={c} />)}
                </datalist>

                <input type="text" placeholder="Provincia" list="provincias-datalist" className="w-1/3 p-3 border border-slate-200 rounded-xl"
                  value={patientForm.provinciaNombre || ''}
                  onChange={e => setPatientForm({...patientForm, provinciaNombre: e.target.value})} />
                <datalist id="provincias-datalist">
                  {provinciasList.map(p => <option key={p} value={p} />)}
                </datalist>

                <input type="text" placeholder="País" list="paises-datalist" className="w-1/3 p-3 border border-slate-200 rounded-xl"
                  value={patientForm.paisNombre || ''}
                  onChange={e => setPatientForm({...patientForm, paisNombre: e.target.value})} />
                <datalist id="paises-datalist">
                  {paisesList.map(p => <option key={p} value={p} />)}
                </datalist>
              </div>
              
              <input 
                type="tel" 
                placeholder="Teléfono" 
                className="w-full p-3 border border-slate-200 rounded-xl"
                value={patientForm.telefono || ''}
                onChange={e => {
                    const valorLimpio2 = e.target.value.replace(/\D/g, '');
                    setPatientForm({ ...patientForm, telefono: valorLimpio2 });
                }} />

              <button type="submit" className="w-full bg-emerald-500 text-white py-3 rounded-xl font-bold mt-4 shadow-lg shadow-emerald-200">
                {isEditing ? 'Actualizar Datos' : 'Guardar Paciente'}
              </button>
            </form>
          </div>
        </div>
      )}

      {/* MODAL DE INTERNACIÓN */}
      {isInternarModalOpen && (
        <div className="fixed inset-0 bg-slate-900/50 backdrop-blur-sm flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-3xl w-full max-w-md p-8 shadow-2xl">
            <div className="flex justify-between items-center mb-6">
              <h2 className="text-2xl font-bold text-slate-800">
                Internar Paciente
              </h2>
              <button onClick={() => setIsInternarModalOpen(false)}><X className="text-slate-400" /></button>
            </div>
            
            <p className="text-slate-600 mb-6">
              Asignar cama para: <strong>{pacienteAInternar?.nombre} {pacienteAInternar?.apellido}</strong> (DNI: {pacienteAInternar?.dni})
            </p>

            <form onSubmit={confirmarInternacion} className="space-y-4">
              
              <div>
                <label className="block text-sm font-bold text-slate-700 mb-2">Habitación</label>
                <select 
                  className="w-full p-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none"
                  value={internarForm.idHabitacion}
                  onChange={(e) => setInternarForm({ idHabitacion: e.target.value, nCama: '' })}
                  required
                >
                  <option value="">Seleccione una habitación</option>
                  {habitacionesList.map(h => (
                    <option key={h.idHabitacion} value={h.idHabitacion}>{h.nombre}</option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-bold text-slate-700 mb-2">Cama Disponible</label>
                <select 
                  className="w-full p-3 border border-slate-200 rounded-xl focus:ring-2 focus:ring-blue-500 outline-none disabled:bg-slate-50"
                  value={internarForm.nCama}
                  onChange={(e) => setInternarForm({ ...internarForm, nCama: e.target.value })}
                  disabled={!internarForm.idHabitacion}
                  required
                >
                  <option value="">Seleccione una cama libre</option>
                  {camasList
                    .filter(c => c.idHabitacion.toString() === internarForm.idHabitacion)
                    .map(c => (
                      <option key={`${c.idHabitacion}-${c.nCama}`} value={c.nCama}>
                        Cama {c.nCama}
                      </option>
                  ))}
                </select>
                {internarForm.idHabitacion && camasList.filter(c => c.idHabitacion.toString() === internarForm.idHabitacion).length === 0 && (
                    <p className="text-rose-500 text-sm mt-2">No hay camas libres en esta habitación.</p>
                )}
              </div>

              <button type="submit" className="w-full bg-blue-500 hover:bg-blue-600 transition-colors text-white py-3 rounded-xl font-bold mt-4 shadow-lg shadow-blue-200">
                Confirmar Internación
              </button>
            </form>
          </div>
        </div>
      )}
    </>
  )
}