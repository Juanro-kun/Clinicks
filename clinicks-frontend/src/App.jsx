import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Layout from './components/Layout';
import Login from './pages/Login';
import Pacientes from './pages/Pacientes';
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
            {/* otras paginas que requieran layoutn */}
          </Route>
        </Route> 

        <Route path="/" element={<Navigate to="/login" />} />
      </Routes>
    </Router>
  );
}

export default App;