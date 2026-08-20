import { useState, useEffect } from 'react';
import { useMutation } from 'react-query';
import { inventoryService } from '../../services/api';
import { ArrowDownCircle, ArrowUpCircle, X } from 'lucide-react';
import toast from 'react-hot-toast';
import { getErrorMessage } from '../../utils/errors';

const TYPES = [
  { value: 'In', label: 'Entrada', icon: ArrowUpCircle, color: 'var(--success)', desc: 'Aumenta el stock' },
  { value: 'Out', label: 'Salida', icon: ArrowDownCircle, color: 'var(--danger)', desc: 'Disminuye el stock' },
];

export default function StockAdjustModal({ item, onClose, onSuccess }) {
  const [type, setType] = useState('In');
  const [qty, setQty] = useState('');
  const [notes, setNotes] = useState('');

  useEffect(() => {
    if (item) {
      setType('In');
      setQty('');
      setNotes('');
    }
  }, [item]);

  const mutation = useMutation((payload) => inventoryService.adjust(payload), {
    onSuccess: () => {
      toast.success('Stock actualizado correctamente');
      if (onSuccess) onSuccess();
      onClose();
    },
    onError: (err) => toast.error(getErrorMessage(err)),
  });

  if (!item) return null;

  const qtyValue = parseFloat(qty);
  const valid = qtyValue > 0;

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!valid) return toast.error('Ingresa una cantidad mayor a 0');
    mutation.mutate({
      productId: item.id,
      quantityChange: type === 'In' ? qtyValue : -qtyValue,
      movementType: { In: 0, Out: 1 }[type],
      notes: notes.trim() || null,
    });
  };

  const selectedType = TYPES.find((t) => t.value === type);

  return (
    <div className="adjust-overlay" onClick={onClose}>
      <div className="adjust-panel" onClick={(e) => e.stopPropagation()}>
        <div className="adjust-header">
          <h3 style={{ margin: 0 }}>Ajustar Stock</h3>
          <button className="btn btn-ghost btn-sm" onClick={onClose}><X size={16} /></button>
        </div>

        <p style={{ margin: '0.25rem 0 1rem', color: 'var(--text-light)', fontSize: '0.9rem' }}>
          <strong>{item.name}</strong>
          {item.sku ? ` · ${item.sku}` : ''}
          {item.unitSymbol ? ` · ${item.unitSymbol}` : ''}
        </p>

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Tipo de Movimiento</label>
            <div style={{ display: 'flex', flexDirection: 'column', gap: '0.4rem' }}>
              {TYPES.map((t) => {
                const Icon = t.icon;
                const active = type === t.value;
                return (
                  <button
                    key={t.value}
                    type="button"
                    onClick={() => setType(t.value)}
                    className="adjust-type"
                    style={{
                      borderColor: active ? t.color : 'var(--border)',
                      background: active ? `${t.color}12` : 'transparent',
                    }}
                  >
                    <span className="adjust-type-icon" style={{ background: `${t.color}1a`, color: t.color }}>
                      <Icon size={18} />
                    </span>
                    <span style={{ flex: 1 }}>
                      <span style={{ display: 'block', fontWeight: 600, fontSize: '0.875rem' }}>{t.label}</span>
                      <span style={{ display: 'block', fontSize: '0.75rem', color: 'var(--text-light)' }}>{t.desc}</span>
                    </span>
                    {type === t.value && <span className="badge" style={{ background: `${t.color}1a`, color: t.color }}>✓</span>}
                  </button>
                );
              })}
            </div>
          </div>

          <div className="form-group">
            <label>Cantidad *</label>
            <input
              type="number"
              min="0"
              step="0.001"
              autoFocus
              value={qty}
              onChange={(e) => setQty(e.target.value)}
              placeholder="0.000"
              required
            />
          </div>

          <div className="form-group">
            <label>Notas (opcional)</label>
            <textarea value={notes} onChange={(e) => setNotes(e.target.value)} rows={2} placeholder="Ej. conteo físico" />
          </div>

          <div className="adjust-footer">
            <button type="button" className="btn btn-ghost" onClick={onClose}>Cancelar</button>
            <button type="submit" className="btn btn-primary" disabled={!valid || mutation.isLoading}>
              {mutation.isLoading ? 'Aplicando...' : `Aplicar ${selectedType.label}`}
            </button>
          </div>
        </form>
      </div>

      <style>{`
        .adjust-overlay {
          position: fixed; inset: 0; background: rgba(15, 23, 42, 0.6);
          display: flex; align-items: center; justify-content: center; z-index: 200; padding: 1rem;
        }
        .adjust-panel {
          width: 100%; max-width: 480px; max-height: 90vh; overflow-y: auto;
          background: white; border-radius: 14px; box-shadow: var(--shadow-md);
          padding: 1.5rem;
        }
        .adjust-header { display: flex; justify-content: space-between; align-items: center; }
        .adjust-type {
          display: flex; align-items: center; gap: 0.75rem; text-align: left;
          width: 100%; padding: 0.6rem; border-radius: 10px; border: 1px solid var(--border);
          background: transparent; cursor: pointer; color: var(--text);
        }
        .adjust-type:hover { border-color: #cbd5e1; background: var(--bg-secondary); }
        .adjust-type-icon {
          display: flex; align-items: center; justify-content: center;
          width: 36px; height: 36px; border-radius: 10px; flex-shrink: 0;
        }
        .adjust-footer {
          display: flex; justify-content: flex-end; gap: 0.5rem; margin-top: 0.5rem;
        }
      `}</style>
    </div>
  );
}
