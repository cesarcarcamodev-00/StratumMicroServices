# API Reference

Todas las rutas se exponen a través del portal en `http://localhost:3000/api` (nginx enruta a cada servicio).

| Prefijo | Servicio |
|---------|----------|
| `/auth`, `/users`, `/roles`, `/permissions` | identity (`http://localhost:5001/api`) |
| resto de `/api/*` | inventory (`http://localhost:5000/api`) |

Todos los endpoints (excepto login/refresh) requieren `Authorization: Bearer <token>`.

## Formato de respuesta

### Éxito
```json
{ "succeeded": true, "data": {}, "message": null }
```

### Error
```json
{ "succeeded": false, "errors": ["mensaje"], "message": "..." }
```

### Paginado (inventario)
```json
{ "items": [], "totalCount": 50, "page": 1, "pageSize": 20, "totalPages": 3, "hasPreviousPage": false, "hasNextPage": true }
```

## Identidad

### POST /auth/login
`{ "username": "admin", "password": "Admin123!" }`

**Response data:**
```json
{
  "token": "eyJhbGci...", "refreshToken": "base64...", "expiresAt": "2026-...",
  "userId": "guid", "username": "admin", "email": "admin@inventario.com",
  "roles": ["Admin"], "permissions": ["identity.users.view", "..."]
}
```

### POST /auth/refresh
`{ "refreshToken": "..." }` → devuelve un nuevo `AuthResponse`.

### POST /auth/logout
Invalida el refresh token del usuario actual.

### POST /auth/change-password
`{ "currentPassword": "...", "newPassword": "..." }`

### GET /auth/me
Devuelve `{ userId, username, roles, permissions, succeeded }` del token actual.

### GET /users
Lista usuarios paginada. Query: `page`, `pageSize`, `search`. Requiere `identity.users.manage`.

### POST /users
```json
{ "username": "jdoe", "email": "j@corp.com", "password": "Pass123!", "isActive": true, "roleIds": ["guid"], "permissionIds": [] }
```

### PUT /users/{id}
Actualiza usuario (sin password: no se modifica). Requiere `identity.users.manage`.

### DELETE /users/{id}
Elimina usuario (no puedes eliminarte a ti mismo).

### GET /roles
Lista roles. Query: `includeInactive`. Requiere `identity.users.manage`.

### POST /roles
```json
{ "name": "Auditor", "description": "...", "permissionIds": ["guid"] }
```

### PUT /roles/{id} / DELETE /roles/{id}
Actualiza / elimina un rol (el rol Admin no se elimina desde el frontend).

### GET /permissions
Lista permisos. Query: `module`. Autenticado.

## Inventario

### GET/POST /productos, PUT/DELETE /productos/{id}
Productos con paginación. Query GET: `search`, `categoryId`, `isActive`, `page`, `pageSize`.

### GET/POST /categorias, PUT/DELETE /categorias/{id}
Categorías. Query GET: `includeInactive`.

### GET/POST /categorias/{id}/atributos, PUT/DELETE /categorias/{id}/atributos/{attrId}
Atributos personalizados de categoría.

### GET/PUT /productos/{id}/atributos-valores
Valores de atributos de un producto.

### GET /unidades, POST /unidades, PUT/DELETE /unidades/{id}
Unidades de medida (con `includeInactive` y `productCount`).

### GET /inventario/stock/{productId}
Stock por ubicación de un producto.

### GET /inventario/stock-bajo
Productos con stock en o bajo el nivel de reorden.

### POST /inventario/ajustar
```json
{ "productId": "guid", "quantityChange": 50, "movementType": 0, "notes": "..." }
```
`movementType`: 0 = Entrada, 1 = Salida, 2 = Ajuste.

### GET /movimientos-inventario
Movimientos paginados. Query: `productId`, `movementType`, `fromDate`, `toDate`, `page`, `pageSize`.

### GET /reportes/dashboard
```json
{ "activeProducts": 6, "inactiveProducts": 0, "lowStockProducts": 1, "outOfStockProducts": 0,
  "activeCategories": 4, "inactiveCategories": 0, "inventoryValue": 523.05, "totalMovementsThisMonth": 3 }
```

### GET /settings, PUT /settings
Settings globales: `{ "settings": [{ "key": "...", "value": "..." }] }`

## Códigos de estado

| Código | Descripción |
|--------|-------------|
| 200 | Éxito |
| 400 | Error de validación / negocio (con `errors[]`) |
| 401 | Token faltante o inválido |
| 403 | Sin permisos |
| 404 | Recurso no encontrado |
| 500 | Error interno |
