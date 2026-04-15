# PlanTA — Documentación Funcional

ERP/MES modular para PYMEs industriales que sustituye a Excel + WhatsApp + papel. Construido con .NET 9 (Clean Architecture + CQRS) y Angular 19 (standalone + signals). Multi-tenant por `EmpresaId`, multi-módulo con schemas PostgreSQL separados, desplegado en Render + Vercel + Neon.

---

## 1. Arquitectura general

### Stack

| Capa | Tecnología |
|---|---|
| Backend | .NET 9, Minimal API, MediatR 12, FluentValidation, Serilog |
| ORM / DB | Entity Framework Core 9 + PostgreSQL 16 (Neon en prod, Docker en local puerto 5433) |
| Auth | ASP.NET Core Identity + JWT |
| Frontend | Angular 19 standalone components, signals, RxJS, lazy loading |
| Mensajería interna | Outbox Pattern + DomainEventInterceptor + BackgroundService |
| Logging | Serilog + Seq (local) |
| Deploy | Render (API), Vercel (frontend), Neon (PostgreSQL) |

### Capas (Clean Architecture)

```
src/
├── PlanTA.SharedKernel/                  # BaseEntity, AggregateRoot, Result<T>, Error,
│                                           ValueObject, ICurrentTenant, CQRS interfaces,
│                                           MediatR Behaviors, Integration Events
├── PlanTA.<Modulo>.Domain/                # Entidades, ValueObjects, Enums, Domain Events, Errors
├── PlanTA.<Modulo>.Application/           # CQRS (Commands, Queries, Handlers, Validators),
│                                           DTOs, Interfaces de DbContext
├── PlanTA.<Modulo>.Infrastructure/        # DbContext, EF Configurations, repositorios,
│                                           servicios externos
└── PlanTA.API/                            # Minimal API, IEndpointGroup auto-discovery,
                                            JWT, CORS, Swagger, Program.cs, OutboxProcessor
```

### Multi-tenancy

- Toda entidad de negocio hereda de `BaseEntity` con `EmpresaId` (`ICurrentTenant` lo resuelve del JWT).
- Los DbContext aplican filtros automáticos por `EmpresaId`.
- El JWT incluye `nombre`, `rol` y `empresaNombre` (mapeados al frontend como `name`, `role`, `company`).

### Patrón Result<T>

Todos los handlers devuelven `Result<T>` o `Result`. No se usan excepciones para lógica de negocio. Los endpoints las traducen a 200/400/404/409.

### Soft delete

Las entidades implementan `SoftDeletableEntity` con `IsDeleted` + `DeletedAt`. El DbContext aplica query filters globales.

---

## 2. Estructura de schemas PostgreSQL

Cada módulo tiene su propio schema en la misma base de datos:

| Schema | Módulo |
|---|---|
| `inventario` | Inventario Core |
| `produccion` | Producción / BOM / Órdenes Fabricación |
| `compras` | Compras / Proveedores / Recepciones |
| `ventas` | Ventas / Clientes / Expediciones |
| `calidad` | Calidad / Inspecciones / NCs |
| `activos` | Activos físicos |
| `mantenimiento` | Órdenes de mantenimiento |
| `incidencias` | Incidencias de planta |
| `crm` | CRM (oportunidades, pipeline) |
| `costes` | Centros de coste, imputación |
| `facturacion` | Facturas, líneas |
| `oee` | OEE / paradas / disponibilidad |
| `rrhh` | Empleados, partes de horas |
| `ia` | Integraciones LLM (Claude) |
| `importador` | Carga masiva (CSV/Excel) |
| `seguridad` | Identity (usuarios, roles, empresas) |
| `shared` | Outbox + Audit Log |

---

## 3. Módulos de negocio (15)

### 3.1 Inventario

**Entidades:** Producto, Familia, Almacén, UbicacionAlmacen, MovimientoInventario, Lote, Alerta.
**ValueObjects:** SKU, CantidadStock, NivelStock.
**Comandos clave:** CreateProducto, AjustarStock, MoverInventario, CrearLote, CerrarAlerta.
**Queries:** ListProductos (paged), ListMovimientos, GetAlertasActivas.
**Endpoints:** `/api/v1/inventario/productos`, `/almacenes`, `/movimientos`, `/lotes`, `/alertas`.

