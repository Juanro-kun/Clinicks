import axios from 'axios';

const api = axios.create({
    // Apuntamos al puerto HTTP que levanta `dotnet run` por defecto
    baseURL: 'http://localhost:5223/api' 
});

// Este es el "Interceptor": antes de mandar CUALQUIER pedido, 
// se fija si hay un token en el navegador y lo pega en el encabezado.
api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

export default api;