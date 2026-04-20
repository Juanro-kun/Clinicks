import { Outlet, useNavigate, Link } from 'react-router-dom';
import { Users, Bed, ArrowLeftRight, LogOut } from 'lucide-react';

const Layout = () => {
    const navigate = useNavigate();

    const handleLogout = () => {
        localStorage.removeItem('token');
        navigate('/login');
    };

    return (
        <div className="flex min-h-screen bg-[#f8fafc]">
            {/* SIDEBAR FIJO */}
            <aside className="w-64 bg-white border-r border-slate-200 p-6 hidden md:flex flex-col fixed h-full">
                <div className="flex items-center gap-2 mb-10 px-2">
                    <div className="w-8 h-8 bg-emerald-500 rounded-lg flex items-center justify-center text-white font-bold">C</div>
                    <h2 className="text-xl font-bold text-slate-800">Clinicks</h2>
                </div>
                
                <nav className="space-y-1 flex-1">
                    <Link to="/pacientes" className="flex items-center gap-3 px-4 py-3 text-emerald-600 bg-emerald-50 rounded-xl font-medium">
                        <Users className="w-5 h-5" /> Pacientes
                    </Link>
                    <div className="flex items-center gap-3 px-4 py-3 text-slate-400 cursor-not-allowed">
                        <Bed className="w-5 h-5" /> Camas
                    </div>
                    <div className="flex items-center gap-3 px-4 py-3 text-slate-400 cursor-not-allowed">
                        <ArrowLeftRight className="w-5 h-5" /> Ingresos
                    </div>
                </nav>

                <button 
                    onClick={handleLogout}
                    className="flex items-center gap-3 px-4 py-3 text-slate-400 hover:text-red-500 transition-colors mt-auto"
                >
                    <LogOut className="w-5 h-5" /> Cerrar Sesión
                </button>
            </aside>

            {/* CONTENIDO DINÁMICO */}
            <main className="flex-1 md:ml-64 p-8">
                <div className="max-w-6xl mx-auto">
                    <Outlet /> {/* AQUÍ SE CARGAN LAS PÁGINAS */}
                </div>
            </main>
        </div>
    );
};

export default Layout;