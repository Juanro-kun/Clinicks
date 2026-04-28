import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Layout from './components/Layout';
import Login from './pages/Login';
import Pacientes from './pages/Pacientes';
import Habitaciones from './pages/Habitaciones';
import Medicos from './pages/Medicos';
import Internaciones from './pages/Internaciones';
import ProtectedRoute from './components/ProtectedRoute';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/login" element={<Login />} />
        
        <Route element={<ProtectedRoute />}>
        {/* Rutas con Layout */}
          <Route element={<Layout />}>
            <Route path="/pacientes" element={<Pacientes />} />
            <Route path="/habitaciones" element={<Habitaciones />} />
            <Route path="/medicos" element={<Medicos />} />
            <Route path="/internaciones" element={<Internaciones />} />
            {/* otras paginas que requieran layoutn */}
          </Route>
        </Route> 

        <Route path="/" element={<Navigate to="/login" />} />
      </Routes>
    </Router>
  );
}

export default App;