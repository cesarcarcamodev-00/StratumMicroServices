import axios from 'axios';

const api = axios.create({
  baseURL: '/api',
  headers: { 'Content-Type': 'application/json' },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('refreshToken');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export const authService = {
  login: (data) => api.post('/auth/login', data),
  logout: () => api.post('/auth/logout'),
  me: () => api.get('/auth/me'),
};

export const usersService = {
  getAll: (params) => api.get('/users', { params }),
  create: (data) => api.post('/users', data),
  update: (id, data) => api.put(`/users/${id}`, data),
  delete: (id) => api.delete(`/users/${id}`),
};

export const rolesService = {
  getAll: (params) => api.get('/roles', { params }),
  create: (data) => api.post('/roles', data),
  update: (id, data) => api.put(`/roles/${id}`, data),
  delete: (id) => api.delete(`/roles/${id}`),
};

export const permissionsService = {
  getAll: (params) => api.get('/permissions', { params }),
};

export const productsService = {
  getAll: (params) => api.get('/productos', { params }),
  create: (data) => api.post('/productos', data),
  update: (id, data) => api.put(`/productos/${id}`, data),
  delete: (id) => api.delete(`/productos/${id}`),
};

export const inventoryService = {
  getStock: (productId) => api.get(`/inventario/stock/${productId}`),
  getLowStock: () => api.get('/inventario/stock-bajo'),
  adjust: (data) => api.post('/inventario/ajustar', data),
};

export const inventoryMovementsService = {
  getAll: (params) => api.get('/movimientos-inventario', { params }),
};

export const unitsService = {
  getAll: (params) => api.get('/unidades', { params }),
};

export default api;
