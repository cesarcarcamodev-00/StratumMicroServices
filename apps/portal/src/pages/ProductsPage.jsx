import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from 'react-query';
import { productsService, unitsService } from '../services/api';
import { Plus, Edit2, Trash2, Search, RotateCcw } from 'lucide-react';
import toast from 'react-hot-toast';
import { getErrorMessage } from '../utils/errors';
import { formatQty } from '../utils/format';
import FieldHelp from '../components/ui/FieldHelp';

export default function ProductsPage() {
  const [search, setSearch] = useState('');
  const [isActiveFilter, setIsActiveFilter] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [editingProduct, setEditingProduct] = useState(null);
  const [formData, setFormData] = useState({ name: '', sku: '', unitId: '', reorderLevel: 10, description: '' });
  const queryClient = useQueryClient();

  const { data: productsData, isLoading } = useQuery(['products', search, isActiveFilter], () =>
    productsService.getAll({ search, pageSize: 100, isActive: isActiveFilter })
  );
  const { data: unitsData } = useQuery('units-list', () => unitsService.getAll());
  const units = unitsData?.data || [];

  const createMutation = useMutation((data) => productsService.create(data), {
    onSuccess: () => {
      queryClient.invalidateQueries('products');
      toast.success('Producto creado correctamente');
      resetForm();
    },
    onError: (err) => toast.error(getErrorMessage(err))
  });

  const updateMutation = useMutation(({ id, data }) => productsService.update(id, data), {
    onSuccess: () => {
      queryClient.invalidateQueries('products');
      toast.success('Producto actualizado correctamente');
      resetForm();
    },
    onError: (err) => toast.error(getErrorMessage(err))
  });

  const deleteMutation = useMutation((id) => productsService.delete(id), {
    onSuccess: () => {
      queryClient.invalidateQueries('products');
      toast.success('Producto desactivado');
    },
    onError: (err) => toast.error(getErrorMessage(err))
  });

  const resetForm = () => {
    setFormData({ name: '', sku: '', unitId: '', reorderLevel: 10, description: '', isActive: true });
    setShowForm(false);
    setEditingProduct(null);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (editingProduct) {
      updateMutation.mutate({ id: editingProduct.id, data: { ...formData, id: editingProduct.id, isActive: formData.isActive ?? true } });
    } else {
      createMutation.mutate(formData);
    }
  };

  const handleEdit = async (product) => {
    setEditingProduct(product);
    setFormData({
      name: product.name,
      sku: product.sku,
      unitId: product.unitId,
      reorderLevel: product.reorderLevel,
      description: product.description || '',
      isActive: product.isActive,
    });
    setShowForm(true);
  };

  const products = productsData?.data?.items || [];

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <h2>Productos</h2>
        <button className="btn btn-primary" onClick={() => { setEditingProduct(null); resetForm(); setShowForm(true); }}>
          <Plus size={16} /> Agregar Producto
        </button>
      </div>

      <div className="card" style={{ marginBottom: '1rem' }}>
        <div style={{ display: 'flex', gap: '0.75rem', alignItems: 'center' }}>
          <div className="form-group" style={{ margin: 0, position: 'relative', flex: 1 }}>
            <Search size={16} style={{ position: 'absolute', left: '0.75rem', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-light)' }} />
            <input
              style={{ paddingLeft: '2.5rem' }}
              placeholder="Buscar productos por nombre, SKU..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
          </div>
          <select
            style={{ width: 'auto' }}
            value={isActiveFilter === undefined ? '' : isActiveFilter ? 'true' : 'false'}
            onChange={(e) => {
              const v = e.target.value;
              setIsActiveFilter(v === '' ? undefined : v === 'true');
            }}
          >
            <option value="true">Activos</option>
            <option value="false">Inactivos</option>
            <option value="">Todos</option>
          </select>
        </div>
      </div>

      {showForm && (
        <div className="card" style={{ marginBottom: '1rem' }}>
          <h3 style={{ marginBottom: '1rem' }}>{editingProduct ? 'Editar Producto' : 'Nuevo Producto'}</h3>
          <form onSubmit={handleSubmit}>
            <div className="grid grid-2">
              <div className="form-group">
                <label>Nombre * <FieldHelp text="Nombre del producto tal como lo verás en las listas." /></label>
                <input value={formData.name} onChange={(e) => setFormData({ ...formData, name: e.target.value })} required />
              </div>
              <div className="form-group">
                <label>SKU * <FieldHelp text="Código único interno para identificar el producto (p. ej. para etiquetas)." /></label>
                <input value={formData.sku} onChange={(e) => setFormData({ ...formData, sku: e.target.value })} required />
              </div>
              <div className="form-group">
                <label>Unidad de Medida * <FieldHelp text="Unidad en la que se mide el stock del producto (kg, lb, ud, pz, etc.)." /></label>
                <select value={formData.unitId} onChange={(e) => setFormData({ ...formData, unitId: e.target.value })} required>
                  <option value="">Seleccionar unidad</option>
                  {units.map((u) => <option key={u.id} value={u.id}>{u.name} ({u.symbol})</option>)}
                </select>
              </div>
              <div className="form-group">
                <label>Stock Mínimo (Alerta) <FieldHelp text="Umbral de aviso: si el stock llega a este valor o menos, el producto se marca como Stock Bajo en el panel de Inventario." /></label>
                <input type="number" step="0.001" value={formData.reorderLevel} onChange={(e) => setFormData({ ...formData, reorderLevel: parseFloat(e.target.value) })} />
              </div>
            </div>
            <div className="form-group">
              <label>Descripción <FieldHelp text="Detalle opcional visible junto al nombre del producto." /></label>
              <textarea value={formData.description} onChange={(e) => setFormData({ ...formData, description: e.target.value })} rows={2} />
            </div>

            <div style={{ display: 'flex', gap: '0.5rem', marginTop: '1rem' }}>
              <button type="submit" className="btn btn-primary">
                {editingProduct ? 'Actualizar' : 'Crear'}
              </button>
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
                  <th>Nombre</th>
                  <th>SKU</th>
                  <th>Unidad</th>
                  <th>Stock</th>
                  <th>Estado</th>
                  <th>Acciones</th>
                </tr>
              </thead>
              <tbody>
                {products.map((p) => (
                  <tr key={p.id}>
                    <td><strong>{p.name}</strong>{p.description ? <div style={{ fontSize: '0.75rem', color: 'var(--text-light)' }}>{p.description}</div> : null}</td>
                    <td>{p.sku}</td>
                    <td>{p.unitSymbol || '—'}</td>
                    <td>
                      <span className={`badge ${p.currentStock <= p.reorderLevel ? 'badge-danger' : 'badge-success'}`}>
                        {formatQty(p.currentStock)} {p.unitSymbol || ''}
                      </span>
                    </td>
                    <td><span className={`badge ${p.isActive ? 'badge-success' : 'badge-danger'}`}>{p.isActive ? 'Activo' : 'Inactivo'}</span></td>
                    <td>
                      <button className="btn btn-ghost btn-sm" onClick={() => handleEdit(p)}><Edit2 size={14} /></button>
                      <button className="btn btn-ghost btn-sm" style={{ color: 'var(--danger)' }} onClick={() => { if (confirm('¿Desactivar este producto?')) deleteMutation.mutate(p.id); }}><Trash2 size={14} /></button>
                      {!p.isActive && (
                        <button className="btn btn-ghost btn-sm" style={{ color: 'var(--success)' }} onClick={() => updateMutation.mutate({ id: p.id, data: { name: p.name, sku: p.sku, unitId: p.unitId, reorderLevel: p.reorderLevel, description: p.description || '', isActive: true } })} title="Reactivar"><RotateCcw size={14} /></button>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            {products.length === 0 && <p style={{ textAlign: 'center', padding: '2rem', color: 'var(--text-light)' }}>No se encontraron productos</p>}
          </div>
        )}
      </div>
    </div>
  );
}
