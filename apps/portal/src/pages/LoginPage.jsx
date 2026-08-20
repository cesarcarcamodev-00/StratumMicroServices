import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { Layers } from 'lucide-react';
import { APP_NAME } from '../config';

export default function LoginPage() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      await login(username, password);
      navigate('/');
    } catch (err) {
      setError(err.response?.data?.errors?.[0] || 'Credenciales inválidas');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-page">
      <div className="login-card">
        <div className="login-brand">
          <span className="login-logo"><Layers size={30} /></span>
        </div>
        <h1 className="login-title">{APP_NAME}</h1>
        <p className="login-subtitle">Inicia sesión en tu cuenta</p>
        <form onSubmit={handleSubmit}>
          {error && <div className="badge badge-danger login-error">{error}</div>}
          <div className="form-group">
            <label>Usuario</label>
            <input type="text" value={username} onChange={(e) => setUsername(e.target.value)} placeholder="admin" required />
          </div>
          <div className="form-group">
            <label>Contraseña</label>
            <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="••••••••" required />
          </div>
          <button type="submit" className="btn btn-primary btn-block" disabled={loading}>
            {loading ? 'Iniciando sesión...' : 'Iniciar Sesión'}
          </button>
        </form>
        <p className="login-hint">Por defecto: admin / Admin123!</p>
      </div>
      <style>{`
        .login-page {
          min-height: 100vh; display: flex; align-items: center; justify-content: center; padding: 1rem;
          position: relative; overflow: hidden;
          background:
            radial-gradient(1100px 560px at 12% -8%, #312e81 0%, transparent 50%),
            radial-gradient(900px 520px at 112% 112%, #4f46e5 0%, transparent 45%),
            #0f172a;
        }
        .login-card {
          width: 400px; max-width: 90vw;
          background: rgba(255,255,255,0.98); border: 1px solid var(--border);
          border-radius: 22px; box-shadow: var(--shadow-lg);
          padding: 2.5rem 2rem;
        }
        .login-brand { text-align: center; }
        .login-logo {
          display: inline-flex; align-items: center; justify-content: center;
          width: 64px; height: 64px; border-radius: 18px;
          background: linear-gradient(135deg, var(--primary), var(--primary-dark));
          color: white; box-shadow: 0 10px 24px rgba(79,70,229,0.35);
        }
        .login-title { margin-top: 1.25rem; text-align: center; font-size: 1.6rem; font-weight: 800; letter-spacing: -0.02em; }
        .login-subtitle { text-align: center; color: var(--text-light); font-size: 0.875rem; margin-bottom: 1.75rem; }
        .login-error { width: 100%; justify-content: center; padding: 0.6rem; margin-bottom: 1rem; }
        .btn-block { width: 100%; justify-content: center; padding: 0.7rem; }
        .login-hint { text-align: center; margin-top: 1.25rem; font-size: 0.75rem; color: var(--text-light); }
      `}</style>
    </div>
  );
}
