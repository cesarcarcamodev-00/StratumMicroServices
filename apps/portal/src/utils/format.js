export const formatQty = (q) => (Number.isInteger(q) ? q : (q ?? 0).toFixed(2).replace(/\.?0+$/, ''));

export const formatDate = (d) => (d ? new Date(d).toLocaleString('es') : '');
