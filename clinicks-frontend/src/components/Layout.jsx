import { Outlet, useNavigate, Link, useLocation } from 'react-router-dom';
import { Users, Bed, Stethoscope, LogOut, LayoutDashboard, ClipboardList } from 'lucide-react';


const Layout = () => {
    const navigate = useNavigate();
    const location = useLocation(); // Para saber en qué página estamos

    const handleLogout = () => {
        localStorage.removeItem('token');
        navigate('/login');
    };

    // Función para ver si el link está activo
    const isActive = (path) => location.pathname === path;

    return (
        <div className="flex min-h-screen bg-[#f8fafc]">
            {/* SIDEBAR */}
            <aside className="w-80 bg-white border-r border-slate-100 p-10 hidden md:flex flex-col fixed h-full shadow-sm gap-y-8">
                
                {/* Logo: Subimos el margen de mb-12 a mb-24 */}
                <div className="flex items-center gap-4 mb-24 px-2"> 
                    <div className="w-12 h-12 bg-gradient-to-br from-emerald-400 to-emerald-600 rounded-2xl flex items-center justify-center text-white font-black shadow-lg shadow-emerald-200 text-2xl">
                        C
                    </div>
                    <div>
                        <h2 className="text-2xl font-black text-slate-800 leading-none">Clinicks</h2>
                        <span className="text-[10px] font-bold text-slate-400 uppercase tracking-[0.2em] mt-1 block">Hospital System</span>
                    </div>
                </div>
                
                {/* Navegación: usamos gap-y-6 para separar los botones entre sí */}
                <nav className="flex flex-col flex-1 gap-y-6">
                    <Link 
                        to="/pacientes" 
                        className={`flex items-center gap-5 px-6 py-3 rounded-[1.5rem] font-bold transition-all ${
                            isActive('/pacientes') 
                            ? 'text-emerald-600 bg-emerald-50 shadow-sm shadow-emerald-100/30' 
                            : 'text-slate-400 hover:text-slate-600 hover:bg-slate-50'
                        }`}
                    >
                        <Users className={`w-6 h-6 ${isActive('/pacientes') ? 'text-emerald-500' : ''}`} /> 
                        <span className="text-lg">Pacientes</span>
                    </Link>

                    {/* Botón Habitaciones */}
                    <Link 
                        to="/habitaciones" 
                        className={`flex items-center gap-5 px-6 py-3 rounded-[1.5rem] font-bold transition-all ${
                            isActive('/habitaciones') 
                            ? 'text-emerald-600 bg-emerald-50 shadow-sm shadow-emerald-100/30' 
                            : 'text-slate-400 hover:text-slate-600 hover:bg-slate-50'
                        }`}
                    >
                        <Bed className={`w-6 h-6 ${isActive('/habitaciones') ? 'text-emerald-500' : ''}`} /> 
                        <span className="text-lg">Habitaciones</span>
                    </Link>

                    {/* Botón Internaciones */}
                    <Link 
                        to="/internaciones" 
                        className={`flex items-center gap-5 px-6 py-3 rounded-[1.5rem] font-bold transition-all ${
                            isActive('/internaciones') 
                            ? 'text-emerald-600 bg-emerald-50 shadow-sm shadow-emerald-100/30' 
                            : 'text-slate-400 hover:text-slate-600 hover:bg-slate-50'
                        }`}
                    >
                        <ClipboardList className={`w-6 h-6 ${isActive('/internaciones') ? 'text-emerald-500' : ''}`} /> 
                        <span className="text-lg">Internaciones</span>
                    </Link>

                    {/* Botón Médicos */}
                    <Link 
                        to="/medicos" 
                        className={`flex items-center gap-5 px-6 py-3 rounded-[1.5rem] font-bold transition-all ${
                            isActive('/medicos') 
                            ? 'text-emerald-600 bg-emerald-50 shadow-sm shadow-emerald-100/30' 
                            : 'text-slate-400 hover:text-slate-600 hover:bg-slate-50'
                        }`}
                    >
                        <Stethoscope className={`w-6 h-6 ${isActive('/medicos') ? 'text-emerald-500' : ''}`} /> 
                        <span className="text-lg">Médicos</span>
                    </Link>
                </nav>

                {/* Logout con más aire arriba */}
                <div className="pt-8 border-t border-slate-50">
                    <button 
                        onClick={handleLogout}
                        className="w-full flex items-center gap-5 px-6 py-5 text-slate-400 hover:text-red-500 hover:bg-red-50 rounded-[1.5rem] transition-all font-bold"
                    >
                        <LogOut className="w-6 h-6" /> 
                        <span className="text-lg">Cerrar Sesión</span>
                    </button>
                </div>
            </aside>

            {/* CONTENIDO PRINCIPAL - Expandido */}
            <main className="flex-1 md:ml-72 p-10 lg:p-16">
                <div className="max-w-[1400px] mx-auto">
                    <Outlet /> 
                </div>
            </main>
        </div>
    );
};

export default Layout;