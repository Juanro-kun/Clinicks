import { useNavigate } from 'react-router-dom';
import { useState } from 'react';
import { Lock, User, LogIn } from 'lucide-react'; // Iconos para que quede pro
import api from '../api/api';

const Login = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [loading, setLoading] = useState(false); // Para el feedback del botón

    const navigate = useNavigate();

    const handleLogin = async (e) => {
        e.preventDefault();
        setLoading(true);
        try {
            const res = await api.post('/Auth/login', { username, password });
            localStorage.setItem('token', res.data.token);
            
            // Ya no hace falta el alert feo si todo sale bien, vamos directo
            navigate('/pacientes');
        } catch (err) {
            console.error(err);
            alert("Fallo el login. Revisá el usuario o que la API esté prendida.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-slate-50 flex items-center justify-center p-4">
            <div className="bg-white w-full max-w-md rounded-[2.5rem] p-10 border border-slate-100 shadow-2xl shadow-slate-200/60">
                
                {/* Header del Login */}
                <div className="flex flex-col items-center mb-10">
                    <div className="w-16 h-16 bg-emerald-500 rounded-2xl flex items-center justify-center mb-4 shadow-lg shadow-emerald-200">
                        <LogIn className="text-white w-8 h-8" />
                    </div>
                    <h2 className="text-3xl font-extrabold text-slate-900 tracking-tight">Clinicks</h2>
                    <p className="text-slate-500 mt-2 font-medium">Gestión de Salud Inteligente</p>
                </div>

                <form onSubmit={handleLogin} className="space-y-6">
                    {/* Input Usuario */}
                    <div className="space-y-2">
                        <label className="text-sm font-bold text-slate-700 ml-1">Usuario</label>
                        <div className="relative group">
                            <User className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400 group-focus-within:text-emerald-500 transition-colors" />
                            <input 
                                type="text" 
                                placeholder="Ingresá tu usuario" 
                                required
                                className="w-full pl-12 pr-4 py-4 bg-slate-50 border border-slate-200 rounded-2xl focus:outline-none focus:ring-4 focus:ring-emerald-500/10 focus:border-emerald-500 transition-all font-medium text-slate-600"
                                onChange={e => setUsername(e.target.value)} 
                            />
                        </div>
                    </div>

                    {/* Input Contraseña */}
                    <div className="space-y-2">
                        <label className="text-sm font-bold text-slate-700 ml-1">Contraseña</label>
                        <div className="relative group">
                            <Lock className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400 group-focus-within:text-emerald-500 transition-colors" />
                            <input 
                                type="password" 
                                placeholder="••••••••" 
                                required
                                className="w-full pl-12 pr-4 py-4 bg-slate-50 border border-slate-200 rounded-2xl focus:outline-none focus:ring-4 focus:ring-emerald-500/10 focus:border-emerald-500 transition-all font-medium text-slate-600"
                                onChange={e => setPassword(e.target.value)} 
                            />
                        </div>
                    </div>

                    {/* Botón Entrar */}
                    <button 
                        type="submit" 
                        disabled={loading}
                        className="w-full bg-emerald-500 hover:bg-emerald-600 disabled:bg-slate-300 text-white font-bold py-4 rounded-2xl shadow-lg shadow-emerald-200 transition-all transform active:scale-[0.98] flex items-center justify-center gap-2"
                    >
                        {loading ? 'Validando...' : 'Iniciar Sesión'}
                    </button>
                </form>

                <p className="text-center text-slate-400 text-sm mt-8">
                    © 2026 Clinicks Systems. UNNE.
                </p>
            </div>
        </div>
    );
};

export default Login;