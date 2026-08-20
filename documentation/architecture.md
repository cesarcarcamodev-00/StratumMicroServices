# Architecture Documentation

## Overview

Monorepo multi-módulo con microservicios desplegables sobre un único PostgreSQL (esquema por módulo). Un servicio de **Identidad** central emite JWT con rol + permisos; el módulo **Inventario** se autentica contra ese JWT. Un **portal único** (React) consume ambos servicios a través de nginx.

## Repository Structure

```
InventoryApp/
├── services/
│   ├── inventory/            # Módulo de inventario (.NET 10, Clean Architecture)
│   │   ├── InventoryApp.slnx
│   │   └── src/
│   │       ├── InventoryApp.Domain/          # Entidades, enums, value objects
│   │       ├── InventoryApp.Application/     # CQRS + MediatR + FluentValidation + Mapster
│   │       ├── InventoryApp.Infrastructure/  # EF Core (Npgsql), seed
│   │       └── InventoryApp.WebApi/          # Host API (JWT validation)
│   └── identity/             # Servicio de identidad (.NET 10)
│       ├── InventoryApp.Identity.slnx
│       └── src/
│           ├── InventoryApp.Identity.Domain/          # User, Role, Permission + joins
│           ├── InventoryApp.Identity.Application/     # CQRS, auth, DTOs
│           ├── InventoryApp.Identity.Infrastructure/  # DbContext (schema identity), JWT, BCrypt
│           └── InventoryApp.Identity.WebApi/          # Auth/Users/Roles/Permissions API
├── apps/portal/              # Portal único (React 18 + Vite)
└── docker-compose.yml
```

## Layer Architecture (por servicio)

### Domain Layer
- Zero dependencies. Entidades, enums y value objects del módulo.
- Inventario: Product, Category, InventoryItem, InventoryMovement, UnitOfMeasure, CategoryAttribute, ProductAttributeValue, AppSetting.
- Identidad: User, Role, Permission + entidades de unión UserRole, UserRolePermissions.

### Application Layer
- Depende solo de Domain. CQRS con **MediatR**, validación con **FluentValidation**, mapeo con **Mapster**.
- Patrón de respuesta `Result<T>` / `Result` y `PagedResult<T>`.
- Interfaces de infraestructura (`IApplicationDbContext`, `IIdentityDbContext`).

### Infrastructure Layer
- EF Core + **Npgsql** sobre PostgreSQL. `MigrationsHistoryTable` fijado en `public`.
- Identidad: generación de JWT (claims `permission` por permiso), hashing BCrypt, seed idempotente.

### WebApi Layer
- Controllers `api/[controller]`, JWT Bearer, políticas por permiso en ambos servicios.
- Middlewares de excepción y logging de request. Swagger en Development. CORS para el portal.

## Persistence

Un solo PostgreSQL con esquemas por módulo:

| Esquema | Servicio | Tablas |
|---------|----------|--------|
| `identity` | identity | Users, Roles, Permissions, UserRoles, RolePermissions, UserPermissions |
| `inventory` | inventory | Products, Categories, InventoryItems, InventoryMovements, CategoryAttributes, ProductAttributeValues, UnitsOfMeasure, AppSettings |
| `public` | — | `__EFMigrationsHistory` (historial compartido) |

Cada servicio se registra con `HasDefaultSchema` propio y aplica migraciones al arrancar (`MigrateAsync` en Postgres).

## Security

- **Login** → identity valida BCrypt → emite JWT con claims de rol (`ClaimTypes.Role`) y de permisos (`permission`).
- **Inventario** valida el JWT (mismo issuer/audience/secret) y autoriza por permisos `inventory.*` (mismo patrón de identidad); Admin tiene bypass por rol.
- **Identidad** autoriza por permisos (`[Authorize(Policy = "ManageUsers")]` → permiso `identity.users.manage`).
- Permisos sembrados: módulos `identity.*` e `inventory.*`. Roles: Admin, Manager, Viewer (el rol Manager recibe los 8 permisos de inventario; Viewer solo lectura).

## Routing del Portal (nginx)

| Prefijo | Servicio |
|---------|----------|
| `/api/auth`, `/api/users`, `/api/roles`, `/api/permissions` | identity |
| resto de `/api/*` | inventory |
| `/` | SPA (React) |

En desarrollo, Vite replica este enrutamiento con su proxy (identity en `5164`, inventory en `5200`).

## Tech Stack

| Componente | Tecnología |
|------------|------------|
| Backends | .NET 10, EF Core 10, Npgsql, MediatR 12, FluentValidation, Mapster |
| Auth | JWT Bearer + BCrypt |
| Base de datos | PostgreSQL 17 |
| Frontend | React 18 + Vite, axios, react-query, react-router |
| Deploy | docker-compose (nginx como reverse proxy) |