### 3.2 Producción

**Entidades:** ListaMateriales (BOM), LineaBOM, RutaProduccion, OperacionRuta, OrdenFabricacion, LineaConsumoOF, ParteProduccion.
**ValueObjects:** CodigoOF, TiempoEstimado, CantidadPlanificada.
**Flujos:** crear BOM → asociar ruta → lanzar OF → registrar partes de producción → cerrar OF.
**Comandos:** CreateBOM, CreateRuta, CreateOF, CambiarEstadoOF, RegistrarProduccion.
**Endpoints:** `/api/v1/produccion/boms`, `/rutas`, `/ordenes-fabricacion`.

### 3.3 Compras

**Entidades:** Proveedor, ContactoProveedor, OrdenCompra, LineaOrdenCompra, Recepcion, LineaRecepcion.
**ValueObjects:** CondicionesPago.
**Flujo:** alta proveedor → crear OC → enviar → recepcionar → integración con Inventario (entrada de stock vía Outbox).
**Endpoints:** `/api/v1/compras/proveedores`, `/ordenes-compra`, `/recepciones`.

### 3.4 Ventas

**Entidades:** Cliente, ContactoCliente, Pedido, LineaPedido, Expedicion, LineaExpedicion.
**Flujo:** alta cliente → crear pedido → confirmar → expedir → integración con Inventario (salida de stock).
**Endpoints:** `/api/v1/ventas/clientes`, `/pedidos`, `/expediciones`.

### 3.5 Calidad

**Entidades:** PlantillaInspeccion, CriterioInspeccion, Inspeccion, ResultadoCriterio, NoConformidad, AccionCorrectiva.
**Flujo:** definir plantilla → ejecutar inspección sobre OF/recepción → si falla, abrir NC → asignar acciones correctivas → cerrar.
**Endpoints:** `/api/v1/calidad/plantillas`, `/inspecciones`, `/no-conformidades`.

### 3.6 Activos

**Entidades:** Activo, CategoriaActivo, HistorialActivo.
**Funcionalidad:** inventario de máquinas/equipos/instalaciones, ubicación, estado (activo/baja/en mantenimiento), criticidad. Base para Mantenimiento y OEE.
**Endpoints:** `/api/v1/activos`.

### 3.7 Mantenimiento

**Entidades:** OrdenMantenimiento, TipoMantenimiento (preventivo/correctivo/predictivo), PlanMantenimiento.
**Flujo:** planes preventivos generan OTs automáticas → técnico ejecuta → reporte de horas y materiales (consumo de Inventario vía Outbox) → cierre.
**Endpoints:** `/api/v1/mantenimiento/ordenes`, `/planes`.

### 3.8 Incidencias

**Entidades:** Incidencia, CategoriaIncidencia, ComentarioIncidencia.
**Funcionalidad:** captura de incidencias desde planta (móvil/PWA), asignación, escalado, integración con Mantenimiento (una incidencia puede generar OT).
**Endpoints:** `/api/v1/incidencias`.

### 3.9 CRM

**Entidades:** Oportunidad, EtapaPipeline, ContactoCRM, ActividadCRM.
**Funcionalidad:** pipeline de oportunidades comerciales con etapas, conversión a Cliente + Pedido cuando se gana.
**Endpoints:** `/api/v1/crm/oportunidades`, `/contactos`, `/actividades`.

### 3.10 Costes

**Entidades:** CentroCoste, ImputacionCoste, PresupuestoCentroCoste.
**Funcionalidad:** centros de coste, imputación de horas/materiales/otros, comparación presupuesto vs real.
**Endpoints:** `/api/v1/costes/centros`, `/imputaciones`.

### 3.11 Facturación

