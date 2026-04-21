import { Navigate, Outlet } from 'react-router-dom';

const ProtectedRoute = () => {
    // Chequeamos si el token existe en el "bolsillo" del navegador
    const token = localStorage.getItem('token');

    // Si NO hay token, lo mandamos al login
    if (!token) {
        return <Navigate to="/login" replace />;
    }

    // Si hay token, que pase nomás
    return <Outlet />;
};

export default ProtectedRoute;