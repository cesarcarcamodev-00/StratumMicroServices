import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from 'react-query';
import { rolesService, permissionsService } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import { Plus, Edit2, Trash2, ShieldPlus } from 'lucide-react';
import toast from 'react-hot-toast';
import { getErrorMessage } from '../utils/errors';

const emptyForm = { name: '', description: '', permissionIds: [] };

const MODULE_LABELS = { Identity: 'Identidad', Inventario: 'Inventario' };

export default function RolesPage() {
  const { hasPermission } = useAuth();
  const [showForm, setShowForm] = useState(false);
  const [editing, setEditing] = useState(null);
  const [formData, setFormData] = useState(emptyForm);
  const queryClient = useQueryClient();

  const canManage = hasPermission('identity.users.manage');

  const { data: roles, isLoading } = useQuery('roles', () => rolesService.getAll());
  const { data: permissions } = useQuery('permissions', () => permissionsService.getAll(), { enabled: canManage });

  const createMutation = useMutation((data) => rolesService.create(data), {
    onSuccess: () => { queryClient.invalidateQueries('roles'); toast.success('Rol creado'); resetForm(); },
    onError: (err) => toast.error(getErrorMessage(err))
  });

  const updateMutation = useMutation(({ id, data }) => rolesService.update(id, data), {
    onSuccess: () => { queryClient.invalidateQueries('roles'); toast.success('Rol actualizado'); resetForm(); },
    onError: (err) => toast.error(getErrorMessage(err))
  });

  const deleteMutation = useMutation((id) => rolesService.delete(id), {
    onSuccess: () => { queryClient.invalidateQueries('roles'); toast.success('Rol eliminado'); },
    onError: (err) => toast.error(getErrorMessage(err))
  });

  const rolesList = roles?.data || [];
  const permissionsList = permissions?.data || [];

  const resetForm = () => { setFormData(emptyForm); setEditing(null); setShowForm(false); };

  const permissionsByModule = permissionsList.reduce((acc, p) => {
    if (!acc[p.module]) acc[p.module] = [];
    acc[p.module].push(p);
    return acc;
  }, {});

  const handleSubmit = (e) => {
    e.preventDefault();
    const payload = { name: formData.name.trim(), description: formData.description.trim() || null, permissionIds: formData.permissionIds };
    if (editing) updateMutation.mutate({ id: editing.id, data: { ...payload, id: editing.id } });
    else createMutation.mutate(payload);
  };

  const handleEdit = (r) => {
    setEditing(r);
    setFormData({ name: r.name, description: r.description || '', permissionIds: r.permissionIds || [] });
    setShowForm(true);
  };

  const togglePermission = (id) => {
    setFormData((prev) => ({
      ...prev,
      permissionIds: prev.permissionIds.includes(id) ? prev.permissionIds.filter((p) => p !== id) : [...prev.permissionIds, id]
    }));
  };

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <div>
          <h2 style={{ marginBottom: '0.25rem' }}>Roles y Permisos</h2>
          <p style={{ color: 'var(--text-light)', fontSize: '0.85rem', margin: 0 }}>
            Los permisos controlan qué módulos y acciones puede ver cada usuario.
          </p>
        </div>
        {canManage && (
          <button className="btn btn-primary" onClick={() => { setEditing(null); setFormData(emptyForm); setShowForm(true); }}>
            <ShieldPlus size={16} /> Nuevo Rol
          </button>
        )}
      </div>

      {showForm && canManage && (
        <div className="card" style={{ marginBottom: '1rem' }}>
          <h4 style={{ marginBottom: '1rem' }}>{editing ? `Editar Rol: ${editing.name}` : 'Nuevo Rol'}</h4>
          <form onSubmit={handleSubmit}>
            <div className="grid grid-2">
              <div className="form-group">
                <label>Nombre *</label>
                <input value={formData.name} onChange={(e) => setFormData({ ...formData, name: e.target.value })} required maxLength={100} />
              </div>
              <div className="form-group">
                <label>Descripción</label>
                <input value={formData.description} onChange={(e) => setFormData({ ...formData, description: e.target.value })} />
              </div>
            </div>
            <div style={{ marginTop: '0.75rem' }}>
              <label style={{ marginBottom: '0.5rem', display: 'block' }}>Permisos</label>
              {Object.keys(permissionsByModule).length === 0 && <p style={{ color: 'var(--text-light)' }}>No hay permisos disponibles</p>}
              {Object.keys(permissionsByModule).map((module) => (
                <div key={module} className="card" style={{ marginBottom: '0.75rem', padding: '0.85rem' }}>
                  <strong style={{ fontSize: '0.8rem', textTransform: 'uppercase', color: 'var(--text-light)' }}>{MODULE_LABELS[module] || module}</strong>
                  <div style={{ display: 'flex', flexWrap: 'wrap', gap: '0.5rem', marginTop: '0.5rem' }}>
                    {permissionsByModule[module].map((p) => (
                      <label key={p.id} className="badge" style={{ cursor: 'pointer', padding: '0.4rem 0.65rem', border: formData.permissionIds.includes(p.id) ? '2px solid var(--primary)' : '2px solid var(--border)', background: formData.permissionIds.includes(p.id) ? 'var(--primary-soft)' : 'transparent' }}>
                        <input type="checkbox" checked={formData.permissionIds.includes(p.id)} onChange={() => togglePermission(p.id)} style={{ marginRight: '0.35rem' }} />
                        {p.description || p.name}
                      </label>
                    ))}
                  </div>
                </div>
              ))}
            </div>
            <div style={{ display: 'flex', gap: '0.5rem', marginTop: '0.75rem' }}>
              <button type="submit" className="btn btn-primary" disabled={createMutation.isLoading || updateMutation.isLoading}>{editing ? 'Actualizar' : 'Crear'}</button>
              <button type="button" className="btn btn-ghost" onClick={resetForm}>Cancelar</button>
            </div>
          </form>
        </div>
      )}

      <div className="card">
        {isLoading ? <p>Cargando...</p> : (
          <div className="table-container">
            <table>
              <thead>
                <tr>
                  <th>Rol</th>
                  <th>Descripción</th>
                  <th>Usuarios</th>
                  <th>Permisos</th>
                  <th>Estado</th>
                  {canManage && <th>Acciones</th>}
                </tr>
              </thead>
              <tbody>
                {rolesList.map((r) => (
                  <tr key={r.id}>
                    <td><strong>{r.name}</strong></td>
                    <td>{r.description || '—'}</td>
                    <td>{r.userCount}</td>
                    <td>{r.permissions.length}</td>
                    <td><span className={`badge ${r.isActive ? 'badge-success' : 'badge-danger'}`}>{r.isActive ? 'Activo' : 'Inactivo'}</span></td>
                    {canManage && (
                      <td>
                        <button className="btn btn-ghost btn-sm" onClick={() => handleEdit(r)}><Edit2 size={14} /></button>
                        {r.name !== 'Admin' && (
                          <button className="btn btn-ghost btn-sm" style={{ color: 'var(--danger)' }} onClick={() => { if (confirm(`¿Eliminar el rol ${r.name}?`)) deleteMutation.mutate(r.id); }}><Trash2 size={14} /></button>
                        )}
                      </td>
                    )}
                  </tr>
                ))}
                {rolesList.length === 0 && <tr><td colSpan={6} style={{ textAlign: 'center', padding: '2rem' }}>No hay roles registrados</td></tr>}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}
