import { Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from './contexts/AuthContext';
import { modules } from './modules';
import Layout from './components/layout/Layout';
import LoginPage from './pages/LoginPage';
import PortalHomePage from './pages/PortalHomePage';
import ProductsPage from './pages/ProductsPage';
import InventoryPage from './pages/InventoryPage';
import MovementsPage from './pages/MovementsPage';
import UsersPage from './pages/UsersPage';
import RolesPage from './pages/RolesPage';

function PrivateRoute({ children }) {
  const { isAuthenticated, loading } = useAuth();
  if (loading) return <div className="flex items-center justify-center min-h-screen">Cargando...</div>;
  return isAuthenticated ? children : <Navigate to="/login" />;
}

function PortalRedirect() {
  const { hasPermission } = useAuth();
  const portal = modules.find((m) => m.id === 'portal');
  const first = portal.items.find((i) => hasPermission(i.permission)) || portal.items[0];
  return <Navigate to={first.to} replace />;
}

function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route
        path="/"
        element={
          <PrivateRoute>
            <Layout />
          </PrivateRoute>
        }
      >
        <Route index element={<PortalHomePage />} />
        <Route path="portal">
          <Route index element={<PortalRedirect />} />
          <Route path="usuarios" element={<UsersPage />} />
          <Route path="roles" element={<RolesPage />} />
        </Route>
        <Route path="inventario">
          <Route index element={<InventoryPage />} />
          <Route path="productos" element={<ProductsPage />} />
          <Route path="movimientos" element={<MovementsPage />} />
        </Route>
        <Route path="*" element={<Navigate to="/" replace />} />
      </Route>
    </Routes>
  );
}

export default App;
