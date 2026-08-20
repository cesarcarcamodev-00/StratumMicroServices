import { Outlet, NavLink, Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { LogOut, X, ChevronDown, LayoutGrid, Home, PanelLeftClose, PanelLeftOpen } from 'lucide-react';
import { useState } from 'react';
import { getModuleForPath, getAvailableModules, modules } from '../../modules';
import { APP_NAME } from '../../config';

export default function Layout() {
  const { logout, user, hasPermission } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const [switcherOpen, setSwitcherOpen] = useState(false);
  const [collapsed, setCollapsed] = useState(() => localStorage.getItem('stratum.sidebar.collapsed') === '1');

  const toggleCollapsed = () => {
    setCollapsed((c) => {
      const next = !c;
      localStorage.setItem('stratum.sidebar.collapsed', next ? '1' : '0');
      return next;
    });
  };

  const handleMenuToggle = () => {
    if (window.innerWidth <= 1024) {
      setSidebarOpen(true);
    } else {
      toggleCollapsed();
    }
  };

  const availableModules = getAvailableModules(modules, user);
  const activeModule = getModuleForPath(modules, location.pathname);

  const canSee = (item) => {
    if (item.permissions) return item.permissions.some((p) => hasPermission(p));
    if (item.permission) return hasPermission(item.permission);
    if (item.adminOnly) return user?.roles?.includes('Admin') || false;
    return true;
  };

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  const goToModule = (m) => {
    setSwitcherOpen(false);
    setSidebarOpen(false);
    navigate(m.path);
  };

  const goHome = () => {
    setSwitcherOpen(false);
    setSidebarOpen(false);
    navigate('/');
  };

  const visibleItems = activeModule ? activeModule.items.filter(canSee) : [];
  const themeClass = activeModule ? `theme-${activeModule.id}` : 'theme-default';

  return (
    <div className={`layout ${themeClass}`}>
      <aside className={`sidebar ${collapsed ? 'collapsed' : ''} ${sidebarOpen ? 'open' : ''}`}>
        <div className="sidebar-header">
          <Link to="/" className="sidebar-brand" onClick={() => setSidebarOpen(false)} title={APP_NAME}>
            <span className="brand-logo"><LayoutGrid size={20} /></span>
            <span className="brand-name">{APP_NAME}</span>
          </Link>
          <button className="btn btn-ghost btn-sm mobile-close" onClick={() => setSidebarOpen(false)}>
            <X size={20} />
          </button>
        </div>
        <nav className="sidebar-nav">
          {activeModule ? (
            <>
              <NavLink to="/" end className="nav-item nav-home" onClick={() => setSidebarOpen(false)} title="Inicio">
                <Home size={20} />
                <span>Inicio</span>
              </NavLink>
              <div className="sidebar-section">
                <activeModule.icon size={14} color={activeModule.color} />
                <span style={{ color: activeModule.color }}>{activeModule.name}</span>
              </div>
              {visibleItems.map((item) => (
                <NavLink
                  key={item.to}
                  to={item.to}
                  end={item.end}
                  className={({ isActive }) => `nav-item ${isActive ? 'active' : ''}`}
                  onClick={() => setSidebarOpen(false)}
                  title={item.label}
                >
                  <item.icon size={20} />
                  <span>{item.label}</span>
                </NavLink>
              ))}
            </>
          ) : (
            <>
              <div className="sidebar-section">
                <LayoutGrid size={14} />
                <span>Módulos</span>
              </div>
              {availableModules.map((m) => (
                <button key={m.id} className="nav-item nav-module" onClick={() => goToModule(m)} title={m.name}>
                  <m.icon size={20} color={m.color} />
                  <span>{m.name}</span>
                </button>
              ))}
            </>
          )}
        </nav>
        <div className="sidebar-footer">
          <div className="user-info">
            <span className="user-avatar">{user?.username?.charAt(0)?.toUpperCase() || 'A'}</span>
            <div className="user-text">
              <span className="user-name">{user?.username}</span>
              <span className="user-role">{(user?.roles || []).join(', ') || 'Usuario'}</span>
            </div>
          </div>
          <button className="logout-btn" onClick={handleLogout} title="Cerrar Sesión">
            <LogOut size={16} /> <span>Cerrar Sesión</span>
          </button>
        </div>
      </aside>

      <div className="main-area">
        <header className="topbar">
          <button className="btn btn-ghost btn-icon menu-btn" onClick={handleMenuToggle}>
            {collapsed ? <PanelLeftOpen size={20} /> : <PanelLeftClose size={20} />}
          </button>
          <div className="module-switcher">
            <button className="module-switcher-btn" onClick={() => setSwitcherOpen((o) => !o)}>
              {activeModule ? (
                <>
                  <activeModule.icon size={18} color={activeModule.color} />
                  <span>{activeModule.name}</span>
                </>
              ) : (
                <>
                  <LayoutGrid size={18} color="var(--text-light)" />
                  <span>Inicio</span>
                </>
              )}
              <ChevronDown size={16} />
            </button>
            {switcherOpen && (
              <>
                <div className="module-menu">
                  <div className="module-menu-title">Módulos</div>
                  <button
                    className={`module-item ${!activeModule ? 'active' : ''}`}
                    onClick={goHome}
                  >
                    <span className="module-item-icon" style={{ background: '#f1f5f9', color: 'var(--text-light)' }}>
                      <Home size={20} />
                    </span>
                    <span className="module-item-body">
                      <span className="module-item-name">Inicio</span>
                      <span className="module-item-tagline">Seleccionar módulo</span>
                    </span>
                  </button>
                  {availableModules.map((m) => (
                    <button
                      key={m.id}
                      className={`module-item ${activeModule?.id === m.id ? 'active' : ''}`}
                      onClick={() => goToModule(m)}
                    >
                      <span className="module-item-icon" style={{ background: `${m.color}1a`, color: m.color }}>
                        <m.icon size={20} />
                      </span>
                      <span className="module-item-body">
                        <span className="module-item-name">{m.name}</span>
                        <span className="module-item-tagline">{m.tagline}</span>
                      </span>
                    </button>
                  ))}
                </div>
                <div className="switcher-overlay" onClick={() => setSwitcherOpen(false)} />
              </>
            )}
          </div>
        </header>
        <main className="content">
          <Outlet />
        </main>
      </div>

      {sidebarOpen && <div className="overlay" onClick={() => setSidebarOpen(false)} />}

      <style>{`
        .layout { display: flex; min-height: 100vh; background: var(--background); }
        .layout.theme-portal { --primary: #6d28d9; --primary-dark: #5b21b6; --primary-soft: #f5f3ff; --background: #f8f7fc; }
        .layout.theme-inventory { --primary: #059669; --primary-dark: #047857; --primary-soft: #ecfdf5; --background: #f5faf8; }

        .sidebar {
          width: 264px; min-width: 264px; background: #0d1420; color: white;
          display: flex; flex-direction: column; position: sticky;
          top: 0; left: 0; height: 100vh; z-index: 100;
          transition: width 0.22s ease, min-width 0.22s ease, transform 0.3s;
          border-right: 1px solid rgba(255,255,255,0.04);
        }
        .sidebar.collapsed { width: 78px; min-width: 78px; }

        .sidebar-header {
          padding: 1.1rem 1.1rem; border-bottom: 1px solid rgba(255,255,255,0.06);
          display: flex; align-items: center; justify-content: space-between; gap: 0.5rem;
        }
        .sidebar-brand { display: flex; align-items: center; gap: 0.65rem; text-decoration: none !important; color: white; min-width: 0; }
        .brand-logo {
          display: flex; align-items: center; justify-content: center;
          width: 38px; height: 38px; border-radius: 11px; flex-shrink: 0;
          background: linear-gradient(135deg, var(--primary), var(--primary-dark));
          color: white; box-shadow: 0 4px 12px rgba(0,0,0,0.25);
        }
        .brand-name { font-size: 1.05rem; font-weight: 700; letter-spacing: -0.01em; white-space: nowrap; }
        .mobile-close { display: none; color: #cbd5e1; border-color: #334155; }

        .sidebar-nav { flex: 1; padding: 1rem 0.85rem; overflow-y: auto; display: flex; flex-direction: column; gap: 2px; }
        .sidebar-section {
          display: flex; align-items: center; gap: 0.55rem; justify-content: flex-start;
          padding: 0.45rem 0.7rem 0.45rem; font-size: 0.66rem; font-weight: 700;
          color: #64748b; text-transform: uppercase; letter-spacing: 0.8px; white-space: nowrap;
        }
        .nav-item {
          display: flex; align-items: center; gap: 0.75rem;
          padding: 0.62rem 0.7rem; border-radius: 10px;
          color: #8b96a5; font-size: 0.875rem; font-weight: 500;
          transition: all 0.16s; text-decoration: none !important; white-space: nowrap;
        }
        .nav-item:hover { background: rgba(255,255,255,0.06); color: #e2e8f0; }
        .nav-item.active { background: var(--primary); color: white; box-shadow: 0 2px 8px rgba(0,0,0,0.2); }
        .nav-home { margin-bottom: 0.5rem; border-bottom: 1px solid rgba(255,255,255,0.06); border-radius: 0; padding-bottom: 0.7rem; }
        .nav-module { background: transparent; border: none; cursor: pointer; width: 100%; text-align: left; }

        .sidebar-footer { padding: 1rem 1.1rem; border-top: 1px solid rgba(255,255,255,0.06); }
        .user-info { display: flex; align-items: center; gap: 0.65rem; }
        .user-avatar {
          display: flex; align-items: center; justify-content: center;
          width: 34px; height: 34px; border-radius: 9999px; flex-shrink: 0;
          background: linear-gradient(135deg, var(--primary), var(--primary-dark));
          color: white; font-weight: 700; font-size: 0.9rem;
        }
        .user-text { min-width: 0; }
        .user-name { display: block; font-weight: 600; font-size: 0.8125rem; color: #e2e8f0; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
        .user-role { display: block; font-size: 0.7rem; color: #64748b; text-transform: capitalize; }
        .logout-btn {
          display: flex; align-items: center; gap: 0.55rem;
          width: 100%; margin-top: 0.85rem; padding: 0.58rem 0.7rem;
          background: transparent; border: 1px solid rgba(148, 163, 184, 0.18);
          border-radius: 10px; color: #94a3b8; font-size: 0.8125rem; font-weight: 600;
          cursor: pointer; transition: all 0.16s; white-space: nowrap;
        }
        .logout-btn:hover { background: rgba(220, 38, 38, 0.12); border-color: rgba(220, 38, 38, 0.4); color: #fecaca; }

        .sidebar.collapsed .brand-name,
        .sidebar.collapsed .sidebar-section span,
        .sidebar.collapsed .nav-item span,
        .sidebar.collapsed .user-text,
        .sidebar.collapsed .logout-btn span { display: none; }
        .sidebar.collapsed .sidebar-brand { justify-content: center; }
        .sidebar.collapsed .sidebar-section { justify-content: center; padding: 0.45rem 0; }
        .sidebar.collapsed .nav-item { justify-content: center; padding: 0.62rem 0; }
        .sidebar.collapsed .user-info { justify-content: center; }
        .sidebar.collapsed .logout-btn { justify-content: center; padding: 0.58rem 0; }
        .sidebar.collapsed .sidebar-header { justify-content: center; padding: 1.1rem 0.5rem; }

        .main-area { flex: 1; min-width: 0; display: flex; flex-direction: column; }
        .topbar {
          display: flex; align-items: center; gap: 1rem;
          padding: 0.85rem 2rem; background: rgba(255,255,255,0.82);
          backdrop-filter: blur(10px); -webkit-backdrop-filter: blur(10px);
          border-bottom: 1px solid var(--border);
          position: sticky; top: 0; z-index: 50;
        }
        .menu-btn { display: flex; color: var(--text-light); }
        .menu-btn:hover { color: var(--text); }

        .module-switcher { position: relative; }
        .module-switcher-btn {
          display: flex; align-items: center; gap: 0.5rem;
          padding: 0.5rem 0.85rem; border-radius: 10px;
          background: var(--surface); border: 1px solid var(--border);
          font-weight: 600; font-size: 0.9rem; color: var(--text); cursor: pointer;
          transition: all 0.18s;
        }
        .module-switcher-btn:hover { background: var(--bg-secondary); border-color: #d5dae3; }
        .module-menu {
          position: absolute; top: calc(100% + 8px); left: 0; z-index: 60;
          min-width: 300px; padding: 0.5rem; border-radius: 14px;
          background: var(--surface); border: 1px solid var(--border);
          box-shadow: var(--shadow-lg);
        }
        .module-menu-title {
          padding: 0.4rem 0.6rem 0.5rem; font-size: 0.66rem; font-weight: 700;
          color: var(--text-light); text-transform: uppercase; letter-spacing: 0.8px;
        }
        .module-item {
          display: flex; align-items: center; gap: 0.75rem; width: 100%;
          padding: 0.6rem; border: none; background: transparent; border-radius: 10px;
          cursor: pointer; text-align: left; color: var(--text);
        }
        .module-item:hover { background: var(--bg-secondary); }
        .module-item.active { background: var(--primary-soft); }
        .module-item-icon {
          display: flex; align-items: center; justify-content: center;
          width: 38px; height: 38px; border-radius: 10px; flex-shrink: 0;
        }
        .module-item-name { display: block; font-weight: 600; font-size: 0.875rem; }
        .module-item-tagline { display: block; font-size: 0.75rem; color: var(--text-light); }
        .switcher-overlay { position: fixed; inset: 0; z-index: 55; }

        .content { padding: 2rem; }

        .overlay { display: none; }

        @media (max-width: 1024px) {
          .sidebar { position: fixed; transform: translateX(-100%); width: 264px; min-width: 264px; }
          .sidebar.collapsed { width: 264px; min-width: 264px; }
          .sidebar.open { transform: translateX(0); }
          .sidebar.collapsed .brand-name,
          .sidebar.collapsed .sidebar-section span,
          .sidebar.collapsed .nav-item span,
          .sidebar.collapsed .user-text,
          .sidebar.collapsed .logout-btn span { display: block; }
          .sidebar.collapsed .sidebar-brand { justify-content: flex-start; }
          .sidebar.collapsed .sidebar-section { justify-content: flex-start; padding: 0.45rem 0.7rem 0.45rem; }
          .sidebar.collapsed .nav-item { justify-content: flex-start; padding: 0.62rem 0.7rem; }
          .sidebar.collapsed .user-info { justify-content: flex-start; }
          .sidebar.collapsed .logout-btn { justify-content: flex-start; padding: 0.58rem 0.7rem; }
          .sidebar.collapsed .sidebar-header { justify-content: space-between; padding: 1.1rem; }
          .mobile-close { display: flex; }
          .main-area { margin-left: 0; }
          .content { padding: 1.25rem; }
          .overlay { display: block; position: fixed; inset: 0; background: rgba(13,20,32,0.55); z-index: 99; backdrop-filter: blur(2px); }
        }
      `}</style>
    </div>
  );
}
