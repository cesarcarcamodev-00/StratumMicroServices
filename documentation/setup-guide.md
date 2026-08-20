# Setup Guide

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 20+](https://nodejs.org/)
- [Docker](https://www.docker.com/) con Docker Compose

## Quick Start (Docker)

```bash
docker compose up -d --build
```

| Servicio | URL |
|----------|-----|
| Portal | http://localhost:3000 |
| API Inventario | http://localhost:5000/api |
| API Identidad | http://localhost:5001/api |

Las migraciones se aplican automáticamente al arrancar. El seed crea permisos, roles y el usuario admin.

### Credenciales por defecto

| Usuario | Contraseña | Rol |
|---------|------------|-----|
| admin | Admin123! | Admin |

## Desarrollo local

### Backend de identidad

```bash
cd services/identity
dotnet restore
dotnet run --project src/InventoryApp.Identity.WebApi   # http://localhost:5164
```

### Backend de inventario

```bash
cd services/inventory
dotnet restore
dotnet run --project src/InventoryApp.WebApi            # http://localhost:5200
```

Requiere PostgreSQL accesible en `localhost:5432` (ver `ConnectionStrings__PostgresConnection`). También puedes levantar solo la BD con `docker compose up -d postgres`.

### Portal

```bash
cd apps/portal
npm install
npm run dev                                            # http://localhost:3000
```

El proxy de Vite enruta `/api/auth|users|roles|permissions` → identity (5164) y el resto de `/api` → inventory (5200).

## Base de datos

- Usuario/BD por defecto: `inventory` / `inventory123` / base `inventory`.
- Esquema por módulo: `identity` e `inventory`; historial de migraciones en `public`.
- Migraciones (desde la carpeta del servicio):

```bash
cd services/inventory
dotnet ef database update -s src/InventoryApp.WebApi -p src/InventoryApp.Infrastructure

cd services/identity
dotnet ef database update -s src/InventoryApp.Identity.WebApi -p src/InventoryApp.Identity.Infrastructure
```

## Cambiar el secreto JWT

Definir `JwtSettings:Secret` (appsettings o variable de entorno). **Debe ser idéntico** en identity e inventory. Sin secret configurado, el servicio falla al arrancar (fail-fast).

## Ejecutar migraciones de EF (nuevo esquema)

```bash
dotnet ef migrations add <Nombre> -s src/InventoryApp.WebApi -p src/InventoryApp.Infrastructure -o Persistence/Migrations
```

## Troubleshooting

- **El portal no conecta**: verifica que identity e inventory estén corriendo y que el JWT compartido sea el mismo en ambos.
- **401 en inventario**: el token es emitido por identity con el mismo secret/issuer/audience.
- **Migraciones duplicadas**: el historial está fijado en `public`; no cambies el `search_path` por defecto.
- **Puertos ocupados**: detén servicios locales (`dotnet`) y contenedores viejos antes de `docker compose up`.
