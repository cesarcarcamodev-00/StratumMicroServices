import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from 'react-query';
import { inventoryService, productsService } from '../services/api';
import { Plus, Minus, Package, Search, SlidersHorizontal } from 'lucide-react';
import toast from 'react-hot-toast';
import { getErrorMessage } from '../utils/errors';
import { formatQty } from '../utils/format';
import StockAdjustModal from '../components/stock/StockAdjustModal';

export default function InventoryPage() {
  const [quickQtys, setQuickQtys] = useState({});
  const [search, setSearch] = useState('');
  const [adjustItem, setAdjustItem] = useState(null);
  const queryClient = useQueryClient();

  const { data: lowStockData, isLoading } = useQuery('low-stock', () => inventoryService.getLowStock());
  const { data: productsData } = useQuery('active-products', () => productsService.getAll({ pageSize: 100, isActive: true }));

  const products = productsData?.data?.items || [];

  const filteredProducts = products.filter((p) =>
    !search || p.name.toLowerCase().includes(search.toLowerCase()) || p.sku.toLowerCase().includes(search.toLowerCase())
  );

  const refreshInventory = () => {
    queryClient.invalidateQueries('low-stock');
    queryClient.invalidateQueries('active-products');
  };

  const adjustMutation = useMutation((data) => inventoryService.adjust(data), {
    onSuccess: () => {
      refreshInventory();
      setQuickQtys({});
      toast.success('Stock actualizado correctamente');
    },
    onError: (err) => toast.error(getErrorMessage(err)),
  });

  const handleQuickAdjust = (item, direction) => {
    const qty = parseFloat(quickQtys[item.id]);
    if (!qty || qty <= 0) return toast.error('Ingresa una cantidad mayor a 0');
    adjustMutation.mutate({
      productId: item.id,
      quantityChange: direction === 'In' ? qty : -qty,
      movementType: direction === 'In' ? 0 : 1,
      notes: '',
    });
  };

  const renderProductRow = (item) => (
    <tr key={item.id}>
      <td>
        <strong>{item.name}</strong>
        <div style={{ fontSize: '0.75rem', color: 'var(--text-light)' }}>{item.description || item.sku}</div>
      </td>
      <td>{item.sku}</td>
      <td>
        <span className={`badge ${item.currentStock <= item.reorderLevel ? 'badge-danger' : 'badge-success'}`}>
          <Package size={12} style={{ marginRight: '0.25rem' }} />
          {formatQty(item.currentStock)} {item.unitSymbol || ''}
        </span>
      </td>
      <td>{formatQty(item.reorderLevel)} {item.unitSymbol || ''}</td>
      <td>
        <div style={{ display: 'flex', gap: '0.35rem', alignItems: 'center' }}>
          <input
            type="number"
            min="0"
            step="0.001"
            style={{ width: '100px' }}
            placeholder="Cant."
            value={quickQtys[item.id] || ''}
            onChange={(e) => setQuickQtys({ ...quickQtys, [item.id]: e.target.value })}
          />
          <button className="btn btn-success btn-sm" onClick={() => handleQuickAdjust(item, 'In')}>
            <Plus size={14} /> Entrada
          </button>
          <button className="btn btn-danger btn-sm" onClick={() => handleQuickAdjust(item, 'Out')}>
            <Minus size={14} /> Salida
          </button>
          <button className="btn btn-ghost btn-sm" title="Ajuste o con notas" onClick={() => setAdjustItem(item)}>
            <SlidersHorizontal size={14} /> Ajustar
          </button>
        </div>
      </td>
    </tr>
  );

  return (
    <div>
      <h2 style={{ marginBottom: '1.5rem' }}>Inventario</h2>

      <div className="card" style={{ marginBottom: '1rem' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem', gap: '1rem', flexWrap: 'wrap' }}>
          <h3 style={{ margin: 0 }}>Ajuste Rápido</h3>
          <div className="form-group" style={{ margin: 0, position: 'relative', minWidth: '240px', flex: 1, maxWidth: '360px' }}>
            <Search size={16} style={{ position: 'absolute', left: '0.75rem', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-light)' }} />
            <input
              style={{ paddingLeft: '2.5rem' }}
              placeholder="Buscar producto..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
          </div>
        </div>

        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>Producto</th>
                <th>SKU</th>
                <th>Stock Actual</th>
                <th>Stock Mínimo</th>
                <th style={{ width: '280px' }}>Cantidad y Acción</th>
              </tr>
            </thead>
            <tbody>
              {filteredProducts.map(renderProductRow)}
              {filteredProducts.length === 0 && (
                <tr><td colSpan={5} style={{ textAlign: 'center', padding: '2rem', color: 'var(--text-light)' }}>No hay productos activos</td></tr>
              )}
            </tbody>
          </table>
        </div>
      </div>

      <div className="card">
        <h3 style={{ marginBottom: '1rem' }}>Productos con Stock Bajo</h3>
        {isLoading ? <p>Cargando...</p> : (
          <div className="table-container">
            <table>
              <thead><tr><th>Producto</th><th>SKU</th><th>Stock Actual</th><th>Stock Mínimo</th><th>Estado</th></tr></thead>
              <tbody>
                {lowStockData?.data?.map((p) => (
                  <tr key={p.id}>
                    <td><strong>{p.name}</strong></td>
                    <td>{p.sku}</td>
                    <td>{formatQty(p.currentStock)} {p.unitSymbol || ''}</td>
                    <td>{formatQty(p.reorderLevel)} {p.unitSymbol || ''}</td>
                    <td><span className={`badge ${p.currentStock === 0 ? 'badge-danger' : 'badge-warning'}`}>{p.currentStock === 0 ? 'Sin Stock' : 'Stock Bajo'}</span></td>
                  </tr>
                ))}
                {lowStockData?.data?.length === 0 && <tr><td colSpan={5} style={{ textAlign: 'center' }}>Todos los productos tienen stock suficiente</td></tr>}
              </tbody>
            </table>
          </div>
        )}
      </div>

      <StockAdjustModal
        item={adjustItem}
        kind="products"
        onClose={() => setAdjustItem(null)}
        onSuccess={refreshInventory}
      />
    </div>
  );
}
