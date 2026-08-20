import { Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { getAvailableModules, modules } from '../modules';
import { ArrowRight } from 'lucide-react';

export default function PortalHomePage() {
  const { user, hasPermission } = useAuth();
  const availableModules = getAvailableModules(modules, user);

  const canSee = (item) => {
    if (item.permissions) return item.permissions.some((p) => hasPermission(p));
    if (item.permission) return hasPermission(item.permission);
    if (item.adminOnly) return user?.roles?.includes('Admin') || false;
    return true;
  };

  return (
    <div>
      <div style={{ marginBottom: '1.75rem' }}>
        <h2 style={{ marginBottom: '0.25rem', fontWeight: 800, letterSpacing: '-0.02em' }}>Hola, {user?.username}</h2>
        <p style={{ color: 'var(--text-light)', fontSize: '0.95rem' }}>
          Selecciona un módulo para comenzar.
        </p>
      </div>
      <div className="grid grid-2">
        {availableModules.map((m) => (
          <Link key={m.id} to={m.path} className="card module-card">
            <div className="module-card-head">
              <span className="module-card-icon" style={{ background: `${m.color}1a`, color: m.color }}>
                <m.icon size={26} />
              </span>
              <ArrowRight size={20} className="module-card-arrow" />
            </div>
            <h3 style={{ marginTop: '1.25rem', marginBottom: '0.25rem', fontWeight: 700, letterSpacing: '-0.01em' }}>{m.name}</h3>
            <p className="module-card-tagline">{m.tagline}</p>
            <div className="module-card-links">
              {m.items.filter(canSee).map((item) => (
                <span key={item.to}>{item.label}</span>
              ))}
            </div>
          </Link>
        ))}
      </div>
      <style>{`
        .module-card { text-decoration: none !important; color: var(--text); transition: all 0.2s ease; border-radius: 18px; }
        .module-card:hover { transform: translateY(-3px); box-shadow: var(--shadow-md); border-color: #d8dde6; }
        .module-card-head { display: flex; align-items: flex-start; justify-content: space-between; }
        .module-card-icon {
          display: inline-flex; align-items: center; justify-content: center;
          width: 48px; height: 48px; border-radius: 13px;
        }
        .module-card-arrow { color: var(--text-faint); transition: all 0.2s; }
        .module-card:hover .module-card-arrow { color: var(--primary); transform: translateX(3px); }
        .module-card-tagline { font-size: 0.875rem; color: var(--text-light); }
        .module-card-links { display: flex; flex-wrap: wrap; gap: 0.4rem; margin-top: 1rem; }
        .module-card-links span {
          padding: 0.26rem 0.62rem; border-radius: 9999px;
          background: var(--primary-soft); color: var(--primary-dark);
          font-size: 0.75rem; font-weight: 600;
        }
      `}</style>
    </div>
  );
}
