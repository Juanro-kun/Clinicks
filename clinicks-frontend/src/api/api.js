import axios from 'axios';

const api = axios.create({
    // REVISÁ que este puerto sea el mismo que usa tu API de .NET (miralo en el Swagger)
    baseURL: 'https://localhost:7296/api' 
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