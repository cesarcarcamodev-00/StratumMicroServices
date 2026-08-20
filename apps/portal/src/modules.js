import {
  LayoutDashboard, Package, History,
  Users, ShieldCheck, Boxes, Settings
} from 'lucide-react';

export const modules = [
  {
    id: 'portal',
    name: 'Portal',
    tagline: 'Usuarios, roles y permisos',
    icon: ShieldCheck,
    color: '#7c3aed',
    path: '/portal',
    available: (user) =>
      user?.roles?.includes('Admin') ||
      (user?.permissions || []).some((p) => p.startsWith('identity.')),
    items: [
      { to: '/portal/usuarios', label: 'Usuarios', icon: Users, permission: 'identity.users.view' },
      { to: '/portal/roles', label: 'Roles y Permisos', icon: Settings, permission: 'identity.roles.view' },
    ],
  },
  {
    id: 'inventory',
    name: 'Inventario',
    tagline: 'Productos y stock',
    icon: Boxes,
    color: '#059669',
    path: '/inventario',
    available: (user) =>
      user?.roles?.includes('Admin') ||
      (user?.permissions || []).some((p) => p.startsWith('inventory.')),
    items: [
      { to: '/inventario', label: 'Panel Principal', icon: LayoutDashboard, end: true, permission: 'inventory.view' },
      { to: '/inventario/productos', label: 'Productos', icon: Package, permission: 'inventory.view' },
      { to: '/inventario/movimientos', label: 'Movimientos', icon: History, permission: 'inventory.movements.view' },
    ],
  },
];

export const getModuleForPath = (allModules, pathname) =>
  allModules.find((m) => pathname === m.path || pathname.startsWith(`${m.path}/`)) || null;

export const getAvailableModules = (allModules, user) => allModules.filter((m) => m.available(user));
