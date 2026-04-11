# CLAUDE.md — PlanTA

> ERP/MES ligero para PYMEs y pequeñas fábricas.
> Instrucciones específicas del proyecto para Claude Code.
> Prevalece sobre el CLAUDE.md global en caso de conflicto.
> Para arquitectura detallada, ver `ARCHITECTURE.md`.

---

## Sobre el Proyecto

**PlanTA** (antes mini-SAP) es un sistema de gestión integral para PYMEs industriales: inventario, producción (BOM/OF), compras, ventas, calidad y trazabilidad de lotes. Sustituye Excel con una plataforma modular.

### Sub-proyectos

| Nombre | Carpeta | Tecnología | Estado |
|---|---|---|---|
| Backend | `PlanTA/` (a renombrar desde `AcademiaOposiciones/`) | .NET 9 Clean Architecture multi-módulo | Estructura inicial |
| Frontend | `fixflow/` (a renombrar a `planta-web/`) | Angular 21, standalone, SCSS | Landing + Auth + Dashboard + Shell |

---

## Stack del Proyecto

| Capa | Tecnología | Versión |
|---|---|---|
| Backend | ASP.NET Core + Clean Architecture multi-módulo | .NET 9 / C# 13 |
| ORM | Entity Framework Core + Npgsql | 9.x |
| Base de datos | PostgreSQL — un schema por módulo | Docker local / Neon prod |
| Auth | ASP.NET Core Identity + JWT + Refresh Tokens | — |
| CQRS | MediatR con pipeline de behaviors | 12.x |
| Validación | FluentValidation | 11.x |
| Jobs | Hangfire + PostgreSQL (Outbox processor) | latest |
| Logs | Serilog → Seq | 4.x |
| Tests | xUnit + FluentAssertions + NSubstitute | latest |
| Frontend | Angular standalone components + Signals | 21 |
| CSS | SCSS (sistema de diseño industrial propio) | — |
| Despliegue | Render (API) + Vercel (frontend) + Neon (DB) | — |

---

## Módulos de Negocio

| Módulo | Schema PG | Entidades principales |
|---|---|---|
| **Inventario** | `inventario` | Producto, Almacen, Ubicacion, Lote, MovimientoStock, AlertaStock |
| **Producción** | `produccion` | ListaMateriales (BOM), RutaProduccion, OrdenFabricacion, ParteProduccion |
| **Compras** | `compras` | Proveedor, OrdenCompra, Recepcion |
| **Ventas** | `ventas` | Cliente, Pedido, Expedicion |
| **Calidad** | `calidad` | PlantillaInspeccion, Inspeccion, NoConformidad |
| **Seguridad** | `seguridad` | Usuario, Empresa, LogAuditoria |

---

## Patrones Obligatorios

### Backend

- **Result<T>** para errores de negocio, nunca excepciones
- **CQRS**: ICommand<T> y IQuery<T> vía MediatR
- **Strongly Typed IDs**: `record ProductoId(Guid Value) : EntityId(Value)`
- **Soft Delete**: `IHasSoftDelete` + query filters automáticos
- **Minimal API**: `IEndpointGroup`, NO controllers
- **DomainEvents**: comunicación entre módulos, nunca referencias directas
- **Outbox Pattern**: consistencia eventual para eventos cross-módulo
- **Multi-tenant**: `ICurrentTenant` inyectado, queries filtran por EmpresaId
- **Un DbContext por módulo** con schema propio

### Frontend

- **Standalone + OnPush** siempre
- **Signals** para estado (signal, computed, effect)
- **Lazy loading** en todas las rutas
- **Mock data** como constante de módulo, nunca `this.method()` en initializers

---

## Lo que NO se debe hacer

### Backend
- No lanzar `Exception` para errores de negocio — usar `Result<T>`
- No exponer entidades de dominio — siempre DTOs
- No usar controllers — solo Minimal API con `IEndpointGroup`
- No referenciar un módulo desde otro — solo DomainEvents via SharedKernel
- No usar `int` o `Guid` suelto como ID — siempre Strongly Typed ID
- No borrar físicamente — siempre soft delete
- No hardcodear secrets
- No crear un DbContext global — cada módulo tiene el suyo

### Frontend
- No importar componentes directamente en `app.routes.ts`
- No usar NgModules — standalone only
- No usar `Default` change detection — siempre `OnPush`
- No inicializar signals con `this.method()` en property initializers

---

## Comandos Frecuentes

```bash
# Backend
cd PlanTA/PlanTA.API && dotnet run

# Migraciones (por módulo)
dotnet ef migrations add NombreDescriptivo \
  --project src/PlanTA.Inventario.Infrastructure \
  --startup-project src/PlanTA.API

# Frontend
cd planta-web && npm run start

# Docker
docker-compose up -d
```

---

## Protocolo de Memoria

- Al **INICIO** de cada sesión: busca en memory el contexto de este proyecto
- Al **FINAL** de cada tarea: guarda en memory el estado actual y decisiones tomadas
