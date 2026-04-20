import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Layout from './components/Layout';
import Login from './pages/Login';
import Pacientes from './pages/Pacientes';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/login" element={<Login />} />
        
        {/* Rutas con Layout */}
        <Route element={<Layout />}>
          <Route path="/pacientes" element={<Pacientes />} />
          {/* Si agregás más páginas, van acá adentro */}
        </Route>

        <Route path="/" element={<Navigate to="/login" />} />
      </Routes>
    </Router>
  );
}

export default App;