**Entidades:** Factura, LineaFactura, SerieFactura.
**Flujo:** generar factura desde Pedido (vía Integration Event de Ventas), numeración por serie, emisión.
**Endpoints:** `/api/v1/facturacion/facturas`, `/series`.

### 3.12 OEE (Overall Equipment Effectiveness)

**Entidades:** RegistroOEE, ParadaProduccion, MotivoParada.
**Funcionalidad:** cálculo de disponibilidad × rendimiento × calidad por activo y turno. Captura paradas con motivo.
**Endpoints:** `/api/v1/oee/registros`, `/paradas`.

### 3.13 RRHH

**Entidades:** Empleado, Departamento, ParteHoras, Ausencia.
**Funcionalidad:** datos básicos de empleados, partes de horas (input para Costes), gestión de ausencias.
**Endpoints:** `/api/v1/rrhh/empleados`, `/partes-horas`.

### 3.14 IA

**Funcionalidad:** integración con Anthropic Claude para:
- Sugerencias de planificación de OFs
- Análisis de no-conformidades recurrentes
- Resumen automático de incidencias
- Asistente conversacional sobre datos del ERP
**Endpoints:** `/api/v1/ia/chat`, `/sugerencias`.

### 3.15 Importador

**Funcionalidad:** carga masiva desde CSV/Excel:
- Productos / clientes / proveedores
- Histórico de movimientos
- Plantilla descargable + validación previa + commit transaccional
**Endpoints:** `/api/v1/importador/upload`, `/plantillas/{tipo}`.

---

## 4. Cross-cutting (Phase 6)

### 4.1 Outbox Pattern

- Tabla `shared.OutboxMessages`.
- `DomainEventInterceptor` (SaveChangesInterceptor de EF Core): captura los `DomainEvents` de los AggregateRoots y los serializa al Outbox dentro de la misma transacción.
- `OutboxProcessor` (BackgroundService): poll cada 5 s, deserializa, publica vía MediatR a `IIntegrationEventHandler<T>` y marca como procesado.
- Garantía: at-least-once cross-módulo sin acoplamiento directo.

### 4.2 Integration Events

Definidos en `SharedKernel/IntegrationEvents/`:
- `RecepcionConfirmadaEvent` (Compras → Inventario: entrada stock)
- `ExpedicionConfirmadaEvent` (Ventas → Inventario: salida stock)
- `OFFinalizadaEvent` (Producción → Inventario: consumo + producto terminado)
- `InspeccionFalladaEvent` (Calidad → Mantenimiento: posible NC con OT)
- `PedidoConfirmadoEvent` (Ventas → Facturación)
- `IncidenciaCriticaEvent` (Incidencias → Mantenimiento)

### 4.3 Audit Log

- `AuditEntry` en schema `shared`: usuario, comando, payload, timestamp, resultado.
- `AuditBehavior` (MediatR pipeline): solo registra Commands, no Queries.
- Endpoint `/api/v1/auditoria` (rol Admin).

### 4.4 MediatR Behaviors

Pipeline en orden:
1. **LoggingBehavior** — Serilog start/stop con duración.
2. **ValidationBehavior** — FluentValidation, devuelve `Result.Failure` si falla.
3. **AuditBehavior** — escribe AuditEntry para Commands.
4. **TransactionBehavior** — abre transacción para Commands.

---

## 5. Seguridad

### 5.1 Identity + JWT

- `PlanTA.Seguridad.Infrastructure` integra ASP.NET Core Identity con `ApplicationUser` (extiende IdentityUser con `Nombre`, `EmpresaId`, `ModulosDeshabilitados` CSV).
- `Empresa` extendida con `TrialHasta` (DateTimeOffset?) y `OnboardingCompletado` (bool). Factory `Empresa.Crear(nombre, cif, email, trialDias)`.
- Endpoints `/api/v1/seguridad/auth/login`, `/register` (self-service con trial 14 días), `/refresh`, `/change-password`.
- JWT firmado con HS256, clave 32+ chars, expiración configurable.
- Claims: `sub`, `email`, `nombre`, `rol`, `empresaId`, `empresaNombre`.
- `UserDto` incluye `OnboardingCompletado`, `TrialHasta`, `ModulosDeshabilitados[]`.
- Schema actualizado al arranque mediante `ALTER TABLE ... ADD COLUMN IF NOT EXISTS` idempotente en `Program.cs` (`TrialHasta`, `OnboardingCompletado`, `ModulosDeshabilitados`).

