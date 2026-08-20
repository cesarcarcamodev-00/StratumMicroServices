import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from 'react-query';
import { usersService, rolesService } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import { Plus, Edit2, Trash2, UserPlus } from 'lucide-react';
import toast from 'react-hot-toast';
import { getErrorMessage } from '../utils/errors';

const emptyForm = { username: '', email: '', password: '', isActive: true, roleIds: [] };

export default function UsersPage() {
  const { user: currentUser, hasPermission } = useAuth();
  const [showForm, setShowForm] = useState(false);
  const [editing, setEditing] = useState(null);
  const [formData, setFormData] = useState(emptyForm);
  const queryClient = useQueryClient();

  const canManage = hasPermission('identity.users.manage');

  const { data: usersData, isLoading } = useQuery('users', () => usersService.getAll({ page: 1, pageSize: 100 }));
  const { data: rolesData } = useQuery('roles', () => rolesService.getAll(), { enabled: canManage });

  const createMutation = useMutation((data) => usersService.create(data), {
    onSuccess: () => { queryClient.invalidateQueries('users'); toast.success('Usuario creado'); resetForm(); },
    onError: (err) => toast.error(getErrorMessage(err))
  });

  const updateMutation = useMutation(({ id, data }) => usersService.update(id, data), {
    onSuccess: () => { queryClient.invalidateQueries('users'); toast.success('Usuario actualizado'); resetForm(); },
    onError: (err) => toast.error(getErrorMessage(err))
  });

  const deleteMutation = useMutation((id) => usersService.delete(id), {
    onSuccess: () => { queryClient.invalidateQueries('users'); toast.success('Usuario eliminado'); },
    onError: (err) => toast.error(getErrorMessage(err))
  });

  const resetForm = () => { setFormData(emptyForm); setEditing(null); setShowForm(false); };

  const users = usersData?.data?.items || [];
  const roles = rolesData?.data || [];

  const handleSubmit = (e) => {
    e.preventDefault();
    const payload = {
      username: formData.username.trim(),
      email: formData.email.trim(),
      isActive: formData.isActive,
      roleIds: formData.roleIds,
      permissionIds: []
    };
    if (editing) {
      updateMutation.mutate({ id: editing.id, data: { ...payload, id: editing.id } });
    } else {
      createMutation.mutate({ ...payload, password: formData.password });
    }
  };

  const handleEdit = (u) => {
    setEditing(u);
    setFormData({ username: u.username, email: u.email, password: '', isActive: u.isActive, roleIds: u.roleIds || [] });
    setShowForm(true);
  };

  const toggleRole = (id) => {
    setFormData((prev) => ({
      ...prev,
      roleIds: prev.roleIds.includes(id) ? prev.roleIds.filter((r) => r !== id) : [...prev.roleIds, id]
    }));
  };

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <div>
          <h2 style={{ marginBottom: '0.25rem' }}>Usuarios</h2>
          <p style={{ color: 'var(--text-light)', fontSize: '0.85rem', margin: 0 }}>
            Usuarios del portal único. Los permisos se heredan de los roles y asignaciones directas.
          </p>
        </div>
        {canManage && (
          <button className="btn btn-primary" onClick={() => { setEditing(null); setFormData(emptyForm); setShowForm(true); }}>
            <UserPlus size={16} /> Nuevo Usuario
          </button>
        )}
      </div>

      {showForm && canManage && (
        <div className="card" style={{ marginBottom: '1rem' }}>
          <h4 style={{ marginBottom: '1rem' }}>{editing ? `Editar Usuario: ${editing.username}` : 'Nuevo Usuario'}</h4>
          <form onSubmit={handleSubmit}>
            <div className="grid grid-2">
              <div className="form-group">
                <label>Usuario *</label>
                <input value={formData.username} onChange={(e) => setFormData({ ...formData, username: e.target.value })} required minLength={3} />
              </div>
              <div className="form-group">
                <label>Email *</label>
                <input type="email" value={formData.email} onChange={(e) => setFormData({ ...formData, email: e.target.value })} required />
              </div>
              {!editing && (
                <div className="form-group">
                  <label>Contraseña *</label>
                  <input type="password" value={formData.password} onChange={(e) => setFormData({ ...formData, password: e.target.value })} required minLength={6} />
                </div>
              )}
              <div className="form-group" style={{ display: 'flex', alignItems: 'flex-end', paddingBottom: '0.3rem' }}>
                <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', cursor: 'pointer' }}>
                  <input type="checkbox" checked={formData.isActive} onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })} />
                  Activo
                </label>
              </div>
              <div className="form-group" style={{ gridColumn: '1 / -1' }}>
                <label>Roles</label>
                <div style={{ display: 'flex', flexWrap: 'wrap', gap: '0.5rem' }}>
                  {roles.map((r) => (
                    <label key={r.id} className="badge" style={{ cursor: 'pointer', padding: '0.5rem 0.75rem', border: formData.roleIds.includes(r.id) ? '2px solid var(--primary)' : '2px solid var(--border)', background: formData.roleIds.includes(r.id) ? 'var(--primary-soft)' : 'transparent' }}>
                      <input type="checkbox" checked={formData.roleIds.includes(r.id)} onChange={() => toggleRole(r.id)} style={{ marginRight: '0.4rem' }} />
                      {r.name}
                    </label>
                  ))}
                  {roles.length === 0 && <span style={{ color: 'var(--text-light)' }}>No hay roles disponibles</span>}
                </div>
              </div>
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
                  <th>Usuario</th>
                  <th>Email</th>
                  <th>Roles</th>
                  <th>Estado</th>
                  <th>Creado</th>
                  {canManage && <th>Acciones</th>}
                </tr>
              </thead>
              <tbody>
                {users.map((u) => (
                  <tr key={u.id}>
                    <td>
                      <strong>{u.username}</strong>
                      {u.id === currentUser?.userId && <span className="badge badge-info" style={{ marginLeft: '0.4rem' }}>tú</span>}
                    </td>
                    <td>{u.email}</td>
                    <td>{(u.roles || []).map((r) => <span key={r} className="badge" style={{ marginRight: '0.3rem' }}>{r}</span>)}</td>
                    <td><span className={`badge ${u.isActive ? 'badge-success' : 'badge-danger'}`}>{u.isActive ? 'Activo' : 'Inactivo'}</span></td>
                    <td>{new Date(u.createdAt).toLocaleDateString()}</td>
                    {canManage && (
                      <td>
                        <button className="btn btn-ghost btn-sm" onClick={() => handleEdit(u)}><Edit2 size={14} /></button>
                        {u.id !== currentUser?.userId && (
                          <button className="btn btn-ghost btn-sm" style={{ color: 'var(--danger)' }} onClick={() => { if (confirm(`¿Eliminar al usuario ${u.username}?`)) deleteMutation.mutate(u.id); }}><Trash2 size={14} /></button>
                        )}
                      </td>
                    )}
                  </tr>
                ))}
                {users.length === 0 && <tr><td colSpan={6} style={{ textAlign: 'center', padding: '2rem' }}>No hay usuarios registrados</td></tr>}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}
