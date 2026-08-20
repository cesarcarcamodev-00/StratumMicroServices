import { useState } from 'react';
import { useQuery } from 'react-query';
import { inventoryMovementsService, productsService } from '../services/api';
import { Search, ChevronLeft, ChevronRight } from 'lucide-react';
import { formatQty, formatDate } from '../utils/format';

const typeMeta = {
  In: { label: 'Entrada', badge: 'badge-success' },
  Out: { label: 'Salida', badge: 'badge-danger' },
  Adjustment: { label: 'Ajuste', badge: 'badge-warning' },
  Waste: { label: 'Merma', badge: 'badge-warning' },
};

export default function MovementsPage() {
  const [productId, setProductId] = useState('');
  const [movementType, setMovementType] = useState('');
  const [fromDate, setFromDate] = useState('');
  const [toDate, setToDate] = useState('');
  const [page, setPage] = useState(1);
  const pageSize = 50;

  const params = {
    page,
    pageSize,
    productId: productId || undefined,
    movementType: movementType || undefined,
    fromDate: fromDate || undefined,
    toDate: toDate || undefined,
  };

  const { data, isLoading } = useQuery(['movements', params], () => inventoryMovementsService.getAll(params), { keepPreviousData: true });
  const { data: productsData } = useQuery('active-products-all', () => productsService.getAll({ pageSize: 100, isActive: true }));

  const items = data?.data?.items || [];
  const totalCount = data?.data?.totalCount || 0;
  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
  const products = productsData?.data?.items || [];

  const resetPage = () => setPage(1);

  return (
    <div>
      <h2 style={{ marginBottom: '1.5rem' }}>Movimientos de Inventario</h2>

      <div className="card" style={{ marginBottom: '1rem' }}>
        <div style={{ display: 'flex', gap: '0.75rem', alignItems: 'flex-end', flexWrap: 'wrap' }}>
          <div className="form-group" style={{ margin: 0, minWidth: '220px', flex: 1 }}>
            <label>Producto</label>
            <select value={productId} onChange={(e) => { setProductId(e.target.value); resetPage(); }}>
              <option value="">Todos los productos</option>
              {products.map((p) => <option key={p.id} value={p.id}>{p.name} ({p.sku})</option>)}
            </select>
          </div>
          <div className="form-group" style={{ margin: 0, minWidth: '160px' }}>
            <label>Tipo</label>
            <select value={movementType} onChange={(e) => { setMovementType(e.target.value); resetPage(); }}>
              <option value="">Todos</option>
              <option value="In">Entrada</option>
              <option value="Out">Salida</option>
              <option value="Adjustment">Ajuste</option>
              <option value="Waste">Merma</option>
            </select>
          </div>
          <div className="form-group" style={{ margin: 0 }}>
            <label>Desde</label>
            <input type="date" value={fromDate} onChange={(e) => { setFromDate(e.target.value); resetPage(); }} />
          </div>
          <div className="form-group" style={{ margin: 0 }}>
            <label>Hasta</label>
            <input type="date" value={toDate} onChange={(e) => { setToDate(e.target.value); resetPage(); }} />
          </div>
          <button className="btn btn-secondary" onClick={resetPage}><Search size={16} /> Buscar</button>
        </div>
      </div>

      <div className="card">
        {isLoading ? <p>Cargando...</p> : (
          <>
            <div className="table-container">
              <table>
                <thead>
                  <tr>
                    <th>Producto</th>
                    <th>Tipo</th>
                    <th>Cantidad</th>
                    <th>Stock Antes</th>
                    <th>Stock Después</th>
                    <th>Referencia</th>
                    <th>Notas</th>
                    <th>Usuario</th>
                    <th>Fecha</th>
                  </tr>
                </thead>
                <tbody>
                  {items.map((m) => {
                    const meta = typeMeta[m.movementType] || { label: m.movementType, badge: 'badge' };
                    return (
                      <tr key={m.id}>
                        <td><strong>{m.productName}</strong></td>
                        <td><span className={`badge ${meta.badge}`}>{meta.label}</span></td>
                        <td>{formatQty(m.quantity)}</td>
                        <td>{formatQty(m.quantityBefore)}</td>
                        <td>{formatQty(m.quantityAfter)}</td>
                        <td>{m.referenceType ? `${m.referenceType}${m.referenceId ? ` · ${m.referenceId}` : ''}` : '—'}</td>
                        <td>{m.notes || '—'}</td>
                        <td>
                          {m.createdBy ? (
                            <span className="badge badge-info">{m.createdBy}</span>
                          ) : '—'}
                        </td>
                        <td>{formatDate(m.createdAt)}</td>
                      </tr>
                    );
                  })}
                  {items.length === 0 && (
                    <tr><td colSpan={9} style={{ textAlign: 'center', padding: '2rem', color: 'var(--text-light)' }}>No se encontraron movimientos</td></tr>
                  )}
                </tbody>
              </table>
            </div>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginTop: '1rem' }}>
              <span style={{ fontSize: '0.8125rem', color: 'var(--text-light)' }}>Mostrando {items.length} de {totalCount} movimientos</span>
              <div style={{ display: 'flex', gap: '0.5rem', alignItems: 'center' }}>
                <button className="btn btn-ghost btn-sm" disabled={page <= 1} onClick={() => setPage((p) => p - 1)}><ChevronLeft size={16} /></button>
                <span style={{ fontSize: '0.8125rem' }}>Página {page} de {totalPages}</span>
                <button className="btn btn-ghost btn-sm" disabled={page >= totalPages} onClick={() => setPage((p) => p + 1)}><ChevronRight size={16} /></button>
              </div>
            </div>
          </>
        )}
      </div>
    </div>
  );
}