### 5.2 Roles

| Rol | Acceso |
|---|---|
| **Admin** | Todo (incluido Auditoría) |
| **Gerente** | Todos los módulos de negocio |
| **JefeAlmacen** | Inventario + Compras (recepciones) |
| **JefeProduccion** | Producción + OEE |
| **JefeCalidad** | Calidad |
| **Comercial** | Ventas + CRM |
| **Tecnico** | Mantenimiento + Incidencias (PWA móvil) |
| **Operario** | PWA móvil: incidencias, partes, ver mis OTs |

El frontend filtra el sidebar según el rol vía `roleGuard`.

### 5.2.1 Permisos granulares por usuario (Fase 3)

Además del filtrado por rol, cada `ApplicationUser` tiene `ModulosDeshabilitados` (lista CSV) que permite al Administrador **desactivar módulos concretos para un usuario individual** con granularidad fina, sobreescribiendo los defaults del rol.

- Entidad: `ApplicationUser.ModulosDeshabilitados` (string CSV, nullable).
- Interfaz: `IIdentityService.ListUsersByEmpresaAsync(empresaId)` y `UpdateModulosDeshabilitadosAsync(userId, modulos)`.
- Endpoints (solo rol `Administrador`):
  - `GET /api/v1/seguridad/usuarios` — lista usuarios de la empresa con sus módulos deshabilitados.
  - `PUT /api/v1/seguridad/usuarios/{userId}/modulos` — body `{ modulosDeshabilitados: string[] }`.
- Frontend: página `/app/permisos` (sólo Admin) con checkboxes por usuario sobre los 17 módulos disponibles.
- Sidebar (`app-shell.component.ts → navItems` computed): tras filtrar por rol, elimina módulos cuyo último segmento de ruta coincida (case-insensitive) con un valor en `user.modulosDeshabilitados`.

### 5.3 DB Seed

`DbInitializer.SeedAsync()` crea en cada arranque:
- Roles (los 8 anteriores).
- Empresa demo `Empresa Demo S.L.`.
- Usuarios demo: `admin@demo.com`, `gerente@demo.com`, `tecnico@demo.com` (password `Demo1234!`).
- Admin global: `admin@planta-erp.com / Admin2026!!`.

### 5.4 Registro self-service + onboarding (Fase 3)

El registro comercial ya no requiere alta manual. Flujo:

1. `POST /api/v1/seguridad/auth/register` — body `{ nombre, email, password, empresaNombre, cif? }`. `IIdentityService.RegisterEmpresaAsync` orquesta:
   - Verifica email único.
   - Crea `Empresa` vía `Empresa.Crear(nombre, cif, email, trialDias: 14)` → SaveChanges.
   - Crea `ApplicationUser` con `UserManager` (rollback de Empresa si falla).
   - Asigna rol `Administrador`.
   - Llama `LoginAsync` y devuelve `TokenPairDto` (acceso inmediato).
2. **Onboarding guard** (`onboardingGuard`): si `user.onboardingCompletado === false`, redirige cualquier ruta `/app/*` a `/app/onboarding`.
3. **Wizard** (`/app/onboarding`): 3 opciones — "Empezar vacío", "Cargar datos demo", "Importar CSV".
4. `POST /api/v1/seguridad/empresa/cargar-datos-demo` — `EmpresaDemoSeeder` crea 21 entidades (5 Leads CRM, 8 Productos, 3 Proveedores, 5 Empleados) y marca `OnboardingCompletado = true`.
5. `POST /api/v1/seguridad/empresa/completar-onboarding` — marca onboarding completado manualmente (opciones vacío / importar).
6. **Banner trial**: `app-shell` muestra un banner con días restantes calculados desde `user.trialHasta`, con variante `--expired` cuando `dias < 0`.

