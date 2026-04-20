import { useNavigate } from 'react-router-dom';
import { useState } from 'react';
import api from '../api/api';

const Login = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');

    const navigate = useNavigate();

    const handleLogin = async (e) => {
        e.preventDefault();
        try {
            // Le pegamos al endpoint que hicimos en C#
            const res = await api.post('/Auth/login', { username, password });
            
            // Guardamos el token en el "bolsillo" del navegador
            localStorage.setItem('token', res.data.token);

            
            alert("¡Logueado con éxito! Ya tenemos el token.");
            // Después acá vamos a meter la redirección a pacientes
            navigate('/pacientes');
        } catch (err) {
            console.error(err);
            alert("Fallo el login. Revisá el usuario o que la API esté prendida.");
        }
    };

    return (
        <div style={{ padding: '20px', textAlign: 'center' }}>
            
            <h2>Clinicks - Acceso</h2>
            <form onSubmit={handleLogin}>
                <input 
                    type="text" 
                    placeholder="Usuario" 
                    onChange={e => setUsername(e.target.value)} 
                    style={{ marginBottom: '10px', padding: '5px' }}
                /><br />
                <input 
                    type="password" 
                    placeholder="Contraseña" 
                    onChange={e => setPassword(e.target.value)} 
                    style={{ marginBottom: '10px', padding: '5px' }}
                /><br />
                <button type="submit" style={{ padding: '10px 20px', cursor: 'pointer' }}>
                    Entrar
                </button>
            </form>
        </div>
    );
};

export default Login;