---

## 6. Frontend Angular 19

### 6.1 Estructura

```
planta-web/src/app/
├── core/
│   ├── auth/                 # AuthService, authGuard, roleGuard, AuthInterceptor (JWT)
│   ├── services/             # ApiService + 1 servicio HTTP por módulo
│   └── shared/               # ToastComponent, NotificationService, PaginationComponent,
│                                ExportService (CSV con BOM)
├── features/
│   ├── inventario/           # productos, almacenes, movimientos, lotes, alertas
│   ├── produccion/           # boms, rutas, ofs
│   ├── compras/              # proveedores, ocs, recepciones
│   ├── ventas/                # clientes, pedidos, expediciones
│   ├── calidad/               # plantillas, inspecciones, ncs
│   ├── activos/               # listado activos
│   ├── mantenimiento/         # órdenes
│   ├── incidencias/           # listado e historial
│   └── movil/                 # PWA: home, mis órdenes, registro de incidencia
├── layout/
│   ├── app-shell.component   # Sidebar filtrado por rol + topbar
│   └── login.component
└── app.routes.ts              # Lazy loading de cada feature
```

### 6.2 Patrones

- **Standalone components** + **OnPush** + **signals** en todos los nuevos.
- **Lazy loading** por feature.
- **Reactive forms** para CRUD modales.
- **Debounce 300ms** en buscadores.
- **PagedResult<T>** del backend mapeado a paginator común.

### 6.3 PWA móvil

- `manifest.webmanifest` con `display: standalone`, theme color, iconos.
- Rutas `/movil/*` con layout simplificado (sin sidebar) para operarios:
  - Home con accesos rápidos.
  - Listado de incidencias propias.
  - Formulario de alta de incidencia (cámara → adjunto pendiente).
  - Mis órdenes de mantenimiento.
- Optimizado táctil, instalable como app.

### 6.4 Servicios HTTP

`ApiService` genérico envuelve `HttpClient` con `HttpParams`. Cada módulo expone su servicio (ej. `InventarioService`) con métodos `listProductos(query)`, `getProducto(id)`, `createProducto(dto)`, etc., todos devolviendo `Observable<PagedResult<T>>` o `Observable<T>`.

### 6.5 Funcionalidad transversal

- **Toasts**: `NotificationService.success/error/info` muestran `ToastComponent` con auto-dismiss 4 s.
- **Confirmación**: modal genérico antes de borrar.
- **Export CSV**: `ExportService.toCsv(rows, filename)` con BOM UTF-8 para compatibilidad Excel.
- **Loading + error states** unificados en cada list page.

---

## 7. Configuración y deploy

### 7.1 Configuración local

| Archivo | Contenido |
|---|---|
| `appsettings.json` | Defaults sin secretos |
| `appsettings.Development.json` | Connection string Docker (gitignoreado) |
| `docker-compose.yml` | PostgreSQL 16 puerto **5433** + Seq |
| `planta-web/src/environments/environment.ts` | `apiUrl: '/api/v1'` (proxy) |
| `planta-web/proxy.conf.json` | `/api → http://localhost:5071` |

### 7.2 Configuración producción

| Servicio | URL/host | Notas |
|---|---|---|
| Frontend | Vercel | `ng build --configuration=production` con `fileReplacements` |
| API | Render (region Frankfurt) | Web Service desde Dockerfile o build .NET |
| BD | Neon PostgreSQL | Connection string en env vars de Render |

**Variables de entorno Render:**
```
ConnectionStrings__DefaultConnection=<neon-string>
Jwt__Key=<32+ chars>
Anthropic__ApiKey=<api-key>
Frontend__Url=https://planta-erp.vercel.app
ASPNETCORE_ENVIRONMENT=Production
```

### 7.3 CI/CD

- Push a `master` → Render redeploya la API automáticamente.
- Vercel detecta cambios en `planta-web/` y redeploya el frontend.
- Sin tests en pipeline aún (próximo paso).

### 7.4 Inicialización de la BD

Al primer arranque en producción, `DbInitializer` ejecuta `EnsureCreatedAsync()` por **cada uno de los 16 DbContext** (15 módulos + Outbox/Audit). Tras eso siembra roles, empresa demo y usuarios demo. **Importante:** al añadir un módulo nuevo, asegurarse de registrar su DbContext en el initializer.

---

## 8. Fase 3 — Funcionalidades comerciales

Tras cerrar la Fase 2 (CRUD completos en los 15 módulos), se añadieron 7 bloques puramente backend/frontend para convertir PlanTA en producto vendible. Otros 7 quedan diferidos (requieren servicios externos o decisiones comerciales: Stripe, integraciones bancarias, PWA avanzada, landing, soporte, GDPR, nicho).

### 8.1 Onboarding sin fricción (punto 1)

Ya documentado en §5.4. Self-service registration + trial 14 días + wizard + seed demo + banner.

### 8.2 Dashboard con KPIs reales (punto 3)

`dashboard.component.ts` ahora inyecta `CrmService`, `FacturacionService`, `RrhhService` además de los anteriores y usa `forkJoin` para cargar en paralelo. KPIs calculados client-side:

- **Facturado (mes)**: suma de `factura.total` de las facturas del mes en curso no `Borrador`/`Anulada`.
- **Pendiente cobro**: suma de facturas en estados `Emitida`/`Firmada`/`EnviadaVerifactu`.
- **Leads nuevos (7d)**: conteo de leads creados en los últimos 7 días.
- **Empleados**: total registrados.
- KPIs existentes (Total Productos, OFs, OCs, Pedidos, Inspecciones, Alertas Stock).

Formato monetario con `Intl.NumberFormat('es-ES', { style: 'currency', currency: 'EUR' })`. Fallback "Sin conexión API" si falla el forkJoin.

### 8.3 Exports CSV (punto 4)

`ExportService.exportToCsv(filename, headers, rows)` genera un CSV con BOM UTF-8 (compat Excel) y dispara la descarga. Integrado en:

- Facturas (`facturacion/facturas-list`).
- Clientes (`ventas/clientes/clientes-list`).
- Proveedores (`compras/proveedores/proveedores-list`).
- Empleados (`rrhh/empleados-list`).
- Leads (`crm/leads-list`).
- Ya existía en Productos (`inventario/productos/productos-list`).

Cada lista expone un botón "Exportar CSV" en el header. PDF de facturas y envío por email quedan diferidos.

### 8.4 Notificaciones in-app (punto 7)

Sistema de notificaciones persistidas en DB con campana en topbar. Email y push web diferidos.

**Backend**:
- Entidad `seguridad.Notificacion` con campos: `Id`, `EmpresaId`, `UsuarioId?` (null = broadcast), `Titulo`, `Mensaje`, `Tipo`, `Url?`, `Leida`, `CreatedAt`, `LeidaAt?`. Factory `Notificacion.Crear(...)` y método `MarcarLeida()`.
- `DbSet<Notificacion>` en `SeguridadDbContext` con índice compuesto `(EmpresaId, UsuarioId, Leida)`.
- Tabla creada vía `CREATE TABLE IF NOT EXISTS seguridad."Notificaciones"` idempotente en `Program.cs` (porque `CreateTablesAsync` aborta al primer conflicto).
- `NotificacionesEndpoints`:
  - `GET /api/v1/notificaciones?soloNoLeidas&take` — lista notificaciones del usuario (las suyas + broadcasts) y contador `noLeidas`.
  - `POST /api/v1/notificaciones` — crear (roles `Administrador`/`GerentePlanta`).
  - `POST /api/v1/notificaciones/{id}/marcar-leida`.
  - `POST /api/v1/notificaciones/marcar-todas-leidas`.

**Frontend**:
- `NotificacionesService` con signals `items` y `noLeidas`.
- Campana en `app-shell` topbar con badge contador y dropdown con lista.
- Polling cada 60 s via `setInterval` en `ngOnInit`; limpieza en `ngOnDestroy`.
- Click en una notificación: marca leída + navega a `n.url` si existe.

### 8.5 Permisos granulares (punto 8)

Ya documentado en §5.2.1.

### 8.6 Auditoría UI (punto 9)

El backend ya escribía `shared.AuditEntries` (via `AuditBehavior`). Se añade el frontend:

- `AuditoriaService` (frontend) consume `GET /api/v1/auditoria` con filtros `entityType`, `userId`, `from`, `to`, `page`, `pageSize`.
- Página `/app/auditoria` (roles `Administrador`/`GerentePlanta`):
  - Tabla con fecha, usuario, acción, entidad, ID, IP.
  - Botón "Ver cambios" expande una fila con `OldValues` / `NewValues` (JSON) lado a lado.
  - Filtros por tipo de entidad y usuario con debounce 300 ms.

### 8.7 Rendimiento y fiabilidad (punto 13)

- **GlobalErrorHandler Angular** (`core/services/error-handler.service.ts`): implementa `ErrorHandler`, captura errores no manejados y los envía via POST a `/api/v1/sistema/frontend-error`. Evita recursión con flag `reporting`. Ignora `HttpErrorResponse` (ya los gestionan los servicios). Registrado como provider en `app.config.ts`.
- **SistemaEndpoints**:
  - `GET /api/v1/sistema/health` — status, timestamp, versión del assembly (público).
  - `POST /api/v1/sistema/frontend-error` — payload `{ message, stack, url, userAgent, timestamp }` → `logger.LogError` (vía Serilog).
- **Middleware `X-Response-Time-ms`**: registrado en `Program.cs` antes de `UseCors`. Usa `Stopwatch` y `ctx.Response.OnStarting` para escribir la cabecera en todas las respuestas.
- Paginación server-side ya estaba en los listados principales (`PagedResult<T>` + `listX(query, page, pageSize)`).
- Sentry / status page quedan diferidos (requieren servicios externos).

### 8.8 Puntos diferidos

| Punto | Descripción | Por qué se difiere |
|---|---|---|
| 2 | Facturación SaaS (Stripe, planes, portal) | Cuenta Stripe + decisiones de pricing |
| 5 | Integraciones bancarias, email, WhatsApp, contabilidad | APIs externas + credenciales |
| 6 | App móvil PWA completa | Requiere decisión sobre alcance nativo vs PWA avanzada |
| 10 | Landing + contenido SEO + testimonios | Trabajo de marketing/copy |
| 11 | Soporte (chat live, help center, vídeos) | Herramientas externas |
| 12 | Cumplimiento GDPR (export/delete, backups, privacidad) | Requiere revisión legal |
| 14 | Foco de mercado | Decisión estratégica de nicho |

---

## 9. Estado actual (2026-04-15)

- ✅ 15 módulos backend implementados, build limpio (0 errores, 0 warnings).
- ✅ Cross-cutting (Outbox, Audit, MediatR Behaviors) funcionando.
- ✅ Frontend con features web + PWA móvil.
- ✅ Fase 3 comercial — 7 puntos backend/frontend cerrados (1, 3, 4, 7, 8, 9, 13), 7 diferidos (2, 5, 6, 10, 11, 12, 14).
- ✅ Deploy productivo Render + Vercel + Neon con los commits de Fase 3 pusheados (`bcbe80f..74299cd`).
- ⚠️ `Anthropic__ApiKey` debe estar configurado en Render para el asistente IA.
- ⚠️ Tests E2E pendientes. QA completo planificado con agente `qa-tester`.

---

## 10. Roadmap próximo

1. QA manual + automatizado de los 7 puntos de Fase 3 con el agente `qa-tester`.
2. Validar que Render crea la tabla `seguridad.Notificaciones` y los nuevos campos en `AspNetUsers` / `Empresas` al primer arranque tras el deploy.
3. Afrontar los diferidos por orden de impacto comercial: primero Stripe (punto 2), después landing (punto 10).
4. Tests de integración por módulo (xUnit + Testcontainers).
5. Notificaciones push web (VAPID) sobre la infraestructura in-app ya existente.
